FROM mcr.microsoft.com/dotnet/sdk:10.0.103 AS development
WORKDIR /app
COPY WebBackend/WebBackend.csproj /app/
RUN dotnet restore
COPY . /app/
RUN dotnet tool restore
ENV ASPNETCORE_URLS=http://0.0.0.0:8001
EXPOSE 8001
CMD ["dotnet", "watch" ,"run", "--no-launch-profile"]