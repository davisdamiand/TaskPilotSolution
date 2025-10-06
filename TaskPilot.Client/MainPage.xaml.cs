using Shared.DTOs;
using System.Buffers.Text;
using System.Net.Http.Json;
using System.Threading.Tasks;
namespace TaskPilot.Client
{
    public partial class MainPage : ContentPage
    {
        private int studentID;

        private readonly HttpClient _httpClient;

        
        public MainPage()
        {
            InitializeComponent();

            _httpClient = new HttpClient
            {
                //Stored in a common config class
                BaseAddress = new Uri(Config.BaseUrl)
            };
        }

        public async void OnRegisterClicked(object sender, EventArgs e)
        {
            //Get the values form the Page
            string name = EntryName.Text?.Trim();
            string email = EntryEmail.Text?.Trim();
            string password = EntryPassword.Text;
            DateOnly dob = DateOnly.FromDateTime((DateTime)DatePickerDOB.Date);
            string surname = EntrySurname.Text?.Trim();
            

            /*Implement validation*/

            //Assign the values to the attributes of the StudentCreateDto
            var student = new StudentCreateDto
            {
                Name = name,
                Surname = surname,
                Email = email,
                Password = password,
                DOB = dob
            };

            //Rend the student object to the controller 
            // Wait for a respond from the controller

           await CreateStudentAsync(student);
            
        }

        private async void OnPageLoad(object sender, EventArgs e)
        {
            var storedID = Preferences.Get("UserID", null);
            if (!string.IsNullOrEmpty(storedID))
            {
                await Shell.Current.GoToAsync("//Home");
            }
        }

        private async Task CreateStudentAsync(StudentCreateDto studentCreateDto)
        {
            try
            {
                // Add this before making the request

                var response = await _httpClient.PostAsJsonAsync("api/Student/CreateStudent", studentCreateDto);


                await DisplayAlertAsync("Debug", $"Status Code: {response.StatusCode}", "OK");

                if (response.IsSuccessStatusCode)
                {
                    var id = await response.Content.ReadFromJsonAsync<int>();

                    //Store the ID of the created student locally for quick logins 
                    Preferences.Set("UserID", id.ToString());

                    //Navigate to the home page
                    await Shell.Current.GoToAsync("//Home");
                }
                else
                {

                    throw new Exception(await response.Content.ReadAsStringAsync());

                }
            }
            catch (Exception ex)
            {

                await DisplayAlertAsync("Error", $"{ex.Message}", "OK");
            }
        }
    }
}
