using IL2CPU.API.Attribs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexumOS.System
{
    public class FileLoader
    {
        [ManifestResourceStream(ResourceName = "NexumOS.ExternalFiles.Executables.test.bin")]
        static byte[] file;

        public void Load()
        {
            if(File.Exists(@"0:\\test.bin"))
                File.Delete(@"0:\\test.bin");
            File.WriteAllBytes(@"0:\\test.bin", file);
        }
    }
}
