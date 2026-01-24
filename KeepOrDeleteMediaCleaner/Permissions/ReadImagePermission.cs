using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace KeepOrDeleteMediaCleaner.Permissions
{
    public class ReadImagePermission : Microsoft.Maui.ApplicationModel.Permissions.BasePlatformPermission
    {
#if ANDROID
    public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
        new[]
        {
            (Android.Manifest.Permission.ReadMediaImages, true)
        };
#endif
    }
}
