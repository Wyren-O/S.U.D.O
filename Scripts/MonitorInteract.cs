using Godot;
using System;

public partial class MonitorInteract : Area3D 
{
	[Export] public Camera3D MainCamera; 
	[Export] public Node3D ZoomTarget;
	[Export] public float TransitionTime = 1.0f;

	[Export] public Control MissionTerminal; 
	[Export] public PlayerHead PlayerHeadScript; 

	private bool isZoomed = false;
	private Transform3D originalCameraTransform;

	public void Interact()
	{
		if (!isZoomed) 
		{
			StartZoom();
		}
	}

	private void StartZoom()
	{
		isZoomed = true;
		originalCameraTransform = MainCamera.GlobalTransform;
		
		if (PlayerHeadScript != null) PlayerHeadScript.IsLocked = true;
		
		Input.MouseMode = Input.MouseModeEnum.Visible; 
		
		if (MissionTerminal != null)
		{
			MissionTerminal.Visible = true;
			MissionTerminal.Modulate = new Color(1, 1, 1, 0); 
		}

		Tween tween = CreateTween();
		tween.SetParallel(true); 
		
		tween.TweenProperty(MainCamera, "global_transform", ZoomTarget.GlobalTransform, TransitionTime)
			 .SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.InOut);
			 
		if (MissionTerminal != null)
		{
			tween.TweenProperty(MissionTerminal, "modulate:a", 10f, 5f)
				 .SetDelay(0.2f); 
		}
	}

	public override void _Process(double delta)
	{
		if (isZoomed)
		{
			if (Input.IsActionJustPressed("ui_cancel") || Input.IsMouseButtonPressed(MouseButton.Right))
			{
				EndZoom();
			}
		}
	}

	private void EndZoom()
	{
		isZoomed = false;
		
		Input.MouseMode = Input.MouseModeEnum.Captured; 
		if (PlayerHeadScript != null) PlayerHeadScript.IsLocked = false;

		if (MissionTerminal != null)
			MissionTerminal.Visible = false;

		Tween tween = CreateTween();
		tween.SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.InOut);
		tween.TweenProperty(MainCamera, "global_transform", originalCameraTransform, TransitionTime);
	}
}
