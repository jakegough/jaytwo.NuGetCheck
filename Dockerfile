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
COPY Makefile .
RUN make restore


FROM base AS builder
WORKDIR /src
COPY . /src
RUN make build


FROM builder AS publish
RUN make publish


FROM builder AS unit-test
RUN chmod a+x scripts/*.sh
RUN make unit-test || echo "FAIL" > "testResults/.failed"


FROM alpine AS unit-test-results
COPY --from=unit-test /src/testResults/* /testResults/


FROM microsoft/dotnet:2.0.9-runtime AS app
WORKDIR /app
COPY --from=publisher /src/out /app
ENTRYPOINT ["dotnet", "jaytwo.NuGetCheck.dll"]