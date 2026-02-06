#if ANDROID
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeepOrDeleteMediaCleaner.Platforms.Android.Services
{
    public class ReadVideoPermissions : Microsoft.Maui.ApplicationModel.Permissions.BasePlatformPermission
    {
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
            new[]
            {
                ("android.permission.READ_MEDIA_VIDEO", true)   
            };
    }
}
#endif
