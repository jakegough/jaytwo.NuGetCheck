FROM microsoft/dotnet:2.2.104-sdk-stretch AS dotnet-sdk
FROM microsoft/dotnet:2.2.2-runtime-alpine AS dotnet-runtime

FROM dotnet-sdk AS base
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


FROM builder AS packer-beta
RUN make pack-beta


FROM alpine AS packer-beta-results
COPY --from=packer-beta /src/out /out


FROM builder AS packer
RUN make pack


FROM alpine AS packer-results
COPY --from=packer /src/out /out


FROM builder AS unit-test
RUN ((make unit-test) && (echo "PASS" > "out/testResults/.passed")) || (echo "FAIL" > "out/testResults/.failed")


FROM alpine AS unit-test-results
COPY --from=unit-test /src/out /out


FROM builder AS integration-test
RUN mkdir -p out/integrationTestResults \
  && ((make integration-test) && (echo "PASS" > "out/integrationTestResults/.passed")) || (echo "FAIL" > "out/integrationTestResults/.failed")


FROM alpine AS integration-test-results
COPY --from=integration-test /src/out /out


FROM builder AS publisher
WORKDIR /src
RUN make publish


FROM dotnet-runtime AS app
WORKDIR /app
COPY --from=publisher /src/out/published /app
ENTRYPOINT ["dotnet", "jaytwo.NuGetCheck.dll"]


FROM dotnet-sdk AS tooldemo
WORKDIR /root
ENV PATH="${PATH}:/root/.dotnet/tools"
COPY container.nuget.config /nuget.config
COPY --from=packer-results /out/packed/* /nupkgs/
RUN dotnet tool install -g jaytwo.NuGetCheck
