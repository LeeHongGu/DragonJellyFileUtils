using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonJellyFileUtils
{
    public class ProgressReport
    {
        public long TotalBytes { get; set; }
        public long BytesProcessed { get; set; }
        public double Percentage => (double)BytesProcessed / TotalBytes * 100;
    }
}
