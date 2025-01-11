using System.Text.Json;
using System.Text;

namespace curs.Views;

public partial class AddAccessPage : ContentPage
{
    private readonly HttpClient _httpClient;
    private readonly string _token;
    private string _selectedStartTime; // ��������� ����� ������
    private string _selectedEndTime;   // ��������� ����� ���������
    public AddAccessPage(string token)
    {
        InitializeComponent();
        _httpClient = new HttpClient();
        _token = token;
    }
    // ���������� ��� ������ �������
    private void OnTimeButtonClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var selectedTime = button.Text;

        if (string.IsNullOrEmpty(_selectedStartTime))
        {
            // ���� ����� ������ �� �������, �������� ���
            _selectedStartTime = selectedTime;
            SelectedStartTimeLabel.Text = $"������: {_selectedStartTime}";
        }
        else if (string.IsNullOrEmpty(_selectedEndTime))
        {
            // ���� ����� ��������� �� �������, ���������, ��� ��� ������ ������� ������
            if (TimeSpan.Parse(selectedTime) > TimeSpan.Parse(_selectedStartTime))
            {
                _selectedEndTime = selectedTime;
                SelectedEndTimeLabel.Text = $"�����: {_selectedEndTime}";
            }
            else
            {
                // ���� ����� ��������� ������ ��� ����� ������� ������, ���������� ������
                DisplayAlert("������", "����� ��������� ������ ���� ����� ������� ������", "OK");
            }
        }
        else
        {
            // ���� ��� ������� ��� �������, ���������� �����
            _selectedStartTime = selectedTime;
            _selectedEndTime = null;
            SelectedStartTimeLabel.Text = $"������: {_selectedStartTime}";
            SelectedEndTimeLabel.Text = "�����: �� �������";
        }

        // ��������� ����� ������
        UpdateButtonColors();
    }

    // ���������� ��� ���������� �����������
    private async void OnAddClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_selectedStartTime) || string.IsNullOrEmpty(_selectedEndTime))
        {
            await DisplayAlert("������", "�������� ����� ������ � ���������", "OK");
            return;
        }

        // ���������, ��� ����� ��������� ������ ������� ������
        if (TimeSpan.Parse(_selectedEndTime) <= TimeSpan.Parse(_selectedStartTime))
        {
            await DisplayAlert("������", "����� ��������� ������ ���� ����� ������� ������", "OK");
            return;
        }

        var date = DateEntry.Text; // �����������, ��� DateEntry � ��� ���� ��� ������ ����
        if (string.IsNullOrEmpty(date))
        {
            await DisplayAlert("������", "�������� ����", "OK");
            return;
        }

        var accessData = new
        {
            date,
            startChange = _selectedStartTime,
            endChange = _selectedEndTime
        };

        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

        try
        {
            var response = await _httpClient.PostAsync(
                "http://courseproject4/api/accesses",
                new StringContent(JsonSerializer.Serialize(accessData), Encoding.UTF8, "application/json")
            );

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("�����", "����������� ���������", "OK");
                await Navigation.PopAsync(); // ��������� �� ���������� ��������
            }
            else
            {
                await DisplayAlert("������", "�� ������� �������� �����������", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("������ ����", ex.Message, "OK");
        }
    }

    private void UpdateButtonColors()
    {
        if (string.IsNullOrEmpty(_selectedStartTime) || string.IsNullOrEmpty(_selectedEndTime))
        {
            // ���� �������� �� ������, ���������� ����� ���� ������
            foreach (var child in TimeButtonsContainer.Children)
            {
                if (child is Button button)
                {
                    button.BackgroundColor = Colors.Transparent; // ����� �����
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

                // ����������� ������, ������� �������� � ��������
                if (buttonTime >= startTime && buttonTime <= endTime)
                {
                    button.BackgroundColor = Colors.LightGreen; // ������� ���� ��� ���������� ���������
                }
                else
                {
                    button.BackgroundColor = Colors.Transparent; // ����� �����
                }
            }
        }
    }
}