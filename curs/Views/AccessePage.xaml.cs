using curs.Models;
using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Maui.Controls;
using System.Security.Authentication;
using Microsoft.Maui.Networking;

namespace curs.Views
{
    public partial class AccessePage : ContentPage
    {
        private readonly HttpClient _httpClient;
        private readonly string _token;
        public AccessePage(string token)
        {
            InitializeComponent();
            _httpClient = new HttpClient();
            _token = token;
            LoadAccessesAsync();
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadAccessesAsync(); // Обновляем список доступностей
        }
        private async Task LoadAccessesAsync()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

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
                await DisplayAlert("Ошибка сети", ex.Message, "OK");
            }
        }

        private async void OnAddAccessClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddAccessPage(_token));
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var accessId = button.CommandParameter.ToString(); // Получаем ID доступности

            bool confirm = await DisplayAlert("Подтверждение", "Вы уверены, что хотите удалить доступность?", "Да", "Нет");
            if (!confirm)
            {
                return; // Если пользователь отменил удаление
            }

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

                // Отправляем запрос на удаление
                var response = await _httpClient.DeleteAsync($"http://courseproject4/api/accesses/{accessId}");

                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Успех", "Доступность удалена", "OK");
                    await LoadAccessesAsync(); // Обновляем список доступностей
                }
                else
                {
                    await DisplayAlert("Ошибка", "Не удалось удалить доступность", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка сети", ex.Message, "OK");
            }
        }
    }
}