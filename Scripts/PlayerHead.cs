using Godot;
using System;

public partial class PlayerHead : Camera3D
{
	[Export] public float MouseSensitivity = 0.003f;
	[Export] public float MaxLookLeftRight = 100.0f;
	[Export] public float MaxLookUpDown = 50.0f; 
	[Export] public RayCast3D InteractionRay; 
	
	public bool IsLocked = false;  

	private float _rotationX = 0f; 
	private float _rotationY = 0f; 

	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _Input(InputEvent @event)
	{
		if (IsLocked) return;
		
		if (@event is InputEventMouseMotion motion)
		{
			_rotationY -= motion.Relative.X * MouseSensitivity;
			_rotationX -= motion.Relative.Y * MouseSensitivity;

			float limitY = Mathf.DegToRad(MaxLookLeftRight);
			float limitX = Mathf.DegToRad(MaxLookUpDown);

			_rotationX = Mathf.Clamp(_rotationX, -limitX, limitX);
			_rotationY = Mathf.Clamp(_rotationY, -limitY, limitY);

			Rotation = new Vector3(_rotationX, _rotationY, 0);
		}
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (IsLocked) return;

		if (@event.IsActionPressed("scroll_up") || @event.IsActionPressed("scroll_down"))
		{
			if (InteractionRay != null && InteractionRay.IsColliding())
			{
				var collider = InteractionRay.GetCollider() as Node;
				ServerRack rack = null;

				if (collider is ServerRack r)
				{
					rack = r;
				}
				else if (collider.GetParent() is ServerRack parentRack)
				{
					rack = parentRack;
				}

				if (rack != null)
				{
					float dir = @event.IsActionPressed("scroll_up") ? 1.0f : -1.0f;
					rack.AdjustFrequency(dir);
				}
			}
		}
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ui_cancel")) 
		{
			GetTree().Quit();
		}
	}
}
