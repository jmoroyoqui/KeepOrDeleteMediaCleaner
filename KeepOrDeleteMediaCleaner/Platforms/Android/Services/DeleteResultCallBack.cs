using Android.App; // Add this using directive for Result.Ok
using AndroidX.Activity.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeepOrDeleteMediaCleaner.Platforms.Android.Services
{
    public class DeleteResultCallBack : Java.Lang.Object, IActivityResultCallback
    {
        private readonly Action<bool> _onResult;

        public DeleteResultCallBack(Action<bool> onResult)
        {
            _onResult = onResult;
        }
        public void OnActivityResult(Java.Lang.Object? result)
        {
            if(result is ActivityResult activityResult)
            {
                // Use Result.Ok from Android.App instead of AndroidX.Activity.Result.Ok
                bool success = activityResult.ResultCode == (int)Result.Ok;
                _onResult.Invoke(success);
            }
        }
    }
}
