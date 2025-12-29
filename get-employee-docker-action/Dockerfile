FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

WORKDIR /App
COPY ./src ./
RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /App
COPY --from=build-env /App/out .
COPY --from=build-env /entrypoint.sh /entrypoint.sh
ENTRYPOINT ["dotnet", "/App/DotNetDockerAction.dll"]

