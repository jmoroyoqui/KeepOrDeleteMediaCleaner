using Android.Content;
using Android.Provider;
using KeepOrDeleteMediaCleaner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeepOrDeleteMediaCleaner.Services
{
    public class MediaScannerService
    {

        public List<MediaFile> GetMediaFiles(Context context)
        {
            var mediaFiles = new List<MediaFile>();

            var projection = GetProjection();

            var selection = $"{MediaStore.Files.FileColumns.MediaType}=? OR {MediaStore.Files.FileColumns.MediaType}=?";

            var selectionArgs = GetSelectionArgs();

            var sortOrder = MediaStore.MediaColumns.DateAdded + " ASC";


        }

        private string[]? GetProjection()
        {
            return new[]
            {
                MediaStore.MediaColumns.Id,
                MediaStore.MediaColumns.DisplayName,
                MediaStore.MediaColumns.DateAdded,
                MediaStore.MediaColumns.Size,
                MediaStore.MediaColumns.Data,
                MediaStore.Files.FileColumns.MediaType
            }; 
        }

        private string[]? GetSelectionArgs()
        {
            return new[]
            {
                ((int)MediaType.Image).ToString(),
                ((int)MediaType.Video).ToString()
            };
        }

        private List<MediaFile> GetMediaFilesFromCursor(Context context, string[]? projection, string selection, string[]? selectionArgs, string sortOrder)
        {
            using var cursor = context.ContentResolver.Query(
                MediaStore.Files.GetContentUri("external"),
                projection,
                selection,
                selectionArgs,
                sortOrder
                );
        }
    }
}
