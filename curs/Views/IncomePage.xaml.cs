using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using curs.Models;
using System.Net.Http;
using Microsoft.Maui.Controls;
using System.Text.Json;
using System.Threading.Tasks;

namespace curs.Views
{
    public partial class IncomePage : ContentPage, INotifyPropertyChanged
    {
        private string _userNameLabel;
        private string _userRoleLabel;
        private ObservableCollection<Bonus> _bonuses;

        public string UserNameLabel
        {
            get => _userNameLabel;
            set
            {
                _userNameLabel = value;
                OnPropertyChanged();
            }
        }

        public string UserRoleLabel
        {
            get => _userRoleLabel;
            set
            {
                _userRoleLabel = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Bonus> Bonuses
        {
            get => _bonuses;
            set
            {
                _bonuses = value;
                OnPropertyChanged();
            }
        }

        private readonly HttpClient _httpClient;
        private readonly string _token;
        private readonly User _user;

        public IncomePage(User user, string token)
        {
            InitializeComponent();
            _user = user; // Инициализация пользователя
            _token = token; // Инициализация токена

            // Устанавливаем контекст данных для привязки
            BindingContext = this;

            // Инициализация HttpClient
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

            // Устанавливаем начальные значения
            UserNameLabel = _user.Name;
            UserRoleLabel = GetRoleName(_user.RoleId); // Получаем название роли

            // Загружаем бонусы
            LoadBonusesAsync();
        }

        // Метод для получения названия роли по RoleId
        private string GetRoleName(ulong roleId)
        {
            switch (roleId)
            {
                case 1:
                    return "Администратор";
                case 2:
                    return "Пользователь";
                case 3:
                    return "Гость";
                default:
                    return "Неизвестная роль";
            }
        }

        // Метод для загрузки бонусов с сервера
        private async void LoadBonusesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("http://courseproject4/api/bonus");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var bonuses = JsonSerializer.Deserialize<List<Bonus>>(content);

                    // Обновляем список бонусов
                    Bonuses = new ObservableCollection<Bonus>(bonuses);
                }
                else
                {
                    await DisplayAlert("Ошибка", "Не удалось загрузить бонусы.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка сети", ex.Message, "OK");
            }
        }

        private async void edit(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EditPage(_user, _token));
        }

        private async void accesse(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AccessePage(_token));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}