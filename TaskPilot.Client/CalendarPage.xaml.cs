using Shared.DTOs;
using TaskPilot.Client.Services;
using System.Diagnostics;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Storage;
using System.Linq;

namespace TaskPilot.Client
{
    public partial class CalendarPage : ContentPage
    {
        private readonly TodoService _todoService;
        private List<TodoGetDto> _allTasks = new();
        private DateTime _currentDay;
        private bool _isInitialLoad = true;

        private int currentUserId = int.Parse(Preferences.Get("UserID", "0"));
        private const int RowHeight = 60;    // height per hour row

        public CalendarPage(TodoService todoService)
        {
            InitializeComponent();
            _todoService = todoService;

            InitializeCalendarGrid();
        }

        private void InitializeCalendarGrid()
        {
            // Create 24 hour rows
            CalendarGrid.RowDefinitions.Clear();
            for (int i = 0; i < 24; i++)
            {
                CalendarGrid.RowDefinitions.Add(new RowDefinition { Height = RowHeight });
            }

            // Add time labels in column 0
            for (int hour = 0; hour < 24; hour++)
            {
                var time = new DateTime(2000, 1, 1, hour, 0, 0);
                var timeLabel = new Label
                {
                    Text = time.ToString("h tt").ToUpper(),
                    FontSize = 11,
                    TextColor = Colors.Gray,
                    VerticalOptions = LayoutOptions.Start,
                    Padding = new Thickness(0, 5, 0, 0)
                };

                Grid.SetRow(timeLabel, hour);
                Grid.SetColumn(timeLabel, 0);
                CalendarGrid.Children.Add(timeLabel);
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            _currentDay = DateTime.Today;
            await LoadTasksAndDisplay();

            if (_isInitialLoad)
            {
                // small delay so layout measured, then scroll to 8am
                await Task.Delay(100);
                await CalendarScrollView.ScrollToAsync(0, (8 * RowHeight) - RowHeight, false);
                _isInitialLoad = false;
            }
        }

        private async Task LoadTasksAndDisplay()
        {
            try
            {
                _allTasks = await _todoService.GetTodosAsync(currentUserId);
                UpdateCalendarView();
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Error", $"Failed to load tasks: {ex.Message}", "OK");
            }
        }

        private void UpdateCalendarView()
        {
            UpdateDayLabel();
            PopulateCalendarForDay(_currentDay);
        }

        private void UpdateDayLabel()
        {
            var today = DateTime.Today;
            if (_currentDay.Date == today)
            {
                DayLabel.Text = $"Today • {_currentDay:ddd, MMM d}";
            }
            else
            {
                DayLabel.Text = _currentDay.ToString("dddd, MMM d");
            }
        }

        #region Navigation (day-by-day)

        private void GoToToday_Tapped(object sender, TappedEventArgs e)
        {
            _currentDay = DateTime.Today;
            UpdateCalendarView();
            _ = ScrollToHourAsync(8);
        }

        private void PreviousDay_Clicked(object sender, EventArgs e)
        {
            _currentDay = _currentDay.AddDays(-1);
            UpdateCalendarView();
            _ = ScrollToHourAsync(8);
        }

        private void NextDay_Clicked(object sender, EventArgs e)
        {
            _currentDay = _currentDay.AddDays(1);
            UpdateCalendarView();
            _ = ScrollToHourAsync(8);
        }

        #endregion

        private async Task ScrollToHourAsync(int hour)
        {
            hour = Math.Clamp(hour, 0, 23);
            await CalendarScrollView.ScrollToAsync(0, (hour * RowHeight) - RowHeight, true);
        }

        /// <summary>
        /// Render tasks that belong to the selected day into the single day column.
        /// Only deadlines are shown (uses DueDateTime). Tasks without a valid DueDate are skipped.
        /// </summary>
        private void PopulateCalendarForDay(DateTime day)
        {
            // Remove existing task frames (keep time labels)
            var frames = CalendarGrid.Children.Where(c => c is Frame).ToList();
            foreach (var f in frames) CalendarGrid.Children.Remove(f);

            var startOfDay = day.Date;

            // Show only tasks whose DueDateTime falls on the selected day
            var tasksForDay = _allTasks
                .Where(t => t.DueDateTime > DateTime.MinValue && t.DueDateTime.Date == startOfDay)
                .OrderBy(t => t.DueDateTime)
                .ToList();

            foreach (var task in tasksForDay)
            {
                var due = task.DueDateTime;
                var hour = Math.Clamp(due.Hour, 0, 23);

                // simple label includes time + title so it's clear it's a deadline
                var content = new Label
                {
                    Text = $"{due:hh\\:mm tt} • {task.Title}",
                    TextColor = Colors.White,
                    FontSize = 12,
                    FontAttributes = FontAttributes.Bold
                };

                var taskFrame = new Frame
                {
                    BackgroundColor = Color.FromArgb("#1E1E5A"),
                    CornerRadius = 5,
                    Padding = new Thickness(8, 5),
                    Margin = new Thickness(2),
                    Content = content
                };

                Grid.SetRow(taskFrame, hour);
                Grid.SetColumn(taskFrame, 1); // day column
                CalendarGrid.Children.Add(taskFrame);
            }
        }

        // small wrapper to avoid using obsolete Sync APIs
        private Task DisplayAlertAsync(string title, string message, string cancel) =>
            this.DisplayAlert(title, message, cancel);

    }
}