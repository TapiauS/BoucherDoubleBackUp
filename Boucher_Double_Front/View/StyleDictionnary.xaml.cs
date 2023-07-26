namespace Boucher_Double_Front.View;

public partial class StyleDictionnary : ResourceDictionary
{
	public static StyleDictionnary Instance { get; set; }
	private StyleDictionnary()
	{
		AppShell shell = Shell.Current as AppShell;
		InitializeComponent();
	}

	public static StyleDictionnary GetInstance()
	{
		if(Instance == null)
		{
			Instance = new StyleDictionnary();
		}
		return Instance;
	}
}