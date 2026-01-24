using Android.App;
using Android.Provider;
using AndroidX.Activity;
using AndroidX.Activity.Result;
using AndroidX.Activity.Result.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeepOrDeleteMediaCleaner.Platforms.Android.Services
{
    public static class AndroidMediaDeleteService
    {
        private static ActivityResultLauncher? _launcher;
        private static Action<bool>? _callback;
        private static Action<bool>? _pendingCallback;
        public static void Init(ComponentActivity activity)
        {
            _launcher = activity.RegisterForActivityResult(
                new ActivityResultContracts.StartIntentSenderForResult(), 
                new DeleteResultCallBack(success =>
                {
                    _pendingCallback?.Invoke(success);
                }));
        }
        public static void RequestDelete(string contentUri, Action<bool> onCompleted)
        {
            var uri = global::Android.Net.Uri.Parse(contentUri);

            var intent = MediaStore.CreateDeleteRequest(Platform.CurrentActivity.ContentResolver, new List<global::Android.Net.Uri> { uri });

            var request = new IntentSenderRequest.Builder(intent.IntentSender).Build();

            _launcher?.Launch(request);
        }

        public static void RequestDeleteList(List<string> contentUriList, Action<bool> onCompleted)
        {
            var uriList = contentUriList.Select(uriString => global::Android.Net.Uri.Parse(uriString)).ToList();
            var intent = MediaStore.CreateDeleteRequest(Platform.CurrentActivity.ContentResolver, uriList);
            var request = new IntentSenderRequest.Builder(intent.IntentSender).Build();
            _pendingCallback = onCompleted;
            _launcher?.Launch(request);
        }
    }
}
