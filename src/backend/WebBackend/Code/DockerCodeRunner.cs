using System.Diagnostics;
using System.Text;
using Docker.DotNet;
using Docker.DotNet.Models;
namespace WebBackend.Code;

public record CodeRunResult(
    string Stdout,
    string Stderr,
    long ExitCode,
    bool TimedOut,
    long PeakMemoryBytes,
    long ExecutionTimeMs);

public class DockerCodeRunner
{
    private readonly DockerClient _client;
    private readonly string _uploadsVolumeName;

    public DockerCodeRunner(string uploadsVolumeName)
    {
        _client = new DockerClientConfiguration(new Uri("unix://var/run/docker.sock")).CreateClient();
        _uploadsVolumeName = uploadsVolumeName;
    }

    public async Task<CodeRunResult> RunAsync(string relativeSubmissionPath, string image, string[] cmd, int timeoutSeconds = 10)
    {
        var createResponse = await _client.Containers.CreateContainerAsync(new CreateContainerParameters
        {
            Image = image,
            Cmd = cmd,
            AttachStdout = true,
            AttachStderr = true,
            NetworkDisabled = true,
            HostConfig = new HostConfig
            {
                Binds = new[] { $"{_uploadsVolumeName}:/mnt/uploads:ro" },
                Memory = 256 * 1024 * 1024,
                NanoCPUs = 1_000_000_000,
                AutoRemove = false,
                ReadonlyRootfs = true,
                Tmpfs = new Dictionary<string, string> { { "/tmp", "rw,size=64m" } },
                CapDrop = new[] { "ALL" },
                SecurityOpt = new[] { "no-new-privileges" }
            },
            User = "nobody"
        });
        
        var containerId = createResponse.ID;
        bool timedOut = false;
        long peakMemoryBytes = 0;

        using var statsCts = new CancellationTokenSource();

        var statsTask = Task.Run(async () =>
        {
            try
            {
                var progress = new Progress<ContainerStatsResponse>(stats =>
                {
                    var used = (long)stats.MemoryStats.Usage;
                    if (used > peakMemoryBytes)
                        peakMemoryBytes = used;
                });

                await _client.Containers.GetContainerStatsAsync(
                    containerId,
                    new ContainerStatsParameters { Stream = true },
                    progress,
                    statsCts.Token);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                Console.WriteLine($"[stats-error] {ex.GetType().Name}: {ex.Message}");
            }
        });

        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _client.Containers.StartContainerAsync(containerId, new ContainerStartParameters());

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
            try
            {
                await _client.Containers.WaitContainerAsync(containerId, cts.Token);
            }
            catch (OperationCanceledException)
            {
                timedOut = true;
                await _client.Containers.KillContainerAsync(containerId, new ContainerKillParameters());
            }

            stopwatch.Stop();

            statsCts.Cancel();
            await Task.WhenAny(statsTask, Task.Delay(500));

            var logsStream = await _client.Containers.GetContainerLogsAsync(
                containerId, tty: false,
                new ContainerLogsParameters { ShowStdout = true, ShowStderr = true });
            var (stdout, stderr) = await ReadLogsAsync(logsStream);

            var inspect = await _client.Containers.InspectContainerAsync(containerId);
            Console.WriteLine($"[debug] peakMemoryBytes={peakMemoryBytes} elapsedMs={stopwatch.ElapsedMilliseconds}");
            return new CodeRunResult(
                stdout, stderr, inspect.State.ExitCode, timedOut,
                peakMemoryBytes, stopwatch.ElapsedMilliseconds);
        }
        finally
        {
            statsCts.Cancel();
            try
            {
                await _client.Containers.RemoveContainerAsync(containerId,
                    new ContainerRemoveParameters { Force = true });
            }
            catch (DockerApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Conflict) { }
            catch (DockerContainerNotFoundException) { }
        }
    }

    private async Task<(string, string)> ReadLogsAsync(MultiplexedStream stream)
    {
        var stdoutBuf = new MemoryStream();
        var stderrBuf = new MemoryStream();
        await stream.CopyOutputToAsync(null, stdoutBuf, stderrBuf, CancellationToken.None);
        return (Encoding.UTF8.GetString(stdoutBuf.ToArray()), Encoding.UTF8.GetString(stderrBuf.ToArray()));
    }
}