FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY BoardGameRankings.slnx ./
COPY global.json ./
COPY src/BoardGameRankings.Api/BoardGameRankings.Api.csproj src/BoardGameRankings.Api/
COPY src/BoardGameRankings.Application/BoardGameRankings.Application.csproj src/BoardGameRankings.Application/
COPY src/BoardGameRankings.Domain/BoardGameRankings.Domain.csproj src/BoardGameRankings.Domain/
COPY src/BoardGameRankings.Infrastructure/BoardGameRankings.Infrastructure.csproj src/BoardGameRankings.Infrastructure/
COPY src/BoardGameRankings.DevTools/BoardGameRankings.DevTools.csproj src/BoardGameRankings.DevTools/
COPY tests/BoardGameRankings.Application.Tests/BoardGameRankings.Application.Tests.csproj tests/BoardGameRankings.Application.Tests/
COPY tests/BoardGameRankings.Domain.Tests/BoardGameRankings.Domain.Tests.csproj tests/BoardGameRankings.Domain.Tests/

RUN dotnet restore BoardGameRankings.slnx

COPY src/ src/
COPY data/ data/

RUN dotnet publish src/BoardGameRankings.Api/BoardGameRankings.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

ENTRYPOINT ["dotnet", "BoardGameRankings.Api.dll"]
