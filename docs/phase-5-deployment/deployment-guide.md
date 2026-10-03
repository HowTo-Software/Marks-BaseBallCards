# Frontend deployment guide

**Status:** repository-supported packaging; production setup pending Hank.
**Updated:** October 3, 2026.

## Existing delivery model

The API hosts the compiled client and REST endpoints on one origin. Its project
references Client and Shared; [the Dockerfile](../../src/Api/Dockerfile) publishes
the API into `/app/publish`, then runs `MarksBaseballCards.Api.dll` using the
.NET 10 ASP.NET runtime image and its non-root user.

[Compose](../../docker-compose.yml) defines one web service, port mapping
`8090:8080`, Production environment, `.env` configuration and
`restart: unless-stopped`. There is no SQL container. An external approved SQL
Server must be reachable before the API can initialize and serve the client.

## Development container integration

Obtain the correct development configuration from Hank. Copy the existing sample
to a local file, then replace its unsafe sample values with the approved settings:

```powershell
Copy-Item .env.example .env
```

Do not overwrite an existing configured `.env`. The sample SQL address and
privileged login are source examples, not this developer's infrastructure. Required
keys and seeding cautions are explained in [backend reference](backend-reference.md).

Once configured, the existing repository command is:

```powershell
docker compose up --build -d
docker compose ps
docker compose logs --tail 100 web
```

Open [localhost:8090](http://localhost:8090) for local integration. This is the HTTP
container mapping, not proof of a safe public endpoint. The earlier security review
records the TLS, SQL and secret-default concerns; production exposure needs their
remediation and Hank's approved host/proxy configuration.

## Publish without Docker

To produce the existing API-hosted artifact:

```powershell
dotnet publish src/Api/MarksBaseballCards.Api.csproj -c Release -o publish/web /p:UseAppHost=false
```

The artifact includes the frontend. Running it requires the configured database and
server settings. Production host service management, environment injection, domain
and TLS are not established by that command and remain pending Hank.

For a static client artifact only:

```powershell
dotnet publish src/Client/MarksBaseballCards.Client.csproj -c Release -o publish/client
```

The client's published `wwwroot` contains browser assets. A static-only host can
serve the archive, but cannot provide login, live inventory or payment APIs by
itself. Separate-origin hosting would require a deliberate API/auth/CORS design
change; it is not the configured architecture.

## Frontend artifact requirements

- Deploy the complete compiled client/framework assets with matching HTML, CSS,
  JavaScript, translations and local fonts.
- Preserve font license files and visible component attribution.
- Serve `data/collection.json` and local JS/font assets from their expected paths.
- Support SPA route fallback for direct route visits. The current API already does this.
- Keep the site base at `/` unless a separately reviewed subpath change is made.
- Review stale caches after replacing assets. The client project explicitly disables
  asset fingerprinting and HTML asset placeholder overrides.
- Keep all server secrets outside browser assets.

These are consequences of the current source, not a claim that a CDN/cache policy
has been configured.

## Release and rollback boundary

Before a frontend release, record its revision, artifact, public UI review and any
integration checks performed. Use [release notes](release-notes.md) and the
[test plan](../phase-4-testing/test-plan.md). The delivered frontend's historical
validation does not substitute for a new release check.

A frontend rollback must restore a coherent previous artifact compatible with the
backend contracts. Do not roll back HTML alone while leaving a different framework
or script bundle. Database migrations run at API startup, so rolling back the web
image is not automatically a database rollback. The actual release/rollback,
database recovery and payment reconciliation procedures are pending Hank.

CI/CD status is documented in [ci-cd.md](ci-cd.md).
