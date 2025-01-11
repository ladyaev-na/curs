using curs.Models;
using Microsoft.Maui.Controls.PlatformConfiguration;
using System.Net.Http.Headers;
using System.Text.Json;

namespace curs.Views;

public partial class Ass : ContentPage
{
    private readonly HttpClient _httpClient;
    private readonly string _token;
    public Ass(string token)
	{
        InitializeComponent();

        _httpClient = new HttpClient();
        _token = token;
        LoadAccessesAsync();
    }
    private async Task LoadAccessesAsync()
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync("http://courseproject4/api/accesses/my");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var accesses = JsonSerializer.Deserialize<List<Access>>(content);
                AccessesListView.ItemsSource = accesses;
            }
            else
            {
                await DisplayAlert("Ошибка", "Не удалось загрузить доступности", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка сети", ex.Message, "ОК");
        }
    }
}