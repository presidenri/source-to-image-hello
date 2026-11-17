# Local test only. Reproduces the OpenShift S2I build by running the same
# builder image and the same assemble/run scripts that OpenShift uses.
#
# This file lives in local/ on purpose: a Dockerfile in the repository root
# would make OpenShift's "Import from Git" pick the Dockerfile strategy
# instead of the Builder Image (S2I) strategy.
#
# Build from the repository root:
#   docker build -f local/Dockerfile -t hello-s2i .
#   docker run --rm -p 8080:8080 hello-s2i

# Keep the version in step with TargetFramework in HelloS2I.csproj
# (ubi9/dotnet-100 = net10.0, ubi8/dotnet-80 = net8.0).
#
# Red Hat tags these images by build timestamp, not by .NET version.
# Tag 1794295111 contains .NET 10.0.14 (SDK 10.0.114).
FROM registry.access.redhat.com/ubi9/dotnet-100:1794295111

# S2I injects the source at /tmp/src, owned by the non-root build user.
USER 0
COPY . /tmp/src
RUN chown -R 1001:0 /tmp/src
USER 1001

# Restore, publish and install the app, exactly as the OpenShift build does.
RUN /usr/libexec/s2i/assemble

EXPOSE 8080
CMD ["/usr/libexec/s2i/run"]
