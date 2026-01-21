using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeepOrDeleteMediaCleaner.Models
{
    public class MediaFile
    {
        public string Id { get; set; }
        public string FilePath { get; set; }
        public string DisplayName { get; set; }
        public DateTime DateCreated { get; set; }
        public long Size { get; set; }
        public bool IsVideo { get; set; }
    }
}
