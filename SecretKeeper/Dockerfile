FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.2-sdk AS build
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
WORKDIR /app/wwwroot
RUN mkdir Uploads
WORKDIR /app
COPY ./SecretKeeper/Certificates/cert.pfx /app/Certificates/cert.pfx
EXPOSE 443
ENTRYPOINT ["dotnet", "SecretKeeper.dll"]
