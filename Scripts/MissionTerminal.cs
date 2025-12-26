using Godot;
using System;
using System.Text.Json;

public class MissionData
{
	public int id { get; set; }
	public string title { get; set; }
	public string description { get; set; }
	public float frequency { get; set; }
	public float file_size { get; set; }
	public string mission_content { get; set; }
	public string mission_type { get; set; }
}

public partial class MissionTerminal : Control
{
	[Export] public Label MissionText;
	[Export] public Button RequestButton;
	[Export] public HttpRequest Http; 
	[Export] public ServerRack RackScript; 

	private string _getUrl = "http://127.0.0.1:8000/missions/next";
	private string _completeUrlBase = "http://127.0.0.1:8000/missions/";

	private int _currentMissionId = -1;
	private bool _hasActiveMission = false;

	public override void _Ready()
	{
		RequestButton.Pressed += OnButtonPressed;
		Http.RequestCompleted += OnRequestCompleted;
		
		ResetTerminal();
	}

	private void ResetTerminal()
	{
		_hasActiveMission = false;
		_currentMissionId = -1;
		MissionText.Text = "SYSTEM READY.\nWAITING FOR NEW ASSIGNMENT...";
		RequestButton.Text = "DOWNLOAD MISSION"; 
		RequestButton.Disabled = false;
	}

	private void OnButtonPressed()
	{
		if (!_hasActiveMission)
		{
			MissionText.Text = "CONNECTING TO HQ...";
			RequestButton.Disabled = true; 
			Http.Request(_getUrl);
		}
		else
		{
			CheckAndSubmitMission();
		}
	}

	private void CheckAndSubmitMission()
	{
		if (RackScript == null) 
		{
			MissionText.Text = "ERROR: RACK NOT CONNECTED";
			return;
		}

		if (RackScript.IsSignalReady)
		{
			MissionText.Text = "UPLOADING DATA...";
			RequestButton.Disabled = true;
			
			string url = $"{_completeUrlBase}{_currentMissionId}/complete";
			string[] headers = { "Content-Type: application/json" };
			Http.Request(url, headers, HttpClient.Method.Post, "{}");
		}
		else
		{
			MissionText.Text += "\n\nERROR: SIGNAL NOT LOCKED!\nTUNE FREQUENCY FIRST.";
		}
	}

	private void OnRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		RequestButton.Disabled = false;

		if (responseCode == 200)
		{
			if (_hasActiveMission && RequestButton.Text == "SUBMIT DATA") 
			{
				MissionText.Text = "MISSION COMPLETE!\nDATA UPLOADED SUCCESSFULLY.\nGOOD JOB.";
				_hasActiveMission = false; 
				RequestButton.Text = "NEW MISSION";
				return;
			}

			var jsonString = System.Text.Encoding.UTF8.GetString(body);
			try 
			{
				MissionData mission = JsonSerializer.Deserialize<MissionData>(jsonString);
				
				_currentMissionId = mission.id;
				_hasActiveMission = true;

				MissionText.Text = $"TARGET: {mission.title}\n" +
								   $"TYPE: {mission.mission_type}\n" +
								   $"FREQ: {mission.frequency} MHz\n" +
								   $"DESC: {mission.description}";

				RequestButton.Text = "SUBMIT DATA"; 

				if (RackScript != null)
				{
					RackScript.SetMissionData(mission.mission_type, mission.frequency, mission.mission_content);
				}
			}
			catch (Exception e)
			{
				MissionText.Text = "DATA CORRUPTED";
				GD.PrintErr(e.Message);
			}
		}
		else
		{
			MissionText.Text = $"SERVER ERROR: {responseCode}";
		}
	}
}
