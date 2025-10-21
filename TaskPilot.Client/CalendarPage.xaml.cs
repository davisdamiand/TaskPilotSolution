using Shared.DTOs;
using TaskPilot.Client.Services;
using System.Diagnostics;

namespace TaskPilot.Client
{
    public partial class CalendarPage : ContentPage
    {
        private readonly TodoService _todoService;
        private List<TodoGetDto> _allTasks = new List<TodoGetDto>();
        private DateTime _currentWeekStartDate;
        private bool _isInitialLoad = true;

        private int currentUserId = int.Parse(Preferences.Get("UserID", null));
        private const int RowHeight = 60;    // Define row height as a constant for easy calculation

        public CalendarPage(TodoService todoService)
        {
            InitializeComponent();
            _todoService = todoService;

            // Generate the static parts of the grid once when the page is constructed
            InitializeCalendarGrid();
        }

        /// <summary>
        /// This method builds the 24-hour grid structure (rows and time labels) when the page is first created.
        /// This is done only once to improve performance.
        /// </summary>
        private void InitializeCalendarGrid()
        {
            // Add 24 rows to the Grid, one for each hour of the day
            for (int i = 0; i < 24; i++)
            {
                CalendarGrid.RowDefinitions.Add(new RowDefinition { Height = RowHeight });
            }

            // Add 24 time labels to the first column (the time ruler)
            for (int hour = 0; hour < 24; hour++)
            {
                var time = new DateTime(2000, 1, 1, hour, 0, 0); // Date doesn't matter, only time
                var timeLabel = new Label
                {
                    // Formats as "12 AM", "1 AM", ..., "12 PM", "1 PM", etc.
                    Text = time.ToString("h tt").ToUpper(),
                    FontSize = 11,
                    TextColor = Colors.Gray,
                    VerticalOptions = LayoutOptions.Start,
                    Padding = new Thickness(0, 5, 0, 0) // Align text nicely at the top of the row
                };

                Grid.SetRow(timeLabel, hour);
                Grid.SetColumn(timeLabel, 0);
                CalendarGrid.Children.Add(timeLabel);
            }
        }

        private List<TodoGetDto> CreateMockTasks()
        {
            Debug.WriteLine("--- USING MOCK DATA FOR TESTING ---");
            var today = DateTime.Today;
            var startOfWeek = GetStartOfWeek(today); // Use the existing helper

            return new List<TodoGetDto>
    {
        // A task for today at 10 AM
        new TodoGetDto
        {
            Id = 1,
            Title = "Test Task Today",
            DueDateTime = today.Date.AddHours(10),
            StartDateTime = today.Date.AddHours(10),
            EndDateTime = today.Date.AddHours(11)
        },
        // A task for tomorrow (Tuesday, if today is Monday) at 2 PM
        new TodoGetDto
        {
            Id = 2,
            Title = "Review PR",
            DueDateTime = startOfWeek.AddDays(1).AddHours(14), // Tuesday at 2 PM
            StartDateTime = startOfWeek.AddDays(1).AddHours(14),
            EndDateTime = startOfWeek.AddDays(1).AddHours(15)
        },
        // A task for Friday at 9 AM
        new TodoGetDto
        {
            Id = 3,
            Title = "Deploy to Staging",
            DueDateTime = startOfWeek.AddDays(4).AddHours(9), // Friday at 9 AM
            StartDateTime = startOfWeek.AddDays(4).AddHours(9),
            EndDateTime = startOfWeek.AddDays(4).AddHours(10)
        },
        // A task for NEXT week to confirm it's correctly filtered out
        new TodoGetDto
        {
            Id = 4,
            Title = "Next Week's Plan",
            DueDateTime = startOfWeek.AddDays(8).AddHours(11), // Next Tuesday
            StartDateTime = startOfWeek.AddDays(8).AddHours(11),
            EndDateTime = startOfWeek.AddDays(8).AddHours(12)
        }
    };
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            _currentWeekStartDate = GetStartOfWeek(DateTime.Today);
            await LoadTasksAndDisplay();

            // On the very first load, automatically scroll the view to 8 AM.
            // This prevents the user from always seeing the middle of the night first.
            if (_isInitialLoad)
            {
                // We use a small delay to ensure the UI is fully rendered before scrolling.
                await Task.Delay(100);
                await CalendarScrollView.ScrollToAsync(0, (8 * RowHeight) - RowHeight, false); // Scroll to 8am
                _isInitialLoad = false;
            }
        }

        private async Task LoadTasksAndDisplay()
        {
            try
            {
                _allTasks = await _todoService.GetTodosAsync(currentUserId);
                Debug.WriteLine("--- TESTING WITH REAL DATA ---");
                Debug.WriteLine($"API call completed. Fetched {_allTasks.Count} total tasks from the service for user ID: {currentUserId}.");

                if (_allTasks.Any())
                {
                    Debug.WriteLine("Tasks returned from API:");
                    foreach (var task in _allTasks)
                    {
                        // We log the crucial DueDateTime property to check if it's correct
                        Debug.WriteLine($"  - Title: '{task.Title}', DueDateTime: {task.DueDateTime:yyyy-MM-dd HH:mm:ss}");
                    }
                }
                else
                {
                    Debug.WriteLine("The API returned an empty list of tasks. This is a likely cause of the blank screen.");
                }
                // --- END OF LOGGING ---

                // This will now try to display the real data
                UpdateCalendarView();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load tasks: {ex.Message}", "OK");
            }
        }

