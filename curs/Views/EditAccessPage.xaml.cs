using System.Text.Json;
using System.Text;
using curs.Models;

namespace curs.Views;

public partial class EditAccessPage : ContentPage
{
    private readonly HttpClient _httpClient;
    private readonly string _token;
    private readonly Access _access; // Объект доступности для редактирования
    private string _selectedStartTime; // Выбранное время начала
    private string _selectedEndTime;   // Выбранное время окончания

    public EditAccessPage(string token, Access access)
    {
        InitializeComponent();
        _httpClient = new HttpClient();
        _token = token;
        _access = access;

        // Предзаполняем данные
        DateLabel.Text = _access.Date.ToString("yyyy-MM-dd"); // Отображаем дату
        _selectedStartTime = _access.StartChange.ToString(@"hh\:mm");
        _selectedEndTime = _access.EndChange.ToString(@"hh\:mm");
        SelectedStartTimeLabel.Text = $"Начало: {_selectedStartTime}";
        SelectedEndTimeLabel.Text = $"Конец: {_selectedEndTime}";

        // Обновляем цвета кнопок
        UpdateButtonColors();
    }

    // Обработчик для сохранения изменений
    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_selectedStartTime) || string.IsNullOrEmpty(_selectedEndTime))
        {
            await DisplayAlert("Ошибка", "Выберите время начала и окончания", "OK");
            return;
        }

        // Проверяем, что время окончания больше времени начала
        if (TimeSpan.Parse(_selectedEndTime) <= TimeSpan.Parse(_selectedStartTime))
        {
            await DisplayAlert("Ошибка", "Время окончания должно быть позже времени начала", "OK");
            return;
        }

        // Используем дату из объекта доступности (она не изменяется)
        var date = _access.Date.ToString("yyyy-MM-dd");

        var accessData = new
        {
            date, // Дата остается неизменной
            startChange = _selectedStartTime,
            endChange = _selectedEndTime
        };

        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

        try
        {
            var response = await _httpClient.PutAsync(
                $"http://courseproject4/api/accesses/{_access.Id}",
                new StringContent(JsonSerializer.Serialize(accessData), Encoding.UTF8, "application/json")
            );

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Успех", "Доступность обновлена", "OK");
                await Navigation.PopAsync(); // Вернуться на предыдущую страницу
            }
            else
            {
                await DisplayAlert("Ошибка", "Не удалось обновить доступность", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка сети", ex.Message, "OK");
        }
    }

    // Остальные методы остаются без изменений
    private void OnTimeButtonClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var selectedTime = button.Text;

        if (string.IsNullOrEmpty(_selectedStartTime))
        {
            // Если время начала не выбрано, выбираем его
            _selectedStartTime = selectedTime;
            SelectedStartTimeLabel.Text = $"Начало: {_selectedStartTime}";
        }
        else if (string.IsNullOrEmpty(_selectedEndTime))
        {
            // Если время окончания не выбрано, проверяем, что оно больше времени начала
            if (TimeSpan.Parse(selectedTime) > TimeSpan.Parse(_selectedStartTime))
            {
                _selectedEndTime = selectedTime;
                SelectedEndTimeLabel.Text = $"Конец: {_selectedEndTime}";
            }
            else
            {
                // Если время окончания меньше или равно времени начала, показываем ошибку
                DisplayAlert("Ошибка", "Время окончания должно быть позже времени начала", "OK");
            }
        }
        else
        {
            // Если оба времени уже выбраны, сбрасываем выбор
            _selectedStartTime = selectedTime;
            _selectedEndTime = null;
            SelectedStartTimeLabel.Text = $"Начало: {_selectedStartTime}";
            SelectedEndTimeLabel.Text = "Конец: не выбрано";
        }

        // Обновляем цвета кнопок
        UpdateButtonColors();
    }

    private void UpdateButtonColors()
    {
        if (string.IsNullOrEmpty(_selectedStartTime) || string.IsNullOrEmpty(_selectedEndTime))
        {
            // Если интервал не выбран, сбрасываем цвета всех кнопок
            foreach (var child in TimeButtonsContainer.Children)
            {
                if (child is Button button)
                {
                    button.BackgroundColor = Colors.Transparent; // Сброс цвета
                }
            }
            return;
        }

        var startTime = TimeSpan.Parse(_selectedStartTime);
        var endTime = TimeSpan.Parse(_selectedEndTime);

        foreach (var child in TimeButtonsContainer.Children)
        {
            if (child is Button button)
            {
                var buttonTime = TimeSpan.Parse(button.Text);

                // Закрашиваем кнопки, которые попадают в интервал
                if (buttonTime >= startTime && buttonTime <= endTime)
                {
                    button.BackgroundColor = Colors.LightGreen; // Зеленый цвет для выбранного интервала
                }
                else
                {
                    button.BackgroundColor = Colors.Transparent; // Сброс цвета
                }
            }
        }
    }
}