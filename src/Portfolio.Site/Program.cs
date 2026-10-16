using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Portfolio.Site.Content;
using Portfolio.Site.Pages;

// Static site generator: renders Razor components to plain HTML at build time.
// Output is a folder of static files that a Cloudflare Worker serves as static assets.
//
//   dotnet run --project src/Portfolio.Site -- --out dist

var root = FindRepoRoot();
var outDir = Path.GetFullPath(ArgValue(args, "--out") ?? Path.Combine(root, "dist"));
var contentPath = ArgValue(args, "--content") ?? Path.Combine(root, "content", "portfolio.json");
var wwwroot = Path.Combine(root, "src", "Portfolio.Site", "wwwroot");

var content = LoadContent(contentPath);

if (Directory.Exists(outDir)) Directory.Delete(outDir, recursive: true);
Directory.CreateDirectory(outDir);
CopyDirectory(wwwroot, outDir);

var services = new ServiceCollection().AddLogging().BuildServiceProvider();
await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());

var pages = new List<(string Path, Type Component, string Url)>
{
    ("index.html", typeof(Home), "/"),
    ("cv/index.html", typeof(Cv), "/cv/"),
    ("404.html", typeof(NotFound), ""),
};
if (content.IsPublished("share-registry"))
{
    pages.Add(("work/share-registry/index.html", typeof(ShareRegistry), "/work/share-registry/"));
}

var todoCount = 0;
foreach (var page in pages)
{
    var html = await RenderAsync(renderer, page.Component, content);
    todoCount += CountOccurrences(html, "class=\"todo\"");
    var target = Path.Combine(outDir, page.Path);
    Directory.CreateDirectory(Path.GetDirectoryName(target)!);
    await File.WriteAllTextAsync(target, "<!DOCTYPE html>\n" + html, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
    Console.WriteLine($"  rendered {page.Path}");
}

await File.WriteAllTextAsync(Path.Combine(outDir, "sitemap.xml"), BuildSitemap(content, pages.Select(p => p.Url)));
Console.WriteLine($"Built {pages.Count} pages into {outDir}");
if (todoCount > 0)
{
    Console.WriteLine($"warning: {todoCount} placeholder(s) still marked TODO in published pages.");
}

return 0;

static async Task<string> RenderAsync(HtmlRenderer renderer, Type component, PortfolioContent content) =>
    await renderer.Dispatcher.InvokeAsync(async () =>
    {
        var parameters = ParameterView.FromDictionary(new Dictionary<string, object?> { ["Content"] = content });
        var output = await renderer.RenderComponentAsync(component, parameters);
        return output.ToHtmlString();
    });

static PortfolioContent LoadContent(string path)
{
    var options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };
    return JsonSerializer.Deserialize<PortfolioContent>(File.ReadAllText(path), options)
        ?? throw new InvalidOperationException($"Could not read content from {path}");
}

static string BuildSitemap(PortfolioContent content, IEnumerable<string> urls)
{
    var sb = new StringBuilder("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">\n");
    foreach (var url in urls.Where(u => u.Length > 0))
    {
        sb.Append("  <url><loc>").Append(content.Site.BaseUrl.TrimEnd('/')).Append(url).Append("</loc></url>\n");
    }
    return sb.Append("</urlset>\n").ToString();
}

static void CopyDirectory(string source, string destination)
{
    foreach (var dir in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
    {
        Directory.CreateDirectory(dir.Replace(source, destination));
    }
    foreach (var file in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
    {
        File.Copy(file, file.Replace(source, destination), overwrite: true);
    }
}

static int CountOccurrences(string text, string value)
{
    var count = 0;
    for (var i = text.IndexOf(value, StringComparison.Ordinal); i >= 0; i = text.IndexOf(value, i + value.Length, StringComparison.Ordinal))
    {
        count++;
    }
    return count;
}

static string? ArgValue(string[] args, string name)
{
    var index = Array.IndexOf(args, name);
    return index >= 0 && index + 1 < args.Length ? args[index + 1] : null;
}

static string FindRepoRoot()
{
    for (var dir = new DirectoryInfo(Directory.GetCurrentDirectory()); dir is not null; dir = dir.Parent)
    {
        if (File.Exists(Path.Combine(dir.FullName, "content", "portfolio.json"))) return dir.FullName;
    }
    throw new InvalidOperationException("Run from inside the repository (content/portfolio.json not found).");
}