        /// <summary>
        /// Central method to refresh all parts of the calendar UI in the correct order.
        /// </summary>
        private void UpdateCalendarView()
        {
            UpdateWeekLabel();
            UpdateDayHeaders(_currentWeekStartDate);
            PopulateCalendarForWeek(_currentWeekStartDate);
        }

        #region Navigation Event Handlers

        private void GoToCurrentWeek_Tapped(object sender, TappedEventArgs e)
        {
            DateTime todayStartOfWeek = GetStartOfWeek(DateTime.Today);

            // Only perform the navigation if the user is not already viewing the current week
            if (_currentWeekStartDate.Date != todayStartOfWeek.Date)
            {
                _currentWeekStartDate = todayStartOfWeek;
                UpdateCalendarView();
            }
        }

        private void PreviousWeek_Clicked(object sender, EventArgs e)
        {
            _currentWeekStartDate = _currentWeekStartDate.AddDays(-7);
            UpdateCalendarView();
        }

        private void NextWeek_Clicked(object sender, EventArgs e)
        {
            _currentWeekStartDate = _currentWeekStartDate.AddDays(7);
            UpdateCalendarView();
        }

        #endregion

        #region UI Drawing and Helper Methods

        /// <summary>
        /// Populates the main calendar grid with task frames for a given week.
        /// </summary>
        private void PopulateCalendarForWeek(DateTime weekStartDate)
        {
            // Clear only the task frames from previous renders, leaving the time labels intact.
            var tasksToRemove = CalendarGrid.Children.Where(c => c is Frame).ToList();
            foreach (var taskView in tasksToRemove)
            {
                CalendarGrid.Children.Remove(taskView);
            }

            DateTime weekEndDate = weekStartDate.AddDays(7);
            var tasksForWeek = _allTasks.Where(t => t.DueDateTime >= weekStartDate && t.DueDateTime < weekEndDate).ToList();

            foreach (var task in tasksForWeek)
            {
                int column = GetDayColumn(task.DueDateTime.DayOfWeek);
                // The row index is now simply the hour of the day (0-23).
                int row = task.DueDateTime.Hour;

                // The row check now validates against all 24 hours.
                if (column > 0 && row >= 0 && row < 24)
                {
                    var taskFrame = new Frame
                    {
                        BackgroundColor = Color.FromHex("#1E1E5A"),
                        CornerRadius = 5,
                        Padding = new Thickness(8, 5),
                        Margin = new Thickness(2),
                        Content = new Label
                        {
                            Text = task.Title,
                            TextColor = Colors.White,
                            FontSize = 12,
                            FontAttributes = FontAttributes.Bold
                        }
                    };

                    Grid.SetRow(taskFrame, row);
                    Grid.SetColumn(taskFrame, column);
                    CalendarGrid.Children.Add(taskFrame);
                }
            }
        }

        /// <summary>
        /// Dynamically generates the day headers (e.g., "20 Mon") for the current week.
        /// </summary>
        private void UpdateDayHeaders(DateTime weekStartDate)
        {
            DayHeaderGrid.Children.Clear(); // Clear previous headers

            for (int i = 0; i < 7; i++)
            {
                var currentDay = weekStartDate.AddDays(i);
                var dayHeaderStack = new VerticalStackLayout
                {
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Spacing = 2
                };

                var numberLabel = new Label
                {
                    Text = currentDay.Day.ToString(),
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 14,
                    HorizontalOptions = LayoutOptions.Center,
                    TextColor = Color.FromHex("#1E1E5A")
                };

                var nameLabel = new Label
                {
                    Text = currentDay.ToString("ddd"), // "Mon", "Tue", etc.
                    FontSize = 12,
                    HorizontalOptions = LayoutOptions.Center,
                    TextColor = Color.FromHex("#1E1E5A")
                };

                // Highlight today's date to make it stand out
                if (currentDay.Date == DateTime.Today)
                {
                    numberLabel.TextColor = Colors.Red;
                    nameLabel.TextColor = Colors.Red;
                }

                dayHeaderStack.Children.Add(numberLabel);
                dayHeaderStack.Children.Add(nameLabel);

                Grid.SetColumn(dayHeaderStack, i + 1); // Set column index from 1 to 7
                DayHeaderGrid.Children.Add(dayHeaderStack);
            }
        }

        /// <summary>
        /// Updates the central navigation label to show either "This Week" or a date range.
        /// </summary>
        private void UpdateWeekLabel()
        {
            var todayStartOfWeek = GetStartOfWeek(DateTime.Today);
            if (_currentWeekStartDate.Date == todayStartOfWeek.Date)
            {
                WeekLabel.Text = "This Week";
            }
            else
            {
                var endDate = _currentWeekStartDate.AddDays(6);
                WeekLabel.Text = $"{_currentWeekStartDate:MMM d} - {endDate:MMM d}";
            }
        }

        /// <summary>
        /// Calculates the start date (Monday) of the week for a given date.
        /// </summary>
        private DateTime GetStartOfWeek(DateTime date)
        {
            int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return date.AddDays(-1 * diff).Date;
        }

        /// <summary>
        /// Converts a DayOfWeek enum to the corresponding grid column index (1-7).
        /// </summary>
        private int GetDayColumn(DayOfWeek dayOfWeek)
        {
            return dayOfWeek switch
            {
                DayOfWeek.Monday => 1,
                DayOfWeek.Tuesday => 2,
                DayOfWeek.Wednesday => 3,
                DayOfWeek.Thursday => 4,
                DayOfWeek.Friday => 5,
                DayOfWeek.Saturday => 6,
                DayOfWeek.Sunday => 7,
                _ => -1, // Should not happen
            };
        }

        #endregion
    }
}