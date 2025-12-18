using Godot;
using System;

public partial class ServerRack : Node3D
{
	[Export] public Label3D ScreenFrequency;
	[Export] public Label3D ScreenInfo;
	[Export] public MeshInstance3D ScreenImage;
	[Export] public Label3D ScreenDecoder;

	private float currentFrequency = 90.0f;
	private float scrollSensitivity = 0.5f;

	public override void _Ready()
	{
		UpdateScreens();
	}

	public void AdjustFrequency(float direction)
	{
		currentFrequency += direction * scrollSensitivity;
		
		currentFrequency = (float)Math.Round(currentFrequency, 1);
		
		currentFrequency = Mathf.Clamp(currentFrequency, 0f, 200f);

		UpdateScreens();
	}

	private void UpdateScreens()
	{
		if (ScreenFrequency != null)
		{
			ScreenFrequency.Text = $"{currentFrequency:F1} MHz";
		}
	}
}
