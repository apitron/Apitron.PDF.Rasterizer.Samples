namespace ExtractFontInformation
{
    using System;
    using System.IO;
    using Apitron.PDF.Rasterizer;
    using Apitron.PDF.Rasterizer.Fonts;

    internal class Program
    {
        private static void Main(string[] args)
        {
            // open and load the file
            using (FileStream fs = new FileStream(@"..\..\..\Documents\testfile.pdf", FileMode.Open))
            {
                // this objects represents a PDF document
                Document document = new Document(fs);

                // enumerate fonts used in document
                foreach (Font font in document.Fonts)
                {
                    // print out the font name, its type and state
                    Console.WriteLine(string.Format("Font name: {0}", font.Name));
                    Console.WriteLine(string.Format("Font type: {0}", Enum.GetName(typeof(FontType), font.Type)));
                    Console.WriteLine(string.Format("Font state: {0}", Enum.GetName(typeof(FontState), font.State)));
                }

                Console.ReadLine();
            }
        }
    }
}
