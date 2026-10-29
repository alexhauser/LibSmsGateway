# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

LibSmsGateway is a small .NET library (published to NuGet as `LibSmsGateway`) for sending SMS via the www.sms-gateway.at `HTTP2SMS` API. Both projects multi-target `netcoreapp3.1` and `net48` (net48 builds use the `Microsoft.NETFramework.ReferenceAssemblies` package, so no targeting pack install is needed; keep library code within C# 7.3, the net48 default). The library is strong-named with the committed key `LibSmsGateway/LibSmsGateway.snk` (committed deliberately, per Microsoft's open-source guidance); never regenerate or replace it, since that changes the assembly identity and breaks binary compatibility for consumers.

## Commands

The solution file lives inside the library folder, not at the repo root:

```sh
dotnet build LibSmsGateway/LibSmsGateway.sln     # builds library + TestClient; also produces the .nupkg (GeneratePackageOnBuild)
dotnet run --project TestClient -f net48         # manual smoke test - sends a real SMS (or -f netcoreapp3.1)
```

There is no unit test project and no linter. `TestClient` is a console app for manual testing: it reads `apikey` (or legacy `user` and `password`) from `TestClient/appsecrets.json` (required, gitignored, copied to output; create it from `TestClient/appsecrets.example.json` - `apikey` wins if set, otherwise `user`/`password` are used), and running it hits the live API and may incur charges.

## Architecture

- `SmsClient` supports two auth modes: API key (`SmsClient(apiKey)`, preferred; sent as the `X-Api-Key` header, never in the URL) and the deprecated username/password (`[Obsolete]` constructor; sent as `username`/`validpass` query params).
- `SmsClient` builds a GET request to `https://www.sms-gateway.at/sms/sendsms.php` with all other parameters in the query string (`message`, `number[]`, `group[]`, `absender`, `receipt`, `flash`, `encoding=utf8`). The single-recipient `Send` overload delegates to the array overload. Sender names are sanitized (special chars stripped, max 10 chars). `Send` does not throw on network failure; exceptions are caught and returned as `StatusCode.UnknownError`.
- `SmsResult.FromHttpResponse` parses the API's XML response body: `<result>` of `OK`/`OK:` means success; `ERROR:` maps `<errorcode>` onto the `StatusCode` enum, and anything unrecognized becomes `UnknownError` (999). The API's success string has changed before (see commit 199b70a), so this parsing is the fragile part when deliveries are misreported.
- `SmsClient.GetHistory(SmsHistoryQuery)` queries the "SMS-Ausgang" endpoint `https://portal.sms-gateway.at/schnittstelle/ausgang.php` (different host). It is API-key only (`X-API-Key` header; throws `InvalidOperationException` for username/password clients) and returns JSON, parsed by `SmsHistoryResult.FromHttpResponse` with `DataContractJsonSerializer` (no extra NuGet dependency) into `SentSms`/`SentSmsRecipient`. Errors arrive with a non-2xx HTTP status but still have a JSON body `{"ok":false,"code":"109","fehler":"..."}`, so the body is parsed regardless of HTTP status.
- `StatusCode` enum values are the API's numeric error codes; add new codes there so they aren't collapsed into `UnknownError`.

## Releasing

Version and release notes are set in `LibSmsGateway/LibSmsGateway.csproj` (`<Version>`, `<PackageReleaseNotes>`). Version bumps are committed separately ("Bump version to x.y.z"). Keep the usage example in `README.md` in sync with `TestClient/Program.cs` and the public `Send` signatures.
