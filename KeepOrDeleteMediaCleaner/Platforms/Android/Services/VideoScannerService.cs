using Android.Database;
using Android.Provider;
using KeepOrDeleteMediaCleaner.Models;
using KeepOrDeleteMediaCleaner.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeepOrDeleteMediaCleaner.Platforms.Android.Services
{
    public class VideoScannerService : IVideoScannerService
    {
        public Task<List<MediaFile>> GetVideosAsync()
        {
            var videos = new List<MediaFile>();

            var projection = GetProjection();

            var cursor = GetCursor(projection);

            if(cursor == null)
            {
                return Task.FromResult(videos);
            }

            int idCol = cursor.GetColumnIndexOrThrow(MediaStore.Video.Media.InterfaceConsts.Id);
            int nameCol = cursor.GetColumnIndexOrThrow(MediaStore.Video.Media.InterfaceConsts.DisplayName);
            int dataCol = cursor.GetColumnIndexOrThrow(MediaStore.Video.Media.InterfaceConsts.Data);
            int dateCol = cursor.GetColumnIndexOrThrow(MediaStore.Video.Media.InterfaceConsts.DateAdded);
            int durationCol = cursor.GetColumnIndexOrThrow(MediaStore.Video.Media.InterfaceConsts.Duration);
            int sizeCol = cursor.GetColumnIndexOrThrow(MediaStore.Video.Media.InterfaceConsts.Size);

            while (cursor.MoveToNext())
            {
                var id = cursor.GetString(idCol);
                var name = cursor.GetString(nameCol);
                var data = cursor.GetString(dataCol);
                var dateAdded = cursor.GetLong(dateCol);
                var duration = cursor.GetLong(durationCol);
                var size = cursor.GetLong(sizeCol);
                var mediaFile = new MediaFile
                {
                    Id = id,
                    FilePath = data,
                    Uri = $"{MediaStore.Video.Media.ExternalContentUri}/{id}",
                    DisplayName = name,
                    DateCreated = DateTimeOffset.FromUnixTimeSeconds(dateAdded).DateTime,
                    Size = size,
                    SizeInMBString = $"{(size / (1024.0 * 1024.0)):0.##} MB",
                    IsVideo = true,
                    Duration = TimeSpan.FromMilliseconds(duration)
                };
                videos.Add(mediaFile);
            }

            cursor.Close();
            return Task.FromResult(videos);
        }

        private string[]? GetProjection()
        {
            //return new[]
            //{
            //    MediaStore.Video.Media.InterfaceConsts.Id,
            //    MediaStore.Video.Media.InterfaceConsts.Data,
            //    MediaStore.Video.Media.InterfaceConsts.DisplayName,
            //    MediaStore.Video.Media.InterfaceConsts.DateAdded,
            //    MediaStore.Video.Media.InterfaceConsts.Duration,
            //    MediaStore.Video.Media.InterfaceConsts.Size
            //};
            return new[]
            {
                MediaStore.Files.FileColumns.Id,
                MediaStore.Files.FileColumns.Data,
                MediaStore.Files.FileColumns.DisplayName,
                MediaStore.Files.FileColumns.DateAdded,
                MediaStore.Video.VideoColumns.Duration,
                MediaStore.Files.FileColumns.Size,

            };
        }

        private ICursor? GetCursor(string[] projection)
        {
            return Platform.CurrentActivity.ContentResolver.Query(
                MediaStore.Files.GetContentUri("external"),
                projection,
                MediaStore.Files.IFileColumns.MediaType + "=" + ((int)MediaType.Video),
                null,
                MediaStore.Video.Media.InterfaceConsts.DateAdded + " DESC");
        }
    }
}
