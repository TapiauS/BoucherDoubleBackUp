using Boucher_DoubleModel.Models.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui;
using Microsoft.Maui.Controls.Xaml;
using MimeKit.Text;
using MimeKit;
using MailKit.Security;
using MailKit.Net.Smtp;
using Boucher_Double_Front.Services;

namespace Boucher_Double_Front.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OneSellout : ContentPage
    {
        public string Bill { get; set; }
        public OneSellout()
        {
            InitializeComponent();
            try
            {
                
                App appInstance = Application.Current as App;
                Sellout sellout = appInstance.ActivCommand;
                BillParameter billParameter=appInstance.BillParameter;
                int totalWidth = 60; // Définissez la largeur totale de la facture
                string invoiceNumber = $"n° de facture: {sellout.Id}";
                string centeredInvoiceNumber = invoiceNumber.PadLeft(invoiceNumber.Length/2 + totalWidth);
                string boughtItems = string.Join("\n", sellout.BuyedProducts.Select(p => $"{p.SoldProduct.Name} Quantité: {p.Quantity} Prix:{p.SoldProduct.Price.GetPrice()} ({p.SoldProduct.Price.GetPrice() * p.Quantity})"));
                decimal total=0;
                foreach(var item in sellout.BuyedProducts)
                {
                    total += item.SoldProduct.Price.GetPrice() * item.Quantity;
                }
                Bill = $"{centeredInvoiceNumber}\n" +
                            $"------------------------------------------------\n" +
                            $"{sellout.SelloutDate.ToString("dd/MM/yyyy")}\n" +
                            $"------------------------------------------------\n" +
                            $"{sellout.Client.Name.ToUpper()} {sellout.Client.Surname.ToUpper()} n°tel: {sellout.Client.PhoneNumber}\n" +
                            $"Mail: {sellout.Client.Mail} Crée le : {sellout.ReceiptDate.ToString("dd/MM/yyyy")}\n" +
                            $"------------------------------------------------\n" +
                            $"\n{boughtItems}\n" +
                            $"------------------------------------------------\n" +
                            $"{billParameter.Foot}\n" +
                            $"Mention spéciale: {billParameter.SpecialMention}\n" +
                            $"Total :{total} €\n" +
                            $"Mention: {billParameter.Mention}\n";
                BindingContext = this;
            }
            catch(Exception ex) 
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void GetDeviceList()
        {
            IPrintService printService=DependencyService.Get<IPrintService>();
            BlueToothPicker.ItemsSource = printService.GetDeviceList().ToList();
        }


        public async void ValidateAndSendMail(object sender,EventArgs args)
        {
            App app=Application.Current as App;
            IPrintService printService = DependencyService.Get<IPrintService>();
            if (app.MailAccessParameter!=null)
            {
                if(app.MailParameter!=null)
                {
                    string fromEmail = app.User.Store.Mail;
                    string password = app.MailAccessParameter.Password;
                    string toEmail = app.ActivCommand.Client.Mail;
                    string subject = app.MailParameter.MailObject;
                    string body = $"{app.MailParameter.MailHead}\n" +
                                  $"{Bill}\n" +
                                  $"{app.MailParameter.MailFoot}";
                    var email = new MimeMessage();
                    email.From.Add(app.MailAccessParameter.Login == null || app.MailAccessParameter.Login == ""?MailboxAddress.Parse(fromEmail): MailboxAddress.Parse(app.MailAccessParameter.Login));
                    email.To.Add(MailboxAddress.Parse(toEmail));
                    email.Subject = subject;
                    email.Body = new TextPart(TextFormat.Plain) { Text = body };

                    var smtp = new SmtpClient();
                    smtp.Connect(app.MailAccessParameter.Server, app.MailAccessParameter.Port, app.MailAccessParameter.ConnexionType == null? SecureSocketOptions.StartTls: (SecureSocketOptions)app.MailAccessParameter.ConnexionType);
                    smtp.Authenticate(app.MailAccessParameter.Login == null || app.MailAccessParameter.Login == ""?fromEmail: app.MailAccessParameter.Login, password);
                    smtp.Send(email);
                    smtp.Disconnect(true);
                    smtp.Dispose();
                    await printService.Print((string)BlueToothPicker.SelectedItem,Bill);
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Vous devez définir votre contenu de mail", "Ok");
                }
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Vous devez définir vos paramétre d'accés au mail, voir avec un admin", "Ok");
            }
        }

    }
}