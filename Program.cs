using System.Runtime.InteropServices;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var startedAt = DateTimeOffset.UtcNow;

// Human-readable page: confirms the image was built and the Route works.
app.MapGet("/", () => Results.Text($"""
    Hello from .NET on OpenShift (built with S2I)!

    Pod:        {Environment.MachineName}
    Runtime:    {RuntimeInformation.FrameworkDescription}
    OS:         {RuntimeInformation.OSDescription}
    Build:      {Environment.GetEnvironmentVariable("OPENSHIFT_BUILD_NAME") ?? "(not an OpenShift build)"}
    Commit:     {Environment.GetEnvironmentVariable("OPENSHIFT_BUILD_COMMIT") ?? "(unknown)"}
    Started at: {startedAt:u}
    Now:        {DateTimeOffset.UtcNow:u}
    """));

// Same information as JSON, handy for curl.
app.MapGet("/info", () => new
{
    pod = Environment.MachineName,
    runtime = RuntimeInformation.FrameworkDescription,
    os = RuntimeInformation.OSDescription,
    build = Environment.GetEnvironmentVariable("OPENSHIFT_BUILD_NAME"),
    commit = Environment.GetEnvironmentVariable("OPENSHIFT_BUILD_COMMIT"),
    startedAt
});

// Target for readiness/liveness probes.
app.MapGet("/healthz", () => Results.Ok(new { status = "ok" }));

app.Run();
