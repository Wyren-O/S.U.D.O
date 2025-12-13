using Godot;
using System;

public partial class CipherDisplay : Label3D
{
	[Export] public double ChangeInterval = 15.0;
	
	private double _timer = 0;
	private Random _rnd = new Random();
	
	public string CurrentPassword { get; private set; } = "";

	public override void _Ready()
	{
		GenerateNewPassword();
	}

	public override void _Process(double delta)
	{
		_timer -= delta;
		
		if (_timer <= 0)
		{
			GenerateNewPassword();
		}
	}

	private void GenerateNewPassword()
	{
		string letters = "";
		for (int i = 0; i < 5; i++) 
			letters += (char)_rnd.Next('A', 'Z' + 1);

		CurrentPassword = $"{letters}";
		Text = CurrentPassword;
		
		_timer = ChangeInterval;
	}
}
