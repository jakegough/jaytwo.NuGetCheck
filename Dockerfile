FROM microsoft/dotnet:2.0-sdk-stretch AS base
RUN apt-get update \
  && apt-get install -y --no-install-recommends \
    make \
  && rm -rf /var/lib/apt/lists/*


FROM base AS builder
WORKDIR /src
COPY . /src
RUN make build


FROM builder AS publisher
RUN make publish


FROM microsoft/dotnet:2.1-runtime-alpine AS app
WORKDIR /app
COPY --from=publisher /src/out /app
ENTRYPOINT ["dotnet", "jaytwo.NuGetCheck.dll"]