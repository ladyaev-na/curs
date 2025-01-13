using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using curs.Models;
using System.Net.Http;
using Microsoft.Maui.Controls;
using System.Text.Json;
using System.Linq;
using System.Text;

namespace curs.Views
{
    public partial class IncomePage : ContentPage, INotifyPropertyChanged
    {
        private User _user;
        private string _token;
        private readonly HttpClient _httpClient = new HttpClient();

        private ObservableCollection<Bonus> _bonuses;
        public ObservableCollection<Bonus> Bonuses
        {
            get => _bonuses;
            set
            {
                _bonuses = value;
                OnPropertyChanged(); // ����������� �� ���������
            }
        }

        public IncomePage(User user, string token)
        {
            InitializeComponent();
            _user = user; // ������������� ������������
            _token = token; // ������������� ������

            // ������������� �������� ������ ��� ��������
            BindingContext = this;

            // ������������� HttpClient
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

            // ��������� ������
            LoadBonusesAsync();
        }

        // ����� ��� �������� ������� � �������
        private async void LoadBonusesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("http://courseproject4/api/bonus");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var allBonuses = JsonSerializer.Deserialize<List<Bonus>>(content);

                    // ��������� ������ �� role_id = 2
                    var filteredBonuses = allBonuses.Where(b => b.RoleId == 2).ToList();

                    // ��������� ������ �������
                    Bonuses = new ObservableCollection<Bonus>(filteredBonuses);
                }
                else
                {
                    await DisplayAlert("������", "�� ������� ��������� ������.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("������ ����", ex.Message, "OK");
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

        private async void openButt(object sender, EventArgs e)
        {
            // �������� �����, ���������� � ������� 
            var selectedBonus = ((Button)sender).BindingContext as Bonus;

            if (selectedBonus != null)
            {
                try
                {
                    // ����������� ������ 
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

                    // ���������� ������ �� ������ 
                    var response = await _httpClient.GetAsync($"http://courseproject4/api/bonus/{selectedBonus.Id}");

                    if (response.IsSuccessStatusCode)
                    {
                        await Navigation.PushAsync(new BonusPage(selectedBonus, _user, _token));
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        await DisplayAlert("������", $"�� ������� �������� �����: {response.StatusCode} - {errorContent}", "��");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("������", $"��������� ������: {ex.Message}", "��");
                }
            }
        }
    }
}