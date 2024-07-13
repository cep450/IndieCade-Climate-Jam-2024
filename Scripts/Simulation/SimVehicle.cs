using Godot;
using System;
using System.Collections.Generic;

public partial class SimVehicle : Node
{
	/* 
	 *	A single vehicle.
	 */

	public SimVehicleType VehicleType { get; private set; }
	public Vector2 CurrentPosition { get; private set; }
	public Vector2 TargetPosition { get; private set; }
	public bool IsInUse { get; private set; }

	public float Emissions { get => VehicleType.Emissions; private set {} }

	public SimVehicle(SimVehicleType vehicleType, Vector2 startPosition)
	{
		VehicleType = vehicleType;
		CurrentPosition = startPosition;
		IsInUse = false;
		Emissions = vehicleType.Emissions;
	}

	public override void _Ready()
	{
		// Initialize the vehicle here if needed
	}

	public override void _Process(double delta)
	{
		if (IsInUse)
		{
			UpdateVehiclePosition((float)delta);
		}
	}

	public void Tick() {
		if(IsInUse) {
			SimEmissionsMeter.Instance.AddEmissions(Emissions);
		}
	}

	public void UpdateVehiclePosition(float delta)
	{
		if (CurrentPosition != TargetPosition)
		{
			// Move towards the target position, assuming a simple step move for now
			Vector2 direction = (TargetPosition - CurrentPosition).Normalized();
			float step = VehicleType.SpeedFactor * delta;
			CurrentPosition += direction * step;

			// If the step overshoots the target, set the position to the target
			if (CurrentPosition.DistanceTo(TargetPosition) < step)
			{
				CurrentPosition = TargetPosition;
				IsInUse = false; // Reached the target, stop the vehicle
			}
		}
	}

	public void SetTarget(Vector2 target)
	{
		TargetPosition = target;
		IsInUse = true;
	}

	public float CalculateWeight(SimEdge edge)
	{
		if (!VehicleType.SupportedEdges.Contains(edge.Mode))
		{
			return float.MaxValue;
		}

		float baseWeight = edge.BaseWeight;
		float speedFactor = VehicleType.SpeedFactor;
		float safetyFactor = edge.GetSafetyFactor(); // Assuming this is a method in SimEdge

		return baseWeight / speedFactor * safetyFactor;
	}

}
