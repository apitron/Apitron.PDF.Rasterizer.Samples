using Apitron.PDF.Rasterizer;
using Apitron.PDF.Rasterizer.Configuration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace SetCustomFontSubstitution
{
    class Program
    {
        static void Main(string[] args)
        {
            using (FileStream fs = new FileStream(@"..\..\..\Documents\LinksUsageSample.pdf", FileMode.Open))
            {
                using (Document doc = new Document(fs))
                {
                    // If system does not have Mincho font - it will be substituted by Arial
                    // map the font to all not found fonts using *, it's possible to map particular fonts
                    // using their names
                    EngineSettings.UserFontMappings.Add(new KeyValuePair<string, string[]>("Arial", new[] { "*" }));

                    foreach (var font in doc.Fonts)
                    {
                        if (font.Name.Contains("Mincho"))
                        {
                            Console.WriteLine(font.Name);
                            Console.WriteLine(font.State);
                            Console.WriteLine(font.ActualFontType);
                            Console.WriteLine(font.ActualFontName);
                        }
                    }
                    RenderingSettings settings = new RenderingSettings();
                    settings.RenderMode = RenderMode.HighQuality;
                    for (int j = 0; j < doc.Pages.Count; j++)
                    {
                        Page page = doc.Pages[j];
                        Bitmap bm = page.Render(new Resolution(300, 300), settings);

                        if (bm != null)
                        {
                            bm.Save($"out{j}.png");
                            bm.Dispose();
                        }
                    }
                }
            }
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
