﻿using System;
using System.Threading.Tasks;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Xamarin.Forms;

namespace HuesMiniHack.ViewModels
{
    public class BridgeRegisterViewModel : BaseViewModel
    {

        public string SelectedBridgeIP { get; set;}

        Command registerAppCommand;
        public Command RegisterAppCommand
        {
            get { return registerAppCommand ?? (registerAppCommand = new Command(async () => await ExecuteRegisterAppCommand())); }
        }

        async Task ExecuteRegisterAppCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                ILocalHueClient client = new LocalHueClient(SelectedBridgeIP);

                if (string.IsNullOrEmpty(Helpers.Settings.AppKey))
                    Helpers.Settings.AppKey = await client.RegisterAsync(App.AppName, App.DeviceName);

                client.Initialize(Helpers.Settings.AppKey);

                Acr.UserDialogs.UserDialogs.Instance.ShowSuccess("Added Bridge!");
            }
            catch (Exception ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowError(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public Color ButtonBackgroundColor
        {
            get
            {
                return Color.FromHex("#81C134");
            }
        }

        public ImageSource LinkImageSource
        {
            get
            {
                return ImageSource.FromFile("link.png");
            }
        }
    }
}


