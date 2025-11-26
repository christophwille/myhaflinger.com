// Created by GitHub Copilot in VSCode MSSQL - review carefully before executing
/*
@mssql The csmyhaflinger database contains the contents for the CommunityServer .NET forum software. All custom objects start with cs_. The forum was subdivided by sections (cs_Sections table) and the posts (cs_posts table) contained in those sections. Posts are hierarchical, the ParentId in cs_Posts points to PostId - for the root of a conversation, both are the same. There is a cs_Threads table, maybe that can be helpful too. Read the schema.

The subject and body in the database is already html escaped.

The following folders exists on disk already:

* CommunityServer.Components.ImageFileViewer
* CommunityServer.Components.PostAttachments
* CommunityServer.Components.SiteFiles
* CommunityServer.Components.UserFiles
* photos

If you find a tag like <a href="/cfs-file.ashx/__key/CommunityServer.Components.UserFiles/00.00.00.36.97/Hebel-beschleunigter-Leerlauf-parallel.jpg"> it should be modified to point to ./CommunityServer.Components.UserFiles and the 00.00.00.36.97 is actually 00/00/00/36/97 (the points represent subdirectories). Do the same for all listed existing folders above.

For image tags like <img src="/resized-image.ashx/__size/550x0/__key/CommunityServer.Components.UserFiles/00.00.00.36.97/Hebel-beschleunigter-Leerlauf-schief.jpg" border="0" /> I have different instructions: instead of going to ./CommunityServer.Components.UserFiles use ./CommunityServer.Components.ImageFileViewer/CommunityServer/Components/UserFiles which contain properly resized prerendered files. For example, /resized-image.ashx/__size/550x0/__key/CommunityServer.Components.UserFiles/00.00.00.36.97/Hebel-beschleunigter-Leerlauf-schief.jpg should be "./CommunityServer.Components.ImageFileViewer/CommunityServer/Components/UserFiles/00/00/00/36/97/Hebel beschleunigter Leerlauf parallel.jpg-550x0.jpg".

I need to export the sections 3, 4, 5 and 6 to disk. I want to do this in program.cs and you write the code for the following instructions (it will be a static Web site with html and image files only). Here is how I want the layout on disk:

* Every post and its replies go into a file of its own, the file is named "post{PostId}.html". Include all sensible detail like TotalViews, PostDate and category information.
* Every section gets a file of its own with all posts linked in date-descending order. It is a list with post title (linking to the post file), followed by author name, TotalViews and PostDate.
* Create an index.html that lists all sections that I wanted to export 
*/

using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

static class Program
{
    static string connectionString = "Server=.;Database=csmyhaflinger;Trusted_Connection=True;Trust Server Certificate=true";
    static string outputPath = @".\export";

    static void Main(string[] args)
    {
        int[] sectionsToExport = { 3, 4, 5, 6 };

        Directory.CreateDirectory(outputPath);

        var sections = GetSections(sectionsToExport);
        var allPosts = GetAllPostsForSections(sectionsToExport);

        // Group posts by thread (root post)
        var postsByThread = allPosts
            .Where(p => p.PostID == p.ParentID)
            .OrderByDescending(p => p.PostDate)
            .ToList();

        // Generate individual post files
        foreach (var rootPost in postsByThread)
        {
            GeneratePostFile(rootPost, allPosts, sections);
        }

        // Generate section index files
        foreach (var section in sections)
        {
            GenerateSectionFile(section, postsByThread, allPosts);
        }

        // Generate main index.html
        GenerateMainIndex(sections);

        Console.WriteLine("Export completed successfully!");
    }

