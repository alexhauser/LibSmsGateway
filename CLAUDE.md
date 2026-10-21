# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

LibSmsGateway is a small .NET library (published to NuGet as `LibSmsGateway`) for sending SMS via the www.sms-gateway.at `HTTP2SMS` API. Both projects target `netcoreapp3.1`.

## Commands

The solution file lives inside the library folder, not at the repo root:

```sh
dotnet build LibSmsGateway/LibSmsGateway.sln     # builds library + TestClient; also produces the .nupkg (GeneratePackageOnBuild)
dotnet run --project TestClient                  # manual smoke test - sends a real SMS
```

There is no unit test project and no linter. `TestClient` is a console app for manual testing: it reads `apikey` (or legacy `user` and `password`) from `TestClient/appsecrets.json` (required, gitignored, copied to output), and running it hits the live API and may incur charges.

## Architecture

- `SmsClient` supports two auth modes: API key (`SmsClient(apiKey)`, preferred; sent as the `X-Api-Key` header, never in the URL) and the deprecated username/password (`[Obsolete]` constructor; sent as `username`/`validpass` query params).
- `SmsClient` builds a GET request to `https://www.sms-gateway.at/sms/sendsms.php` with all other parameters in the query string (`message`, `number[]`, `group[]`, `absender`, `receipt`, `flash`, `encoding=utf8`). The single-recipient `Send` overload delegates to the array overload. Sender names are sanitized (special chars stripped, max 10 chars). `Send` does not throw on network failure; exceptions are caught and returned as `StatusCode.UnknownError`.
- `SmsResult.FromHttpResponse` parses the API's XML response body: `<result>` of `OK`/`OK:` means success; `ERROR:` maps `<errorcode>` onto the `StatusCode` enum, and anything unrecognized becomes `UnknownError` (999). The API's success string has changed before (see commit 199b70a), so this parsing is the fragile part when deliveries are misreported.
- `StatusCode` enum values are the API's numeric error codes; add new codes there so they aren't collapsed into `UnknownError`.

## Releasing

Version and release notes are set in `LibSmsGateway/LibSmsGateway.csproj` (`<Version>`, `<PackageReleaseNotes>`). Version bumps are committed separately ("Bump version to x.y.z"). Keep the usage example in `README.md` in sync with `TestClient/Program.cs` and the public `Send` signatures.
