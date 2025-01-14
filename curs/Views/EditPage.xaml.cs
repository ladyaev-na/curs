using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using curs.Models;
using Microsoft.Maui.Controls;

namespace curs.Views
{
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

            EntrySurname.Text = user.Surname;
            EntryName.Text = user.Name;
            EntryPatronymic.Text = user.Patronymic;
            EntryLogin.Text = user.Login;

            LoadFineDetails();
            LoadStatusDetails();

        }

        private async void LoadFineDetails()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

            try
            {
                // Загружаем информацию о штрафах 
                var responseFine = await _httpClient.GetAsync($"http://courseproject4/api/fine/{_user.FineId}");
                var contentFine = await responseFine.Content.ReadAsStringAsync();


                if (responseFine.IsSuccessStatusCode)
                {
                    var fine = JsonSerializer.Deserialize<Fine>(contentFine);
                    FineLabel.Text = fine.Description;
                }
                else
                {
                    FineLabel.Text = "Не удалось загрузить штраф";
                    await DisplayAlert("Ошибка", $"Код: {responseFine.StatusCode}, Ответ: {contentFine}", "OK");
                }
            }
            catch (Exception ex)
            {
                FineLabel.Text = "Ошибка при загрузке штрафа";
                await DisplayAlert("Ошибка", ex.Message, "OK");
            }
        }

        private async void LoadStatusDetails()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

            try
            {
                // Загружаем информацию о штрафах 
                var responseStatus = await _httpClient.GetAsync($"http://courseproject4/api/status/{_user.StatusId}");
                var contentStatus = await responseStatus.Content.ReadAsStringAsync();


                if (responseStatus.IsSuccessStatusCode)
                {
                    var status = JsonSerializer.Deserialize<Status>(contentStatus);
                    StatusLabel.Text = status.Name;
                }
                else
                {
                    StatusLabel.Text = "Не удалось загрузить статус";
                    await DisplayAlert("Ошибка", $"Код: {responseStatus.StatusCode}, Ответ: {contentStatus}", "OK");
                }
            }
            catch (Exception ex)
            {
                StatusLabel.Text = "Ошибка при загрузке статусв";
                await DisplayAlert("Ошибка", ex.Message, "OK");
            }
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
}