    static List<Section> GetSections(int[] sectionIds)
    {
        var sections = new List<Section>();
        string query = @"
            SELECT SectionID, Name, Description, TotalPosts, TotalThreads
            FROM dbo.cs_Sections
            WHERE SectionID IN (" + string.Join(",", sectionIds) + ")";

        using (var conn = new SqlConnection(connectionString))
        using (var cmd = new SqlCommand(query, conn))
        {
            conn.Open();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    sections.Add(new Section
                    {
                        SectionID = reader.GetInt32(0),
                        Name = reader.IsDBNull(1) ? "" : reader.GetString(1),
                        Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
                        TotalPosts = reader.GetInt32(3),
                        TotalThreads = reader.GetInt32(4)
                    });
                }
            }
        }
        return sections;
    }

    static List<Post> GetAllPostsForSections(int[] sectionIds)
    {
        var posts = new List<Post>();
        string query = @"
            SELECT 
                p.PostID, p.ThreadID, p.ParentID, p.SectionID, p.UserID,
                p.Subject, p.Body, p.PostDate, p.TotalViews, p.PostAuthor,
                u.UserName
            FROM dbo.cs_Posts p
            LEFT JOIN dbo.cs_Users u ON p.UserID = u.UserID
            WHERE p.SectionID IN (" + string.Join(",", sectionIds) + @")
            ORDER BY p.ThreadID, p.PostLevel, p.SortOrder";

        using (var conn = new SqlConnection(connectionString))
        using (var cmd = new SqlCommand(query, conn))
        {
            conn.Open();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    posts.Add(new Post
                    {
                        PostID = reader.GetInt32(0),
                        ThreadID = reader.GetInt32(1),
                        ParentID = reader.GetInt32(2),
                        SectionID = reader.GetInt32(3),
                        UserID = reader.GetInt32(4),
                        Subject = reader.IsDBNull(5) ? "" : reader.GetString(5),
                        Body = reader.IsDBNull(6) ? "" : reader.GetString(6),
                        PostDate = reader.GetDateTime(7),
                        TotalViews = reader.GetInt32(8),
                        PostAuthor = reader.IsDBNull(9) ?
                            (reader.IsDBNull(10) ? "Anonymous" : reader.GetString(10)) :
                            reader.GetString(9),
                        UserName = reader.IsDBNull(10) ? "" : reader.GetString(10)
                    });
                }
            }
        }
        return posts;
    }

    static List<Category> GetCategoriesForPost(int postId)
    {
        var categories = new List<Category>();
        string query = @"
            SELECT c.CategoryID, c.Name
            FROM dbo.cs_Posts_InCategories pic
            INNER JOIN dbo.cs_Post_Categories c ON pic.CategoryID = c.CategoryID
            WHERE pic.PostID = @PostID";

        using (var conn = new SqlConnection(connectionString))
        using (var cmd = new SqlCommand(query, conn))
        {
            cmd.Parameters.AddWithValue("@PostID", postId);
            conn.Open();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    categories.Add(new Category
                    {
                        CategoryID = reader.GetInt32(0),
                        Name = reader.IsDBNull(1) ? "" : reader.GetString(1)
                    });
                }
            }
        }
        return categories;
    }

    static void GeneratePostFile(Post rootPost, List<Post> allPosts, List<Section> sections)
    {
        var replies = allPosts.Where(p => p.ThreadID == rootPost.ThreadID && p.PostID != rootPost.PostID)
                              .OrderBy(p => p.PostDate)
                              .ToList();

        var section = sections.FirstOrDefault(s => s.SectionID == rootPost.SectionID);
        var categories = GetCategoriesForPost(rootPost.PostID);

        var html = new StringBuilder();
        html.AppendLine("<!DOCTYPE html>");
        html.AppendLine("<html lang=\"en\">");
        html.AppendLine("<head>");
        html.AppendLine("    <meta charset=\"UTF-8\">");
        html.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
        html.AppendLine($"    <title>{rootPost.Subject}</title>");
        html.AppendLine("    <style>");
        html.AppendLine("        body { font-family: Arial, sans-serif; max-width: 1200px; margin: 0 auto; padding: 20px; background: #f5f5f5; }");
        html.AppendLine("        .header { background: #fff; padding: 20px; margin-bottom: 20px; border-radius: 5px; box-shadow: 0 2px 5px rgba(0,0,0,0.1); }");
        html.AppendLine("        .breadcrumb { font-size: 14px; color: #666; margin-bottom: 10px; }");
        html.AppendLine("        .breadcrumb a { color: #0066cc; text-decoration: none; }");
        html.AppendLine("        .post { background: #fff; padding: 20px; margin-bottom: 15px; border-radius: 5px; box-shadow: 0 2px 5px rgba(0,0,0,0.1); }");
        html.AppendLine("        .post-header { border-bottom: 1px solid #eee; padding-bottom: 10px; margin-bottom: 15px; }");
        html.AppendLine("        .post-title { font-size: 24px; font-weight: bold; margin: 0 0 10px 0; }");
        html.AppendLine("        .post-meta { font-size: 14px; color: #666; }");
        html.AppendLine("        .post-body { line-height: 1.6; }");
        html.AppendLine("        .reply { border-left: 3px solid #0066cc; margin-left: 20px; }");
        html.AppendLine("        .category { display: inline-block; background: #e0e0e0; padding: 3px 8px; border-radius: 3px; font-size: 12px; margin-right: 5px; }");
        html.AppendLine("        .stats { font-size: 14px; color: #666; margin-top: 10px; }");
        html.AppendLine("    </style>");
        html.AppendLine("</head>");
        html.AppendLine("<body>");

        // Header with breadcrumb
        html.AppendLine("    <div class=\"header\">");
        html.AppendLine("        <div class=\"breadcrumb\">");
        html.AppendLine($"            <a href=\"index.html\">Home</a> &raquo; ");
        html.AppendLine($"            <a href=\"section{rootPost.SectionID}.html\">{EscapeHtml(section?.Name ?? "Section")}</a> &raquo; ");
        html.AppendLine($"            {rootPost.Subject}");
        html.AppendLine("        </div>");
        html.AppendLine("    </div>");

        // Root post
        html.AppendLine("    <div class=\"post\">");
        html.AppendLine("        <div class=\"post-header\">");
        html.AppendLine($"            <h1 class=\"post-title\">{rootPost.Subject}</h1>");
        html.AppendLine("            <div class=\"post-meta\">");
        html.AppendLine($"                Posted by <strong>{EscapeHtml(rootPost.PostAuthor)}</strong> on {rootPost.PostDate:yyyy-MM-dd HH:mm}");
        html.AppendLine("            </div>");
        if (categories.Any())
        {
            html.AppendLine("            <div style=\"margin-top: 10px;\">");
            foreach (var cat in categories)
            {
                html.AppendLine($"                <span class=\"category\">{EscapeHtml(cat.Name)}</span>");
            }
            html.AppendLine("            </div>");
        }
        html.AppendLine("            <div class=\"stats\">");
        html.AppendLine($"                Views: {rootPost.TotalViews} | Replies: {replies.Count}");
        html.AppendLine("            </div>");
        html.AppendLine("        </div>");
        html.AppendLine("        <div class=\"post-body\">");
        html.AppendLine($"            {ProcessBody(rootPost.Body)}");
        html.AppendLine("        </div>");
        html.AppendLine("    </div>");

        // Replies
        foreach (var reply in replies)
        {
            html.AppendLine("    <div class=\"post reply\">");
            html.AppendLine("        <div class=\"post-header\">");
            html.AppendLine($"            <strong>{EscapeHtml(reply.PostAuthor)}</strong> replied on {reply.PostDate:yyyy-MM-dd HH:mm}");
            html.AppendLine("        </div>");
            html.AppendLine("        <div class=\"post-body\">");
            html.AppendLine($"            {ProcessBody(reply.Body)}");
            html.AppendLine("        </div>");
            html.AppendLine("    </div>");
        }

        html.AppendLine("</body>");
        html.AppendLine("</html>");

        File.WriteAllText(Path.Combine(outputPath, $"post{rootPost.PostID}.html"), html.ToString());
    }

    static void GenerateSectionFile(Section section, List<Post> rootPosts, List<Post> allPosts)
    {
        var sectionPosts = rootPosts.Where(p => p.SectionID == section.SectionID)
                                    .OrderByDescending(p => p.PostDate)
                                    .ToList();

        var html = new StringBuilder();
        html.AppendLine("<!DOCTYPE html>");
        html.AppendLine("<html lang=\"en\">");
        html.AppendLine("<head>");
        html.AppendLine("    <meta charset=\"UTF-8\">");
        html.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
        html.AppendLine($"    <title>{EscapeHtml(section.Name)}</title>");
        html.AppendLine("    <style>");
        html.AppendLine("        body { font-family: Arial, sans-serif; max-width: 1200px; margin: 0 auto; padding: 20px; background: #f5f5f5; }");
        html.AppendLine("        .header { background: #fff; padding: 20px; margin-bottom: 20px; border-radius: 5px; box-shadow: 0 2px 5px rgba(0,0,0,0.1); }");
        html.AppendLine("        .breadcrumb { font-size: 14px; color: #666; margin-bottom: 10px; }");
        html.AppendLine("        .breadcrumb a { color: #0066cc; text-decoration: none; }");
        html.AppendLine("        .post-list { background: #fff; padding: 20px; border-radius: 5px; box-shadow: 0 2px 5px rgba(0,0,0,0.1); }");
        html.AppendLine("        .post-item { padding: 15px; border-bottom: 1px solid #eee; }");
        html.AppendLine("        .post-item:last-child { border-bottom: none; }");
        html.AppendLine("        .post-item a { color: #0066cc; text-decoration: none; font-size: 18px; font-weight: bold; }");
        html.AppendLine("        .post-item a:hover { text-decoration: underline; }");
        html.AppendLine("        .post-meta { font-size: 14px; color: #666; margin-top: 5px; }");
        html.AppendLine("    </style>");
        html.AppendLine("</head>");
        html.AppendLine("<body>");

        html.AppendLine("    <div class=\"header\">");
        html.AppendLine("        <div class=\"breadcrumb\">");
        html.AppendLine("            <a href=\"index.html\">Home</a> &raquo; ");
        html.AppendLine($"            {EscapeHtml(section.Name)}");
        html.AppendLine("        </div>");
        html.AppendLine($"        <h1>{EscapeHtml(section.Name)}</h1>");
        if (!string.IsNullOrEmpty(section.Description))
        {
            html.AppendLine($"        <p>{EscapeHtml(section.Description)}</p>");
        }
        html.AppendLine($"        <p style=\"color: #666;\">Total threads: {section.TotalThreads} | Total posts: {section.TotalPosts}</p>");
        html.AppendLine("    </div>");

        html.AppendLine("    <div class=\"post-list\">");
        foreach (var post in sectionPosts)
        {
            var replyCount = allPosts.Count(p => p.ThreadID == post.ThreadID && p.PostID != post.PostID);
            html.AppendLine("        <div class=\"post-item\">");
            html.AppendLine($"            <a href=\"post{post.PostID}.html\">{post.Subject}</a>");
            html.AppendLine("            <div class=\"post-meta\">");
            html.AppendLine($"                By {EscapeHtml(post.PostAuthor)} | {post.PostDate:yyyy-MM-dd HH:mm} | Views: {post.TotalViews} | Replies: {replyCount}");
            html.AppendLine("            </div>");
            html.AppendLine("        </div>");
        }
        html.AppendLine("    </div>");

        html.AppendLine("</body>");
        html.AppendLine("</html>");

        File.WriteAllText(Path.Combine(outputPath, $"section{section.SectionID}.html"), html.ToString());
    }

    static void GenerateMainIndex(List<Section> sections)
    {
        var html = new StringBuilder();
        html.AppendLine("<!DOCTYPE html>");
        html.AppendLine("<html lang=\"en\">");
        html.AppendLine("<head>");
        html.AppendLine("    <meta charset=\"UTF-8\">");
        html.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
        html.AppendLine("    <title>Forum Export</title>");
        html.AppendLine("    <style>");
        html.AppendLine("        body { font-family: Arial, sans-serif; max-width: 1200px; margin: 0 auto; padding: 20px; background: #f5f5f5; }");
        html.AppendLine("        .header { background: #fff; padding: 20px; margin-bottom: 20px; border-radius: 5px; box-shadow: 0 2px 5px rgba(0,0,0,0.1); text-align: center; }");
        html.AppendLine("        .section-list { background: #fff; padding: 20px; border-radius: 5px; box-shadow: 0 2px 5px rgba(0,0,0,0.1); }");
        html.AppendLine("        .section-item { padding: 20px; border-bottom: 1px solid #eee; }");
        html.AppendLine("        .section-item:last-child { border-bottom: none; }");
        html.AppendLine("        .section-item a { color: #0066cc; text-decoration: none; font-size: 22px; font-weight: bold; }");
        html.AppendLine("        .section-item a:hover { text-decoration: underline; }");
        html.AppendLine("        .section-desc { color: #666; margin-top: 10px; }");
        html.AppendLine("        .section-stats { font-size: 14px; color: #888; margin-top: 5px; }");
        html.AppendLine("    </style>");
        html.AppendLine("</head>");
        html.AppendLine("<body>");

        html.AppendLine("    <div class=\"header\">");
        html.AppendLine("        <h1>Forum Archive</h1>");
        html.AppendLine("        <p>Exported from CommunityServer</p>");
        html.AppendLine("    </div>");

        html.AppendLine("    <div class=\"section-list\">");
        html.AppendLine("        <h2>Forum Sections</h2>");
        foreach (var section in sections.OrderBy(s => s.SectionID))
        {
            html.AppendLine("        <div class=\"section-item\">");
            html.AppendLine($"            <a href=\"section{section.SectionID}.html\">{EscapeHtml(section.Name)}</a>");
            if (!string.IsNullOrEmpty(section.Description))
            {
                html.AppendLine($"            <div class=\"section-desc\">{EscapeHtml(section.Description)}</div>");
            }
            html.AppendLine("            <div class=\"section-stats\">");
            html.AppendLine($"                {section.TotalThreads} threads | {section.TotalPosts} posts");
            html.AppendLine("            </div>");
            html.AppendLine("        </div>");
        }
        html.AppendLine("    </div>");

        html.AppendLine("</body>");
        html.AppendLine("</html>");

        File.WriteAllText(Path.Combine(outputPath, "index.html"), html.ToString());
    }

    static string ProcessBody(string body)
    {
        if (string.IsNullOrEmpty(body))
            return "";

        // Convert /cfs-file.ashx links
        body = Regex.Replace(body,
            @"/cfs-file\.ashx/__key/(CommunityServer\.Components\.[^/]+)/(\d+)\.(\d+)\.(\d+)\.(\d+)\.(\d+)/([^""'\s>]+)",
            match =>
            {
                var component = match.Groups[1].Value;
                var path = $"./{component}/{match.Groups[2].Value}/{match.Groups[3].Value}/{match.Groups[4].Value}/{match.Groups[5].Value}/{match.Groups[6].Value}/{match.Groups[7].Value}";
                return path;
            });

        // Convert /resized-image.ashx links
        body = Regex.Replace(body,
            @"/resized-image\.ashx/__size/(\d+)x(\d+)/__key/(CommunityServer\.Components\.[^/]+)/(\d+)\.(\d+)\.(\d+)\.(\d+)\.(\d+)/([^""'\s>]+)",
            match =>
            {
                var width = match.Groups[1].Value;
                var height = match.Groups[2].Value;
                var component = match.Groups[3].Value;
                var filename = match.Groups[9].Value;
                var path = $"./CommunityServer.Components.ImageFileViewer/CommunityServer/Components/{component.Replace("CommunityServer.Components.", "")}/{match.Groups[4].Value}/{match.Groups[5].Value}/{match.Groups[6].Value}/{match.Groups[7].Value}/{match.Groups[8].Value}/{filename}-{width}x{height}.jpg";
                return path;
            });

        return body;
    }

    static string EscapeHtml(string text)
    {
        if (string.IsNullOrEmpty(text))
            return "";
        return System.Net.WebUtility.HtmlEncode(text);
    }
}

class Section
{
    public int SectionID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int TotalPosts { get; set; }
    public int TotalThreads { get; set; }
}

class Post
{
    public int PostID { get; set; }
    public int ThreadID { get; set; }
    public int ParentID { get; set; }
    public int SectionID { get; set; }
    public int UserID { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public DateTime PostDate { get; set; }
    public int TotalViews { get; set; }
    public string PostAuthor { get; set; }
    public string UserName { get; set; }
}

class Category
{
    public int CategoryID { get; set; }
    public string Name { get; set; }
}