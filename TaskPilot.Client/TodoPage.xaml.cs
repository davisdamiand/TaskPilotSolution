
using Shared.DTOs;
using System.Net.Http.Json;

namespace TaskPilot.Client;

public partial class TodoPage : ContentPage
{
    public TodoPage(TodoViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}