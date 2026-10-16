# Changelog

All notable changes to prajwalgiri.com.np. Adding a new `## [x.y.z]` section at the top and merging to `main` publishes a GitHub release with these notes (see `.github/workflows/release.yml`).

## [1.0.0] - 2026-10-16

First public release of the portfolio at https://prajwalgiri.com.np.

### Site
- Home page: hero with photo, quick facts (location, right to work, availability), headline results, two featured projects with architecture diagrams, filterable list of eleven projects with screenshots, working principles, skills, experience and contact.
- One-page A4 CV at `/cv/`, printed to `Prajwal-Giri-CV.pdf` on every deploy so the PDF always matches the site.
- Light and dark mode: follows the system setting, with a header toggle that remembers the visitor's choice and applies it before first paint.
- 404 page, sitemap, robots.txt, Open Graph tags, short links (`/cv.pdf`, `/resume`).

### Engineering
- .NET 8 static site generator: Razor components rendered to HTML at build time with `HtmlRenderer`, no NuGet packages.
- All content in `content/portfolio.json`, loaded into typed records; warnings are errors.
- Served by an assets-only Cloudflare Worker on the custom domain, with security headers (CSP, frame and referrer policies) and cache rules.
- GitHub Actions: build and print the CV on every push and pull request, deploy to Cloudflare on `main`, fail early with a clear message when secrets are missing.
- Architecture decision record in `docs/ARCHITECTURE.md`.
