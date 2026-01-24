
using KeepOrDeleteMediaCleaner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeepOrDeleteMediaCleaner.Services
{
    public interface IImageScannerService
    {
        Task<List<MediaFile>> GetImagesAsync();
    }
}
