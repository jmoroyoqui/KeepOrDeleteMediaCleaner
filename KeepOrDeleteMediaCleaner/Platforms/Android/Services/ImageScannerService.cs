#if ANDROID
using Android.Content;
using Android.Database;
using Android.Provider;
#endif
using KeepOrDeleteMediaCleaner.Models;
using KeepOrDeleteMediaCleaner.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeepOrDeleteMediaCleaner.Platforms.Android.Services
{
    /// <summary>
    /// Android implementation of <see cref="IImageScannerService"/> that queries the
    /// system <c>MediaStore</c> for image metadata and maps results to <see cref="MediaFile"/> instances.
    /// </summary>
    /// <remarks>
    /// This implementation is guarded by the <c>#if ANDROID</c> compilation symbol and should only be used
    /// on Android targets. Callers must ensure the application has the necessary permissions to read media
    /// (for example, legacy <c>READ_EXTERNAL_STORAGE</c> or the appropriate scoped storage / media permissions
    /// on newer Android releases). The service reads metadata only; it does not open image streams.
    /// </remarks>
    public class ImageScannerService : IImageScannerService
    {      

        /// <summary>
        /// Converts a size in bytes to megabytes.
        /// </summary>
        /// <param name="sizeInBytes">Size in bytes to convert.</param>
        /// <returns>Size expressed in megabytes using 1 MB = 1024 * 1024 bytes.</returns>
        private double SizeInMb(long sizeInBytes)
        {
            return sizeInBytes / (1024.0 * 1024.0);
        }

        /// <summary>
        /// Queries the Android <c>MediaStore.Images</c> collection and returns a list of image metadata.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{TResult}"/> that completes with a <see cref="List{MediaFile}"/> containing the discovered images.
        /// If the underlying query returns null, an empty list is returned.
        /// </returns>
        /// <remarks>
        /// - Results are ordered by the <c>DateAdded</c> field in ascending order.
        /// - The returned <see cref="MediaFile"/> objects populate <see cref="MediaFile.FilePath"/>,
        ///   <see cref="MediaFile.Uri"/>, <see cref="MediaFile.DateCreated"/>, <see cref="MediaFile.DisplayName"/>, 
        ///   and <see cref="MediaFile.Size"/> (and <see cref="MediaFile.SizeInMBString"/>).
        /// - <c>DateAdded</c> is interpreted as Unix time seconds and converted to a <c>DateTime</c>.
        /// - The method may throw if the app lacks permission to access the MediaStore; callers should handle
        ///   security-related exceptions as appropriate.
        /// </remarks>
        public async Task<List<MediaFile>> GetImagesAsync()
        {
            var images = new List<MediaFile>();

            var projection = new[]
            {
                MediaStore.Images.Media.InterfaceConsts.Id,
                MediaStore.Images.Media.InterfaceConsts.Data,
                MediaStore.Images.Media.InterfaceConsts.DateAdded,
                MediaStore.Images.Media.InterfaceConsts.DisplayName,
                MediaStore.Images.Media.InterfaceConsts.Size
            };

            using ICursor cursor = Platform.CurrentActivity.ContentResolver.Query(
                MediaStore.Images.Media.ExternalContentUri,
                projection,
                null,
                null,
                MediaStore.Images.Media.InterfaceConsts.DateAdded + " ASC");

            if (cursor == null)
                return images;

            int idColumn = cursor.GetColumnIndex(MediaStore.Images.Media.InterfaceConsts.Id);
            int dataColumn = cursor.GetColumnIndex(MediaStore.Images.Media.InterfaceConsts.Data);
            int dateColumn = cursor.GetColumnIndex(MediaStore.Images.Media.InterfaceConsts.DateAdded);
            int nameColumn = cursor.GetColumnIndex(MediaStore.Images.Media.InterfaceConsts.DisplayName);
            int sizeColumn = cursor.GetColumnIndex(MediaStore.Images.Media.InterfaceConsts.Size);

            while (cursor.MoveToNext())
            {
                long id = cursor.GetLong(idColumn);
                string path = cursor.GetString(dataColumn);
                long dateAdded = cursor.GetLong(dateColumn);
                string displayName = cursor.GetString(nameColumn);
                long size = cursor.GetLong(sizeColumn);


                var contentUri = ContentUris.WithAppendedId(
                    MediaStore.Images.Media.ExternalContentUri, id);

                images.Add(new MediaFile
                {
                    FilePath = path,
                    Uri = contentUri.ToString(),
                    DateCreated = DateTimeOffset
                        .FromUnixTimeSeconds(dateAdded)
                        .DateTime,
                    DisplayName = displayName,
                    Size = SizeInMb(size),
                    SizeInMBString = $"{SizeInMb(size):F2} MB"
                });
            }

            return images;
        }
    }
}
