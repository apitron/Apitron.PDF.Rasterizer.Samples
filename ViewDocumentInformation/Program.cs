using System;

namespace ViewDocumentInformation
{
    using System.IO;
    using Apitron.PDF.Rasterizer;

    internal class Program
    {
        private static void Main(string[] args)
        {
            // open and load the file
            using (FileStream fs = new FileStream(@"..\..\..\Documents\testfile.pdf", FileMode.Open))
            {
                // this objects represents a PDF document
                Document document = new Document(fs);

                // get the object that holds information about document
                DocumentInfo info = document.DocumentInfo;

                // print out desired fields
                Console.WriteLine($"Document title:{info.Title}");
                Console.WriteLine($"Page count:{document.Pages.Count}");
                Console.WriteLine($"Document author:{info.Author}");
                Console.WriteLine($"Created with:{info.Producer}");

                // dates by default are returned as they are written in PDF file,
                // to give you the full control over stored information.
                // helper method used below, converts PDF date representation to DateTime object.
                // you may use it, if you don't need special processing
                Console.WriteLine($"Creation date:{DocumentInfo.ParsePdfDate(info.CreationDate)}");

                // read further if needed..

                Console.ReadLine();
            }
        }
    }
}
