namespace WebBackend.Code;
public static class LanguageRunner
{
    public static(string image, string[] cmd) Get(string languageName, string solutionPathInContainer)
    {
        return languageName switch
        {
            "python"=>("python:3.12-alpine",new[] {"python",solutionPathInContainer})
        };
    }
}