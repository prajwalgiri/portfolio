# Architecture decision: .NET static generation on Cloudflare Workers

**Status:** Accepted, October 2026

## Context

The portfolio should be built with .NET, hosted entirely on Cloudflare, load fast on a recruiter's phone, rank in search, and offer the CV as a PDF that never drifts from the website. Cloudflare Workers run JavaScript and WebAssembly; they cannot host an ASP.NET Core server.

## Options considered

| Option | For | Against |
| --- | --- | --- |
| **.NET static generation (chosen):** Razor components rendered to HTML at build time with `HtmlRenderer`, served as Worker static assets | All C#. Plain HTML: instant load, full SEO, works without JavaScript. No runtime, no cold starts, free tier. | Interactivity is limited to small progressive-enhancement scripts. |
| Blazor WebAssembly, served as static assets | Interactive C# in the browser | Downloads a 2–5 MB runtime before first paint; weak SEO without a prerendering server; overkill for a content site. |
| ASP.NET Core (Blazor SSR / Razor Pages) on a server | Familiar hosting model | Not possible on Workers alone; needs another host, patching and uptime. |

## Decision

Use a .NET 8 console generator. It reads `content/portfolio.json` into immutable records, renders each Razor page with `Microsoft.AspNetCore.Components.Web.HtmlRenderer`, and writes static files to `dist/`. It uses only the ASP.NET Core shared framework, so no NuGet packages are needed. An assets-only Cloudflare Worker serves `dist/`, with `_headers` for security and caching headers and `_redirects` for short links.

The CV is a normal page (`/cv/`) styled for one A4 sheet. CI prints it to `Prajwal-Giri-CV.pdf` with headless Chrome, so the site and the PDF come from one source.

## Consequences

- Content changes are a JSON edit and a push; GitHub Actions rebuilds and deploys in about a minute.
- Pages are typed: renaming a content field breaks the build instead of silently breaking the page (`TreatWarningsAsErrors` is on).
- The project filter is a small vanilla script. Without JavaScript every project is still listed.
- If the site later needs dynamic features (a contact form, say), add a `main` script to the Worker, or a Blazor WebAssembly island for that one page, without changing the rest.
