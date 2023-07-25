using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Bluetooth;
using Boucher_Double_Front.Services;
using global::Android.Bluetooth;
using Java.Util;
using Boucher_Double_Front.Platforms.Android.Services;

[assembly: Dependency(typeof(BluetoothServiceRenderer))]
namespace Boucher_Double_Front.Platforms.Android.Services
{

    class BluetoothServiceRenderer : IPrintService
    {
        public async Task<IList<string>> GetDeviceList()
        {
            try
            {
                using (BluetoothAdapter bluetoothAdapter = BluetoothAdapter.DefaultAdapter)
                {
                    if (await CheckBluetoothStatus())
                    {
                        var btdevice = bluetoothAdapter?.BondedDevices.Select(i => i.Name).ToList();
                        return btdevice;
                    }
                    else
                        return default;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                return default;
            }
        }

        private async Task<bool> CheckBluetoothStatus()
        {
            try
            {
                if (DeviceInfo.Version.Major < 12)
                {
                    var status = PermissionStatus.Unknown;
                    status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                    if (status == PermissionStatus.Granted)
                    {
                        return true;
                    }
                    if (Permissions.ShouldShowRationale<Permissions.LocationWhenInUse>())
                    {
                        await Shell.Current.DisplayAlert("Permission d'accéder au bluetooth", "Permission nécessaire pour imprimer la facture", "Ok");
                    }

                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                    return status == PermissionStatus.Granted;
                }
                else
                {
                    var status = PermissionStatus.Unknown;
                    status = await Permissions.CheckStatusAsync<BluetoothPermissions>();
                    if (status == PermissionStatus.Granted)
                    {
                        return true;
                    }
                    if (Permissions.ShouldShowRationale<BluetoothPermissions>())
                    {
                        await Shell.Current.DisplayAlert("Permission d'accéder au bluetooth", "Permission nécessaire pour imprimer la facture", "Ok");
                    }

                    status = await Permissions.RequestAsync<BluetoothPermissions>();
                    return status == PermissionStatus.Granted;
                }
            }
            catch (Exception ex)
            {
                // logger.LogError(ex);
                return false;
            }
        }

        public async Task Print(string deviceName, string text)
        {
            using (BluetoothAdapter bluetoothAdapter = BluetoothAdapter.DefaultAdapter)
            {
                BluetoothDevice device = (from bd in bluetoothAdapter?.BondedDevices
                                            where bd?.Name == deviceName
                                            select bd).FirstOrDefault();
                try
                {
                    using (BluetoothSocket bluetoothSocket = device?.
                        CreateRfcommSocketToServiceRecord(
                        UUID.FromString("00001101-0000-1000-8000-00805f9b34fb")))
                    {
                        bluetoothSocket?.Connect();
                        byte[] buffer = Encoding.UTF8.GetBytes(text);
                        bluetoothSocket?.OutputStream.Write(buffer, 0, buffer.Length);
                        bluetoothSocket.Close();
                    }
                }
                catch (Exception exp)
                {
                    throw exp;
                }
            }
        }
    }
}

