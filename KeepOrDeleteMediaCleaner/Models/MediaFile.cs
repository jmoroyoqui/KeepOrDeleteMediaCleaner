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
        public string Uri { get; set; }
        public string DisplayName { get; set; }
        public DateTime DateCreated { get; set; }
        public double Size { get; set; }
        public string SizeInMBString { get;set;}
        public bool IsVideo { get; set; }
    }
}
