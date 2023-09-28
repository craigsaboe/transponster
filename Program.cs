using Markdig;
using RazorEngine;
using RazorEngine.Templating;
using System.Diagnostics;

class Program
{
    static void Main()
    {
        string sourceDir = "source";
        string outputDir = "output";        

        if (!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        string templatePath = "Template.cshtml";
        var templateContent = File.ReadAllText(templatePath);

        foreach (var postDir in Directory.GetDirectories(sourceDir))
        {
            // The assumption here is that the folder name matches the Markdown filename.
            // If it doesn't or there is no Markdown file, continue to the next directory.
            string postName = Path.GetFileName(postDir);
            string markdownFile = Path.Combine(postDir, postName + ".md");

            if (!File.Exists(markdownFile)) continue;
            
            // Generate post HTML.
            string markdownContent = File.ReadAllText(markdownFile);
            string htmlContent = Markdown.ToHtml(markdownContent);

            var model = new
            {
                Title = Path.GetFileNameWithoutExtension(markdownFile),
                Content = htmlContent
            };

            string outputPostDir = Path.Combine(outputDir, postName);
            if (!Directory.Exists(outputPostDir))
            {
                Directory.CreateDirectory(outputPostDir);
            }

            string result = Engine.Razor.RunCompile(templateContent, "templateKey", null, model);

            string outputFile = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(markdownFile) + ".html");
            Console.WriteLine("Created new post: " + markdownFile);
            File.WriteAllText(outputFile, result);

            string sourceImagesDir = Path.Combine(postDir, "images");
            string outputImagesDir = Path.Combine(outputPostDir, "images");

            if (Directory.Exists(sourceImagesDir))
            {
                if (!Directory.Exists(outputImagesDir))
                {
                    Directory.CreateDirectory(outputImagesDir);
                }

                foreach (var imageFile in Directory.GetFiles(sourceImagesDir))
                {
                    string destFile = Path.Combine(outputImagesDir, Path.GetFileName(imageFile));
                    File.Copy(imageFile, destFile, true);
                }
            }
        }

        Console.WriteLine("Site generation completed!");
    }
}