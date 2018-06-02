FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY SecretKeeper/SecretKeeper.csproj SecretKeeper/
RUN dotnet restore SecretKeeper/SecretKeeper.csproj
COPY . .
WORKDIR /src/SecretKeeper
RUN dotnet build SecretKeeper.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish SecretKeeper.csproj -c Release -o /app

FROM base AS final
COPY --from=publish /app .
COPY ./SecretKeeper/Certificates/cert.pfx /app/Certificates/cert.pfx
ENTRYPOINT ["dotnet", "SecretKeeper.dll"]
