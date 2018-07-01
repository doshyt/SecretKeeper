# SecretKeeper
Have you ever wanted to share something and make sure it's gone once it is accessed? This tiny web-based service allows you to do exactly that - to share **secrets** and **files** via one-time links.

![Example of UI](docs/UI_example.PNG "Example of UI")

Powered by ASP.NET Core 2.1 and Docker.

# Try it in action
I'm currently hosting a demo instance of SecretKeeper with a default self-sigend cert (expect a warning from browser!) in Azure:

https://secretkeeper.westeurope.azurecontainer.io

Check it out!

# Share a secret
Open `https://<your IP>/index` in a browser. Enter a secret text to be shared and press *Generate one-time link*. Copy the link and share it with someone. When the link is accessed, the content is rendered to the browser. At the same time, the link and the secret are erased and gone forever.

# Share a file
Open `https://<your IP>/upload` in a browser. Select a file to share and click *Upload*. Copy the link and share it with someone. When the link is accessed, the content is rendered to the browser. At the same time, the link and the file are erased and gone forever.

# Security features
- Secrets are stored encrypted in the in-memory database.
- Files are stored locally with randomized names and encrypted.
- Access is granted via supplying a link generated by computing SHA256 of random numbers.
- All secrets and files are removed after being accessed.
- All secrets and files expire after 5 min (by default).
- Configured with HTTPS by default, certificate is imported from PFX file under `Certificates/cert.pfx`.

# Build and Run
SecretKeeper is built in, with and for Docker on *Windows* and *Linux*. It relies on the Docker image *microsoft/dotnet:2.1-aspnetcore-runtime* and supports all OS kernels the the base image does. 
SecretKeeper is released in the form of a Docker image and publiched to the Docker Hub. 

To get it up and running, simply pull **igora/secretkeeper:latest**

To build and run it from the sources:

```
docker-compose -f docker-compose.yml -f docker-compose.override.yml up
```

Or, in 2 steps:

```
docker-compose -f docker-compose.yml -f docker-compose.override.yml --no-ansi build

docker-compose -f docker-compose.yml -f docker-compose.override.yml --no-ansi up -d --no-build --force-recreate --remove-orphans
```

Replace a default certificate in Certificates\cert.pfx with a production one that you would like to use for the web service.

By default, the container publishes the port 443 (https) to the host and makes it accessbile to other compiters on the network (firewall rules need to be added).

# API
Secrets can be generated by posting a json payload to the endpoint: 
```
POST /api/secret 
Host: <IP>
Content-Type: application/json
Cache-Control: no-cache

{
"Value": "<SECRET HERE>"
}
```

The response returns a link to the secret.

# Roadmap
- [x] Encrypt secrets before storing them in the DB
- [x] Prettify UI
- [x] Add Linux support
- [x] Implement file sharing
- [ ] Add integration to secret providers (Hashicorp Vault, Azure KeyVault)
