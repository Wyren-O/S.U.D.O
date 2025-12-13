using Godot;
using System;

public partial class MonitorInteract : Area3D 
{
	[Export] public Camera3D MainCamera; 
	[Export] public Node3D ZoomTarget;
	[Export] public float TransitionTime = 1.0f;

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
		
		Input.MouseMode = Input.MouseModeEnum.Visible; 

		Tween tween = CreateTween();
		tween.SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.InOut);
		tween.TweenProperty(MainCamera, "global_transform", ZoomTarget.GlobalTransform, TransitionTime);
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

		Tween tween = CreateTween();
		tween.SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.InOut);
		tween.TweenProperty(MainCamera, "global_transform", originalCameraTransform, TransitionTime);
	}
}
