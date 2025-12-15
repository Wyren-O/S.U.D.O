using Godot;
using System;
using System.Text.Json;

public class MissionData
{
	public string title { get; set; }
	public string description { get; set; }
	public float frequency { get; set; }
	public float file_size { get; set; }
	public int id { get; set; }
}

public partial class MissionTerminal : Control
{
	[Export] public Label MissionText;
	[Export] public Button RequestButton;
	[Export] public HttpRequest Http; 

	private string _serverUrl = "http://127.0.0.1:8000/missions/next";

	public override void _Ready()
	{
		RequestButton.Pressed += OnRequestButtonPressed;
		
		Http.RequestCompleted += OnRequestCompleted;
		
		MissionText.Text = "SYSTEM READY. WAITING FOR TASKS...";
	}

	private void OnRequestButtonPressed()
	{
		MissionText.Text = "CONNECTING TO HQ...";
		RequestButton.Disabled = true; 

		Error err = Http.Request(_serverUrl);
		if (err != Error.Ok)
		{
			GD.PrintErr("Connection failed!");
			MissionText.Text = "ERROR: CONNECTION FAILED";
			RequestButton.Disabled = false;
		}
	}

	private void OnRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		RequestButton.Disabled = false;

		if (responseCode == 200)
		{
			string jsonString = System.Text.Encoding.UTF8.GetString(body);
			
			try 
			{
				MissionData mission = JsonSerializer.Deserialize<MissionData>(jsonString);
				
				MissionText.Text = $"--- NEW OBJECTIVE ---\n\n" +
								   $"CODE: {mission.title}\n" +
								   $"FREQ: {mission.frequency} MHz\n" +
								   $"SIZE: {mission.file_size} KB\n\n" +
								   $"DESC: {mission.description}";
			}
			catch (Exception e)
			{
				MissionText.Text = "ERROR: CORRUPTED DATA PACKET";
				GD.PrintErr(e.Message);
			}
		}
		else
		{
			MissionText.Text = $"SERVER ERROR: {responseCode}";
		}
	}
}
