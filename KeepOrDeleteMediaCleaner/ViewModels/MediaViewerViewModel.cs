using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KeepOrDeleteMediaCleaner.Models;
using KeepOrDeleteMediaCleaner.Services;
using Microsoft.Maui.Controls.PlatformConfiguration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
#if ANDROID
using Android.Content;
using Android.Net;
using Android.App;
using KeepOrDeleteMediaCleaner.Platforms.Android.Services;
#endif

namespace KeepOrDeleteMediaCleaner.ViewModels
{
    /// <summary>
    /// ViewModel that exposes image browsing and simple keep/delete workflows for the UI.
    /// </summary>
    /// <remarks>
    /// - Uses <see cref="IImageScannerService"/> to load image metadata.
    /// - Maintains an <see cref="ObservableCollection{MediaFile}"/> for data binding to the view.
    /// - Exposes commands to keep, delete and confirm deletion of the currently shown image.
    /// - Platform-specific delete behavior is implemented under <c>#if ANDROID</c>.
    /// </remarks>
    public partial class ImageViewerViewModel : ObservableObject
    {
        /// <summary>
        /// Service used to enumerate images on the device.
        /// </summary>
        private readonly IImageScannerService _mediaScannerService;

        /// <summary>
        /// Collection of media files exposed to the view for binding.
        /// </summary>
        public ObservableCollection<MediaFile> MediaFiles { get; } = new();

        private List<string> listUri = new List<string>();

        /// <summary>
        /// Command to mark the current image as kept (advances to the next image).
        /// </summary>
        public IRelayCommand<MediaFile> KeepCommand { get; }

        /// <summary>
        /// Command to mark the current image for deletion (removes from collection and queues Uri).
        /// </summary>
        public IRelayCommand<MediaFile> DeleteCommand { get; }

        /// <summary>
        /// Command to request confirmation and perform deletion of queued items.
        /// </summary>
        public IRelayCommand<MediaFile> ConfirmDeleteCommand { get; }

        private int _currentIndex;

        /// <summary>
        /// Human readable text representing current position, formatted as "current/total".
        /// </summary>
        public string CurrentIndexText => $"{_currentIndex + 1}/{MediaFiles.Count}";

        /// <summary>
        /// Index of the currently displayed item in <see cref="MediaFiles"/>.
        /// Setting this property updates <see cref="CurrentIndexText"/> notifications.
        /// </summary>
        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                _currentIndex = value;
                OnPropertyChanged(nameof(CurrentIndex));
                OnPropertyChanged(nameof(CurrentIndexText));
            }
        }

#if ANDROID
        private string? _pendingDeleteUri;
#endif


        /// <summary>
        /// Creates a new instance of <see cref="ImageViewerViewModel"/>.
        /// </summary>
        /// <param name="scannerService">Implementation of <see cref="IImageScannerService"/> used to load images.</param>
        public ImageViewerViewModel(IImageScannerService scannerService)
        {
            _mediaScannerService = scannerService;
            KeepCommand = new RelayCommand<MediaFile>(Keep);
            DeleteCommand = new RelayCommand<MediaFile>(Delete);
            ConfirmDeleteCommand = new RelayCommand<MediaFile>(ConfirmDelete);
        }

        /// <summary>
        /// Requests platform-specific deletion for all queued URIs.
        /// </summary>
        /// <param name="file">Unused parameter; command signature compatibility.</param>
        /// <remarks>
        /// On Android this delegates to <see cref="AndroidMediaDeleteService.RequestDeleteList(List{string}, Action{bool})"/>.
        /// The UI should handle post-delete updates; this method only triggers the platform deletion flow.
        /// </remarks>
        private void ConfirmDelete(MediaFile? file)
        {
#if ANDROID
            if(!listUri.Any()) return;
            AndroidMediaDeleteService.RequestDeleteList(listUri, success => {
                if(success){
                    //MainThread.BeginInvokeOnMainThread(RemoveFromCollection);
                }
            });
#endif
        }

        /// <summary>
        /// Keeps the provided file (no deletion) and advances to the next image.
        /// </summary>
        /// <param name="file">The <see cref="MediaFile"/> to keep. The method ignores the parameter and advances the view.</param>
        public void Keep(MediaFile file)
        {
            MoveToNext();
        }

        /// <summary>
        /// Marks the current media file for deletion by queuing its Uri and removing it from the collection.
        /// </summary>
        /// <param name="file">The <see cref="MediaFile"/> to delete. The method uses the current index to determine which item to queue.</param>
        public void Delete(MediaFile file)
        {
            if (MediaFiles.Count == 0) return;

            var itemToRemove = MediaFiles[CurrentIndex];
            listUri.Add(itemToRemove.Uri);

            RemoveFromCollection();
        }

        /// <summary>
        /// Advances the <see cref="CurrentIndex"/> to the next item if possible and notifies bound properties.
        /// </summary>
        private void MoveToNext()
        {
            if (CurrentIndex < MediaFiles.Count - 1)
            {
                CurrentIndex++;
            }
            OnPropertyChanged(nameof(CurrentIndexText));
        }

        /// <summary>
        /// Removes the item at <see cref="CurrentIndex"/> from <see cref="MediaFiles"/> and adjusts the index if needed.
        /// </summary>
        private void RemoveFromCollection()
        {
            if (MediaFiles.Count == 0 || CurrentIndex < 0)
                return;

            MediaFiles.RemoveAt(CurrentIndex);

            if (CurrentIndex >= MediaFiles.Count)
                CurrentIndex = MediaFiles.Count - 1;

            OnPropertyChanged(nameof(CurrentIndexText));
        }

        /// <summary>
        /// Loads images from the underlying scanner service into <see cref="MediaFiles"/>.
        /// </summary>
        /// <returns>A task that completes when the collection is populated.</returns>
        /// <remarks>
        /// Images are ordered by their <see cref="MediaFile.DateCreated"/> before being added to the collection.
        /// After loading, <see cref="CurrentIndex"/> is initialized to zero.
        /// </remarks>
        public async Task InitializeAsync()
        {
            var images = await _mediaScannerService.GetImagesAsync();

            MediaFiles.Clear();
            foreach (var img in images.OrderBy(x => x.DateCreated))
            {
                MediaFiles.Add(img);
            }

            CurrentIndex = 0;
        }

    }
}
