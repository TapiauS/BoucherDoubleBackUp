using Boucher_Double_Front.Database;
using Boucher_Double_Front.Model;
using System.Reflection;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration.TizenSpecific;

namespace Boucher_Double_Front.View;

public partial class StyleDictionnary : ResourceDictionary
{
	public static StyleDictionnary Instance { get; set; }

    private Color backgroundColor;
    public Color BackgroundColor
    {
        get => backgroundColor;
        set
        {
            backgroundColor = value;
            this["BackgroundColor"] = value;
        }
    }

    private Color shellColor;
    public Color ShellColor
    {
        get => shellColor;
        set
        {
            shellColor = value;
            this["ShellColor"] = value;
        }
    }

    private Color buttonColor;
    public Color ButtonColor 
    {
        get => buttonColor;
        set 
        {
            buttonColor = value;
            this["ButtonColor"] = value;
        }
    }

    private Color textColor;
    public Color TextColor
    {
        get => textColor;
        set
        {
            textColor = value;
/*            foreach(Style style in textStyles) 
            {
                List<Setter> setters= new List<Setter>(style.Setters);
                style.Setters.Clear();
                foreach(Setter setter in setters)
                {
                    setter.Value = value;
                    style.Setters.Add(setter);
                }
            }*/
            this["TextColor"] = value;
        }
    }
/*    private void DefineTextColor()
    {
        Assembly mauiControlsAssembly = typeof(Microsoft.Maui.Controls.View).Assembly;
        List<Type> types = mauiControlsAssembly
                        .GetTypes()
                        .Where(t => t.Namespace!=null&&t.Namespace.StartsWith("Microsoft.Maui.Controls") &&t.GetFields().Where(field=>field.Name=="TextColorProperty").Count()>0).ToList();
        foreach (Type type in types)
        {
            if (type != typeof(Button))
            {
                Style style = new(type);
                Setter setter = new() { Property = (BindableProperty)type.GetFields().Where(field => field.Name == "TextColorProperty").ToList()[0].GetValue(null) };
                style.Setters.Add(setter);
                textStyles.Add(style);
                this.Add(style);
            }
        }
    }*/

    private StyleDictionnary()
	{
        App app = Microsoft.Maui.Controls.Application.Current as App;
        BackgroundColor = Color.FromArgb(app.Theme.BackgroundColorHexCode);
        ShellColor = Color.FromArgb(app.Theme.ShellColorHexCode);
        ButtonColor = Color.FromArgb(app.Theme.ButtonColorHexCode);
        TextColor = Color.FromArgb(app.Theme.TextColorHexCode);


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