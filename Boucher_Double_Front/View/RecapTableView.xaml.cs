using Boucher_Double_Front.ViewModel;
using Boucher_DoubleModel.Models.Entitys;
using Microsoft.Maui.Controls.Shapes;
using System.Drawing;
using Color = Microsoft.Maui.Graphics.Color;

namespace Boucher_Double_Front.View;

public partial class RecapTableView : ContentPage
{
	private RecapTableViewModel model=new();
	public RecapTableView()
	{
        Resources = StyleDictionnary.GetInstance();
        InitializeComponent();
        BindingContext = model;
        FillGrid();
	}

    public void OnEventSelection(object sender, EventArgs e)
    {
        if (EventPicker.SelectedIndex > 0)
        {
            model.Filter(EventPicker.SelectedItem as Event);
        }
        else
        {
            model.Filter(null);
        }
        FillGrid();
    }


    public void FillGrid()
	{
        table.Children.Clear();
        table.ColumnDefinitions.Add(new() { Width = GridLength.Auto });
        table.ColumnDefinitions.Add(new() { Width = GridLength.Auto });
        table.ColumnDefinitions.Add(new() { Width = GridLength.Auto });
        table.RowDefinitions.Add(new() { Height = GridLength.Auto });
        table.RowDefinitions.Add(new() {Height = GridLength.Auto});
        table.RowDefinitions.Add(new() { Height = GridLength.Auto });
        Border cell = new()
        {
            Stroke = Colors.Black,
            StrokeThickness = 4,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(0, 0, 0, 0)
            },
        };
        Label totaltitle = new() { Text = "Total", HorizontalOptions = LayoutOptions.Center , VerticalOptions = LayoutOptions.Center };
        cell.Content = totaltitle;
        table.Add(cell, 0, 2);
        Grid.SetColumnSpan(cell, 2);
        int categoryNameColumn = 0;
        foreach (var productGrouping in model.Products)
        {
            int i = 0;
            Border categoryCell = new() { Stroke = Colors.Black, StrokeThickness = 4,  
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                StrokeShape = new RoundRectangle
                {
                    CornerRadius = new CornerRadius(0, 0, 0, 0)
                },
            };
            Label categoryName = new() { Text = model.IdCategoryPair[productGrouping.Key].Name ,HorizontalOptions=LayoutOptions.Center};
            categoryCell.Content = categoryName;
            table.Add(categoryCell, categoryNameColumn + 2, 0);

            foreach (var product in productGrouping)
            {
                Border labelBorder = new() { Stroke = Colors.Black, StrokeThickness = 4,  
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill,
                    StrokeShape = new RoundRectangle
                    {
                        CornerRadius = new CornerRadius(0, 0, 0, 0)
                    },
                };
                Border totalBorder = new() { Stroke =Colors.Black, StrokeThickness = 4,  
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill,
                    StrokeShape = new RoundRectangle
                    {
                        CornerRadius = new CornerRadius(0, 0, 0, 0)
                    },
                };
                Label label = new() { Text = product.Name, HorizontalOptions = LayoutOptions.Center , VerticalOptions = LayoutOptions.Center };
                labelBorder.Content = label;
                Label total = new() { Text = model.Total[product.Id].ToString(), HorizontalOptions = LayoutOptions.Center , VerticalOptions = LayoutOptions.Center };
                totalBorder.Content = total;
                table.ColumnDefinitions.Add(new() { Width = GridLength.Auto });
                table.Add(labelBorder, categoryNameColumn + i + 2, 1);
                table.Add(totalBorder, categoryNameColumn + i + 2, 2);
                i++;
            }
            Grid.SetColumnSpan(categoryCell, productGrouping.Count());
            categoryNameColumn += productGrouping.Count();
        }
        int selloutClientLine = 0;
        foreach (var selloutGrouping in model.CompletedSellouts)
        {
            Border labelnameBorder = new()
            {
                Stroke = Colors.Black,
                StrokeThickness = 4,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                StrokeShape = new RoundRectangle
                {
                    CornerRadius = new CornerRadius(0, 0, 0, 0)
                },
            };
            Label labelName = new() { Text = model.IdClientPair[selloutGrouping.Key].Name, HorizontalOptions = LayoutOptions.Center , VerticalOptions = LayoutOptions.Center };
            labelnameBorder.Content = labelName;
            int i = 0;
            foreach (var sellout in selloutGrouping)
            {
                Border labelidBorder = new() { Stroke = Colors.Black, StrokeThickness = 4,
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill,
                    StrokeShape = new RoundRectangle
                    {
                        CornerRadius = new CornerRadius(0, 0, 0, 0)
                    },
                };

                Label labelid = new() { Text = sellout.Sellout.Id.ToString(), HorizontalOptions = LayoutOptions.Center , VerticalOptions = LayoutOptions.Center };
                labelidBorder.Content = labelid;
                table.RowDefinitions.Add(new() { Height = GridLength.Auto });
                table.Add(labelidBorder, 1, selloutClientLine + i + 3);
                foreach (var line in sellout.Lines)
                {
                    Border labelBorder = new() { Stroke = Colors.Black, StrokeThickness = 4,  
                        HorizontalOptions = LayoutOptions.Fill,
                        VerticalOptions = LayoutOptions.Fill,
                        StrokeShape = new RoundRectangle
                        {
                            CornerRadius = new CornerRadius(0, 0, 0, 0)
                        },
                    };
                    Label label = new() { Text = line.Quantity.ToString(), HorizontalOptions = LayoutOptions.Center , VerticalOptions = LayoutOptions.Center };
                    labelBorder.Content = label;
                    table.Add(labelBorder, sellout.Lines.IndexOf(line) + 2, selloutClientLine + i + 3);
                }
                i++;
            }
            table.Add(labelnameBorder, 0, selloutClientLine+3);
            Grid.SetRowSpan(labelnameBorder, selloutGrouping.Count());
            selloutClientLine += selloutGrouping.Count();
        }
    }
}