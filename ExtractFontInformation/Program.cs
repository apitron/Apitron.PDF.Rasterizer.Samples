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
			using (Document document = new Document(fs)) // this object represents a PDF document
            {
                // enumerate fonts used in document
                foreach (Font font in document.Fonts)
                {
                    // print out the font name, its type and state
                    Console.WriteLine($"Font name: { font.Name }");
                    Console.WriteLine($"Font type: { Enum.GetName(typeof(FontType), font.Type) }");
                    Console.WriteLine($"Font state:{ Enum.GetName(typeof(FontState), font.State) }");
                }

                Console.ReadLine();
            }
        }
    }
}
