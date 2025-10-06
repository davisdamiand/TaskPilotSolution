using Shared.DTOs;
using System.Net.Http.Json;

namespace TaskPilot.Client;

public partial class Home : ContentPage
{
	private readonly HttpClient _httpClient;


	// Application entry point
	public Home()
	{
		InitializeComponent();

		_httpClient = new HttpClient
		{
            //Stored in a common config class
            BaseAddress = new Uri(Config.BaseUrl)
		};
	}

}