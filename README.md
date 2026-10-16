# prajwalgiri.com.np

Personal portfolio and CV of Prajwal Giri, Senior .NET Developer.

The site is written in **C# and Razor**, rendered to static HTML at build time by a small .NET 8 generator, and served by a **Cloudflare Worker** (static assets). There is no server to run and nothing to keep patched.

```
content/portfolio.json ──► .NET generator (Razor + HtmlRenderer) ──► dist/ ──► Cloudflare Worker
                                                                       └──► Prajwal-Giri-CV.pdf (headless Chrome, in CI)
```

## Repository layout

| Path | What it is |
| --- | --- |
| `content/portfolio.json` | All site and CV content. Edit this to update the site. |
| `src/Portfolio.Site/Program.cs` | The generator: loads content, renders each page, writes `dist/`. |
| `src/Portfolio.Site/Content/` | Typed content model and the small inline formatter. |
| `src/Portfolio.Site/Pages/` | One Razor component per page: home, CV, case study, 404. |
| `src/Portfolio.Site/Shared/` | Document shell, header, architecture diagrams. |
| `src/Portfolio.Site/wwwroot/` | CSS, JS, images, `_headers`, `_redirects`, copied as-is. |
| `wrangler.jsonc` | Cloudflare Worker config (assets only). |
| `.github/workflows/deploy.yml` | Build, print the CV PDF, deploy on every push to `main`. |
| `docs/ARCHITECTURE.md` | Why it is built this way. |

## Run locally

Requires the .NET 8 SDK.

```bash
dotnet run --project src/Portfolio.Site -- --out dist
npx wrangler dev          # serves dist/ exactly as Cloudflare will, on http://localhost:8787
```

Any static server works too, for example `python3 -m http.server 8080 --directory dist`.

## Editing content

- **Text, projects, experience, CV:** edit `content/portfolio.json`. In the `tools` field, `**word**` is bold and `==word==` is bold in the accent colour.
- **Project screenshot:** add a `.webp` to `wwwroot/images/work/` and set the project's `image` and `imageAlt`.
- **Case study:** `Pages/ShareRegistry.razor` has placeholders marked `class="todo"`. Fill them, then set `caseStudies.share-registry.published` to `true`. The generator warns about any TODO left in a published page.
- **CV PDF:** generated from `/cv/` on each deploy, so it always matches the site.

## Deploying

Pushing to `main` builds and deploys. One-time setup:

1. In Cloudflare, create an API token with the **Edit Cloudflare Workers** template.
2. In this repo, go to **Settings › Secrets and variables › Actions** and add:
   - `CLOUDFLARE_API_TOKEN`: the token from step 1
   - `CLOUDFLARE_ACCOUNT_ID`: from the Cloudflare dashboard (Workers & Pages overview, right-hand side)
3. Push to `main` or run the workflow manually.

The site is served at **https://prajwalgiri.com.np/**. The custom domain is declared under `routes` in `wrangler.jsonc`, so each deploy keeps it attached to the Worker.

## Releases

Versions are recorded in `CHANGELOG.md`. To cut a release, add a `## [x.y.z] - YYYY-MM-DD` section at the top and merge to `main`; the Release workflow tags that commit and publishes a GitHub release with the section as its notes.

## Theme

Light and dark mode follow the visitor's system setting by default. The toggle in the header saves an explicit choice in `localStorage`. `js/theme.js` loads in `<head>` so the saved theme applies before first paint, and the CSS keeps a `prefers-color-scheme` fallback for visitors without JavaScript. The CV page is always light because it is printed.

Pull requests build the site and the PDF but do not deploy; the output is attached to the run as an artifact.
