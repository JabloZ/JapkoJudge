FROM mcr.microsoft.com/dotnet/sdk:10.0 AS development
WORKDIR /app
COPY WebBackend/WebBackend.csproj /app/
RUN dotnet restore
COPY . /app/
EXPOSE 5000
CMD ["dotnet","watch","run"]