using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;
using Org.BouncyCastle.Asn1.Mozilla;
namespace EMRMS.Utilities.Videos
{
    class VideoThumbnailGen
    {
        public static void GenerateThumbnail(string videoPath, string outputPath)
        {
            var inputFile = new MediaFile { Filename = videoPath };
            var outputFile = new MediaFile { Filename = outputPath };

            using (var engine = new Engine())
            {
                engine.GetMetadata(inputFile);
                TimeSpan duration = inputFile.Metadata.Duration;
                var options = new ConversionOptions { Seek = TimeSpan.FromMilliseconds(duration.TotalMilliseconds / 2) };
                engine.GetThumbnail(inputFile, outputFile, options);
            }
        }
    }
}
