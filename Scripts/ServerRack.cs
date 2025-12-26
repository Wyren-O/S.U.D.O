using Godot;
using System;
using System.Text;

public partial class ServerRack : Node3D
{
	[ExportGroup("Screens")]
	[Export] public Label3D ScreenFrequency;
	[Export] public Label3D ScreenInfo;
	[Export] public MeshInstance3D ScreenImage;
	[Export] public Label3D ScreenDecoder;     
	
	[ExportGroup("Audio")]
	[Export] public AudioStreamPlayer3D SignalAudioPlayer; 

	private string _missionType = "TEXT";
	private float _targetFrequency = 0.0f;     
	private string _targetContent = "";        
	private bool _hasActiveMission = false;    

	private float _currentFrequency = 90.0f;
	private float _signalStrength = 0.0f;
	private Random _rng = new Random();
	
	public bool IsSignalReady => _signalStrength >= 0.95f;
	
	public override void _Ready()
	{
		if (ScreenImage != null) ScreenImage.Visible = false;
	}

	public void SetMissionData(string type, float freq, string content)
	{
		_missionType = type;
		_targetFrequency = freq;
		_targetContent = content;
		_hasActiveMission = true;
		
		ScreenInfo.Text = "SIGNAL LOST\nTUNE FREQUENCY";
		ScreenInfo.Modulate = Colors.Red;

		ScreenDecoder.Text = "";
		if (ScreenImage != null) ScreenImage.Visible = false;
		if (SignalAudioPlayer != null) SignalAudioPlayer.Stop();

		if (_missionType == "IMAGE")
		{
			LoadImageTexture(content);
		}
		else if (_missionType == "AUDIO")
		{
			LoadAudioClip(content);
		}
	}

	public void AdjustFrequency(float direction)
	{
		_currentFrequency += direction * 0.5f;
		_currentFrequency = (float)Math.Round(_currentFrequency, 1);
		_currentFrequency = Mathf.Clamp(_currentFrequency, 0f, 200f);
		UpdateLogic();
	}

	private void UpdateLogic()
	{
		if (ScreenFrequency != null) ScreenFrequency.Text = $"{_currentFrequency:F1} MHz";

		if (!_hasActiveMission) return;

		float diff = Mathf.Abs(_currentFrequency - _targetFrequency);
		float range = 5.0f;
		if (diff > range) _signalStrength = 0.0f;
		else _signalStrength = 1.0f - (diff / range);

		if (_missionType == "TEXT")
		{
			ProcessTextMission();
		}
		else if (_missionType == "IMAGE")
		{
			ProcessImageMission();
		}
		else if (_missionType == "AUDIO")
		{
			ProcessAudioMission();
		}

		if (_signalStrength >= 0.98f)
		{
			ScreenInfo.Text = "SIGNAL LOCKED";
			ScreenInfo.Modulate = Colors.Green;
		}
		else
		{
			ScreenInfo.Text = "SIGNAL False";
			ScreenInfo.Modulate = Colors.Orange;
		}
	}

	private void ProcessTextMission()
	{
		ScreenImage.Visible = false; 
		
		if (_signalStrength >= 0.97f)
		{
			ScreenDecoder.Text = _targetContent;
			return;
		}

		StringBuilder scrambled = new StringBuilder();
		string noiseChars = "#@$%^&*?~"; 
		foreach (char c in _targetContent)
		{
			if (char.IsWhiteSpace(c)) { scrambled.Append(" "); continue; }
			if (_rng.NextDouble() > _signalStrength) scrambled.Append(noiseChars[_rng.Next(noiseChars.Length)]);
			else scrambled.Append(c);
		}
		ScreenDecoder.Text = scrambled.ToString();
	}

	private void ProcessImageMission()
	{
		if (ScreenImage == null) return;
		
		ScreenDecoder.Text = ""; 
		ScreenImage.Visible = true; 

		float visibility = Mathf.Clamp(_signalStrength, 0.05f, 1.0f);
		
		var mat = ScreenImage.GetActiveMaterial(0) as StandardMaterial3D;
		if (mat != null)
		{
			Color c = mat.AlbedoColor;
			c.A = visibility; 
			mat.AlbedoColor = c;
		}
	}

	private void ProcessAudioMission()
	{
		ScreenImage.Visible = false;
		ScreenDecoder.Text = "AUDIO SIGNAL...";

		if (SignalAudioPlayer == null) return;

		if (_signalStrength > 0.1f)
		{
			if (!SignalAudioPlayer.Playing) SignalAudioPlayer.Play();
			
			float volDb = Mathf.LinearToDb(_signalStrength); 
			SignalAudioPlayer.VolumeDb = volDb;
			
			SignalAudioPlayer.PitchScale = 0.5f + (_signalStrength * 0.5f);
		}
		else
		{
			if (SignalAudioPlayer.Playing) SignalAudioPlayer.Stop();
		}
	}

	private void LoadImageTexture(string fileName)
	{
		string path = $"res://Textures/{fileName}";
		var texture = GD.Load<Texture2D>(path);
		
		if (texture != null)
		{
			var mat = ScreenImage.GetActiveMaterial(0) as StandardMaterial3D;
			if (mat == null)
			{
				mat = new StandardMaterial3D();
				mat.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
				ScreenImage.SetSurfaceOverrideMaterial(0, mat);
			}
			mat.AlbedoTexture = texture;
		}
		else
		{
			GD.PrintErr($"Не нашел картинку: {path}");
		}
	}

	private void LoadAudioClip(string fileName)
	{
		string path = $"res://Audio/{fileName}";
		var stream = GD.Load<AudioStream>(path);
		
		if (stream != null && SignalAudioPlayer != null)
		{
			SignalAudioPlayer.Stream = stream;
		}
		else
		{
			GD.PrintErr($"Не нашел звук: {path}");
		}
	}
}
