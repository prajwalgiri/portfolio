namespace Portfolio.Site.Content;

/// <summary>
/// Strongly typed view of content/portfolio.json. Every page renders from this one model,
/// so the website and the CV PDF can never disagree.
/// </summary>
public sealed record PortfolioContent(
    SiteInfo Site,
    Person Person,
    IReadOnlyList<Fact> Facts,
    IReadOnlyList<Stat> Stats,
    IReadOnlyList<FeaturedWork> Featured,
    IReadOnlyList<Filter> Filters,
    IReadOnlyList<Project> Projects,
    About About,
    string Tools,
    IReadOnlyList<Job> Experience,
    Education Education,
    CvContent Cv,
    IReadOnlyDictionary<string, CaseStudyState> CaseStudies)
{
    public bool IsPublished(string? slug) =>
        slug is not null && CaseStudies.TryGetValue(slug, out var state) && state.Published;
}

public sealed record SiteInfo(string BaseUrl, string Title, string Description, string CvPdf);

public sealed record Person(
    string Name,
    string Initials,
    string Role,
    string Experience,
    string Intro,
    string HeadlineBefore,
    string HeadlineEmphasis,
    string HeadlineAfter,
    string Summary,
    string Photo,
    string PhotoAlt,
    string Location,
    string Email,
    string Phone,
    string Website,
    string LinkedIn,
    string GitHub)
{
    public string LinkedInDisplay => Display(LinkedIn);
    public string GitHubDisplay => Display(GitHub);

    private static string Display(string url) =>
        url.Replace("https://", "").Replace("www.", "").TrimEnd('/');
}

public sealed record Fact(string Label, string Value);

public sealed record Stat(string Value, string Label);

public sealed record FeaturedWork(string Diagram, string Kicker, string Title, string Summary, string? CaseStudy);

public sealed record Filter(string Id, string Label);

public sealed record Project(
    string Title,
    string Status,
    IReadOnlyList<string> Categories,
    string Summary,
    IReadOnlyList<string> Tags,
    string? Impact = null,
    string? Url = null,
    string? LinkLabel = null,
    string? Image = null,
    string? ImageAlt = null);

public sealed record About(string Statement, string Aside, IReadOnlyList<Principle> Principles);

public sealed record Principle(string Title, string Body);

public sealed record Job(
    string Title,
    string Company,
    string? Location,
    string Period,
    IReadOnlyList<JobGroup> Groups,
    IReadOnlyList<CvBullet> CvBullets);

public sealed record JobGroup(string Heading, string Body);

public sealed record CvBullet(string? Lead, string Text);

public sealed record Education(string Degree, string Institution, string Period, string Grade);

public sealed record CvContent(
    string RightToWork,
    string Profile,
    IReadOnlyList<Fact> Skills,
    IReadOnlyList<CvProject> OtherProjects);

public sealed record CvProject(string Name, string Text, string? Url);

public sealed record CaseStudyState(bool Published);
