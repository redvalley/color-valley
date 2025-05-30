using Plugin.AdMob.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.AdMob;

namespace ColorValley;

public partial class DataPrivacyPage : ContentPage
{
    public DataPrivacyPage()
    {
        InitializeComponent();
    }

    private async void RemoveConsentButton_OnClicked(object? sender, EventArgs e)
    {
        var adConsentService = IPlatformApplication.Current?.Services.GetService<IAdConsentService>();
        if (adConsentService != null)
        {
            var hasConsentRemovedDisplayAlert = await this.DisplayAlert(Properties.Resources.AlertTitleDataPrivacyConsentRemove,
                Properties.Resources.AlertMessageDataPrivacyConsentRemove, 
                Properties.Resources.ButtonOkText,
                Properties.Resources.ButtonCancelText);

            if (hasConsentRemovedDisplayAlert)
            {
                adConsentService.Reset();
            }
        }

    }
}