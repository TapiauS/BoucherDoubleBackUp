using Android.App;
using Android.Runtime;
using Boucher_Double_Front.Platforms.Android.Services;
using Boucher_Double_Front.Services;

namespace Boucher_Double_Front;

[Application]
public class MainApplication : MauiApplication
{
	public MainApplication(IntPtr handle, JniHandleOwnership ownership)
		: base(handle, ownership)
	{
        DependencyService.Register<IPrintService, BluetoothServiceRenderer>();
    }

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
