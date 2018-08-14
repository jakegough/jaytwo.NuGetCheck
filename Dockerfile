FROM microsoft/dotnet:2.0.9-sdk-2.1.202-stretch AS base
RUN apt-get update \
  && apt-get install -y --no-install-recommends \
    make \
  && rm -rf /var/lib/apt/lists/*


FROM base AS restored
WORKDIR /src
COPY jaytwo.NuGetCheck.sln .
COPY src/jaytwo.NuGetCheck/jaytwo.NuGetCheck.csproj src/jaytwo.NuGetCheck/jaytwo.NuGetCheck.csproj
COPY test/jaytwo.NuGetCheck.Tests/jaytwo.NuGetCheck.Tests.csproj test/jaytwo.NuGetCheck.Tests/jaytwo.NuGetCheck.Tests.csproj
RUN dotnet restore . --verbosity minimal


FROM restored AS builder
WORKDIR /src
COPY . /src
RUN make clean build


FROM builder AS unit-test
RUN make unit-test || echo "FAIL" > "testResults/.failed"


FROM alpine AS unit-test-results
COPY --from=unit-test /src/testResults/* /testResults/


FROM builder AS packer
RUN make pack


FROM alpine AS packer-results
COPY --from=packer /src/out/* /out/


FROM builder AS packer-beta
RUN make pack-beta


FROM alpine AS packer-beta-results
COPY --from=packer-beta /src/out/* /out/


FROM builder AS publisher
RUN make publish


FROM microsoft/dotnet:2.0.9-runtime AS app
WORKDIR /app
COPY --from=publisher /src/out /app
ENTRYPOINT ["dotnet", "jaytwo.NuGetCheck.dll"]