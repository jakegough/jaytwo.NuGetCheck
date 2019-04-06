FROM mcr.microsoft.com/dotnet/core/sdk:2.1 AS dotnet-sdk
FROM mcr.microsoft.com/dotnet/core/runtime:2.1-alpine AS dotnet-runtime

FROM dotnet-sdk AS base
RUN apt-get update \
  && apt-get install -y --no-install-recommends \
    make \
    mono-devel \
  && apt-get clean \
  && rm -rf /var/lib/apt/lists/*
ENV FrameworkPathOverride /usr/lib/mono/4.5/


FROM base AS builder
WORKDIR /build
COPY . /build
RUN make restore


FROM builder AS publisher
RUN make publish


FROM dotnet-runtime AS app
RUN echo "#!/bin/sh" > /usr/local/bin/nugetcheck \
  && echo "dotnet /app/jaytwo.NuGetCheck.dll \$@" >> /usr/local/bin/nugetcheck \
  && chmod +x /usr/local/bin/nugetcheck
COPY --from=publisher /build/out/published /app
ENTRYPOINT ["nugetcheck"]
