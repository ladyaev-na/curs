using System.Text.Json;
using System.Text;
using curs.Models;

namespace curs.Views;

public partial class EditAccessPage : ContentPage
{
    private readonly HttpClient _httpClient;
    private readonly string _token;
    private readonly Access _access; // ������ ����������� ��� ��������������
    private string _selectedStartTime; // ��������� ����� ������
    private string _selectedEndTime;   // ��������� ����� ���������

    public EditAccessPage(string token, Access access)
    {
        InitializeComponent();
        _httpClient = new HttpClient();
        _token = token;
        _access = access;

        // ������������� ������
        DateLabel.Text = _access.Date.ToString("yyyy-MM-dd"); // ���������� ����
        _selectedStartTime = _access.StartChange.ToString(@"hh\:mm");
        _selectedEndTime = _access.EndChange.ToString(@"hh\:mm");
        SelectedStartTimeLabel.Text = $"������: {_selectedStartTime}";
        SelectedEndTimeLabel.Text = $"�����: {_selectedEndTime}";

        // ��������� ����� ������
        UpdateButtonColors();
    }

    // ���������� ��� ���������� ���������
    private async void OnSaveClicked(object sender, EventArgs e)
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

        // ���������� ���� �� ������� ����������� (��� �� ����������)
        var date = _access.Date.ToString("yyyy-MM-dd");

        var accessData = new
        {
            date, // ���� �������� ����������
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

    // ��������� ������ �������� ��� ���������
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