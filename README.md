# source-to-image-hello

A minimal ASP.NET Core app for testing Source-to-Image (S2I) builds on OpenShift.

There is deliberately no Dockerfile: OpenShift should detect the `.csproj`, pick the **.NET builder image**, and build the container image itself.

## Endpoints

| Path       | Returns                                                        |
|------------|----------------------------------------------------------------|
| `/`        | Plain-text page with pod name, .NET runtime, build name, commit |
| `/info`    | The same information as JSON                                   |
| `/healthz` | `{"status":"ok"}` — use for readiness/liveness probes          |

## Deploy from the OpenShift Web Console

1. Switch to the **Developer** perspective and click **+Add**.
2. Choose **Git Repository → Import from Git**.
3. Paste this repository's Git URL.
4. Under the import strategy, make sure **Builder Image** is selected (not Dockerfile) and the builder is **.NET**.
5. Pick builder image version **10.0** — it must match `TargetFramework` in `HelloS2I.csproj`.
6. Leave the target port at **8080** and keep **Create a route** ticked.
7. Click **Create**.

When the build finishes and the pod is running, open the route URL from the Topology view. The page shows the pod name and the OpenShift build name, which confirms the image came from the S2I build.

## Deploy with the CLI

```bash
oc new-app dotnet:10.0~<git-repo-url> --name=hello-s2i
oc logs -f buildconfig/hello-s2i
oc expose service/hello-s2i
oc get route hello-s2i
```

## Run locally

```bash
dotnet run --urls http://localhost:8080
curl http://localhost:8080/
```

## Run locally with Docker

`local/Dockerfile` reproduces the S2I build using the same Red Hat .NET builder image and assemble/run scripts that OpenShift uses. Run from the repository root:

```bash
docker build -f local/Dockerfile -t hello-s2i .
docker run --rm -p 8080:8080 hello-s2i
curl http://localhost:8080/
```

The Dockerfile is kept in `local/` rather than the repository root so that OpenShift's Import from Git still selects the Builder Image strategy.

## Notes

- The app does not set a port itself. The .NET builder image sets `ASPNETCORE_URLS=http://*:8080`, which is what the Service and Route expect.
- If the cluster only offers a different .NET builder version, change `TargetFramework` in `HelloS2I.csproj` to match (for example `net8.0` for `dotnet:8.0`) and update the `FROM` line in `local/Dockerfile`.
- For a private repository, add a source secret in the import form under **Show advanced Git options**.
