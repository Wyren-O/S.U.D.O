using Godot;
using System;

public partial class PlayerHead : Camera3D
{
	[Export] public float MouseSensitivity = 0.003f;
	[Export] public float MaxLookLeftRight = 100.0f;
	[Export] public float MaxLookUpDown = 50.0f;    

	private float _rotationX = 0f; 
	private float _rotationY = 0f; 

	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _Input(InputEvent @event)
	{
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
	
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ui_cancel")) 
		{
			GetTree().Quit();
		}
	}
}
