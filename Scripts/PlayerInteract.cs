using Godot;
using System;

public partial class PlayerInteract : Node3D
{
	[Export] public RayCast3D InteractionRay;

	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _Process(double delta)
	{
		if (Input.IsMouseButtonPressed(MouseButton.Left))
		{
			if (InteractionRay.IsColliding())
			{
				var collider = InteractionRay.GetCollider();

				var monitor = collider as MonitorInteract;

				if (monitor != null)
				{
					monitor.Interact();
				}
			}
		}
	}
}
