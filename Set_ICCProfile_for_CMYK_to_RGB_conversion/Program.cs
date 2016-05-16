using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apitron.PDF.Rasterizer;
using Apitron.PDF.Rasterizer.Configuration;
using System.Drawing;

namespace Set_ICCProfile_for_CMYK_to_RGB_conversion
{
    /// <summary>
    /// This program demonstrates how to use custom CMYK profile for
    /// CMYK to RGB transformation.
    /// IMPORTANT:Don't forget to restore NUGET packages before compilation.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            string outputFileName = "output.png";

            using (Stream stream = File.OpenRead("../../data/cmyk.pdf"), 
                profileStream = File.OpenRead("../../data/profile.icc"))
            {
                // set global CMYK profile to be used for CMYK -> RGB conversion,
                // it's also possible to pass engine settings as parameter 
                // during Document object creation.
                EngineSettings.GlobalSettings.CMYKColorProfile = new IccColorProfile(profileStream);

                using (Document doc = new Document(stream))
                {
                    Bitmap result = doc.Pages[0].Render(new Resolution(96, 96), new RenderingSettings());
                    result.Save(outputFileName);
                }
            }

            Process.Start(outputFileName);
        }
    }
}
