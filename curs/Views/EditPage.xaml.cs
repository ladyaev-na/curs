using System.Text.Json;
using System.Text;
using curs.Models;

namespace curs.Views;

public partial class EditPage : ContentPage
{
    private readonly HttpClient _httpClient = new HttpClient();
    private User _user;
    private string _token;
    private string _userId;

    public EditPage(User user, string token)
    {
        InitializeComponent();

        _user = user;
        _token = token;
        _userId = user.Id.ToString();

        EntrySurname.Text = user.Surname;
        EntryName.Text = user.Name;
        EntryPatronymic.Text = user.Patronymic;
        EntryLogin.Text = user.Login;
    }

    private async void save(object sender, EventArgs e)
    {
        var profileData = new
        {
            surname = EntrySurname.Text,
            name = EntryName.Text,
            patronymic = EntryPatronymic.Text,
            login = EntryLogin.Text,
            password = EntryPassword.Text,
            confirmPassword = EntryConfirmPassword.Text
        };

        bool success = await EditeProfile(profileData);
        if (success)
        {
            await DisplayAlert("Успех", "Профиль успешно обновлен", "OK");
            await Navigation.PopAsync();
        }
    }

    private async Task<bool> EditeProfile(object profileData)
    {
        var jsonContent = new StringContent(JsonSerializer.Serialize(profileData), Encoding.UTF8, "application/json");

        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
            HttpResponseMessage response = await _httpClient.PutAsync($"http://courseproject4/api/profile/{_userId}", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                await DisplayAlert("Ошибка", "Ресурс не найден. Проверьте идентификатор пользователя.", "OK");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Ошибка", $"Произошла ошибка на сервере. Код: {response.StatusCode}. Сообщение: {errorContent}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка сети", ex.Message, "ОК");
        }
        return false;
    }
}