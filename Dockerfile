FROM microsoft/dotnet:2.2.104-sdk-stretch AS base
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


FROM builder AS packer
RUN make pack


FROM scratch AS packer-results
COPY --from=packer /src/out/packed/* /packed/


FROM base AS tooldemo
WORKDIR /root
ENV PATH="${PATH}:/root/.dotnet/tools"
COPY container.nuget.config /nuget.config
COPY --from=packer-results /packed/* /nupkgs/
RUN dotnet tool install -g jaytwo.NuGetCheck


FROM builder AS unit-test
RUN make unit-test || echo "FAIL" > "testResults/.failed"


FROM scratch AS unit-test-results
COPY --from=unit-test /src/out/testResults/* /testResults/


FROM builder AS packer-beta
RUN make pack-beta


FROM scratch AS packer-beta-results
COPY --from=packer-beta /src/out/packed/* /packed/


FROM builder AS publisher
WORKDIR /src
RUN make publish


FROM microsoft/dotnet:2.2.2-runtime-alpine AS app
WORKDIR /app
COPY --from=publisher /src/out/published /app
ENTRYPOINT ["dotnet", "jaytwo.NuGetCheck.dll"]