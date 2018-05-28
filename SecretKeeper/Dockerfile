FROM microsoft/aspnetcore:2.0-nanoserver-1709 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0-nanoserver-1709 AS build
WORKDIR /src
COPY SecretKeeper/SecretKeeper.csproj SecretKeeper/
RUN dotnet restore SecretKeeper/SecretKeeper.csproj
COPY . .
WORKDIR /src/SecretKeeper
RUN dotnet build SecretKeeper.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish SecretKeeper.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "SecretKeeper.dll"]
