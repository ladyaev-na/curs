using curs.Models;
using Microsoft.Maui.Controls.PlatformConfiguration;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace curs.Views;

public partial class BonusPage : ContentPage
{
    private User _user;
    private string _token;
    private Bonus _bonus;
    private readonly HttpClient _httpClient = new HttpClient();
    public BonusPage(Bonus bonus, User user, string token)
    {
        InitializeComponent();
        _bonus = bonus;
        _user = user;
        _token = token;

        nameLabel.Text = bonus.Title;
        descriptionLabel.Text = bonus.Description;
        priceLabel.Text = "Награда: " + bonus.FormattedPrice;
    }
}