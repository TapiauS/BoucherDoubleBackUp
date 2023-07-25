using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel;
using System.Collections.Generic;
using static Microsoft.Maui.ApplicationModel.Permissions;

partial class BluetoothPermissions : BasePlatformPermission
    {
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions=>new List<(string permission,bool isRuntime)>()
        {
            ("android.permission.BLUETOOTH_SCAN",true),
            ("android.permission.BLUETOOTH_CONNECT",true)
        }.ToArray();
        
    }

