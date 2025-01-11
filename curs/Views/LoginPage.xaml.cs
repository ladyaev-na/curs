using System.Text.Json;
using System.Text;
using curs.Models;

namespace curs.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}

    private readonly HttpClient _httpClient = new HttpClient();

    private async void exet(object sender, EventArgs e)
    {
        string login = Login.Text;
        string password = Password.Text;

        if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Ошибка", "Введите логин и пароль", "OK");
            return;
        }

        var loginResponse = await AuthenticateUserAsync(login, password);
        if (loginResponse != null)
        {
            await Navigation.PushAsync(new IncomePage(loginResponse.User, loginResponse.Token));
        }
    }

    private async Task<AuthResponse> AuthenticateUserAsync(string login, string password)
    {
        var loginData = new { login, password };
        var jsonContent = new StringContent(JsonSerializer.Serialize(loginData), Encoding.UTF8, "application/json");

        try
        {
            HttpResponseMessage response = await _httpClient.PostAsync("http://courseproject4/api/login", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<AuthResponse>(content);

                if (result?.Token != null)
                {
                    return result;
                }
                await DisplayAlert("Ошибка", "Не удалось получить токен", "OK");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await DisplayAlert("Ошибка входа", "Неправильный логин или пароль", "OK");
            }
            else
            {
                await DisplayAlert("Ошибка", "Произошла ошибка на сервере", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка сети", ex.Message, "ОК");
        }
        return null;
    }
}