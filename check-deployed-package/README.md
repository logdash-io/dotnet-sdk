# LogDash .NET SDK Demo

Use command below to test if the published SDK works:

```
LOGDASH_API_KEY=<ingest-key> LOGDASH_PERSONAL_API_KEY=<personal-api-key> ./check-deployed-package/run.sh
```

It sends logs and metrics as the `dotnet-sdk` app with the ingest key of the shared "SDK smoke tests" domain, then reads them back with the personal API key: an `ldp_` key with logs:read and metrics:read, restricted to that domain.
It checks the latest version on NuGet; set `LOGDASH_VERSION`, such as `LOGDASH_VERSION=0.1.0`, to check another.
