using Godot;
using System;

public partial class SimClock : Node
{
	private float _currentTime;
	private float _timeStep = 1.0f;

	public void UpdateTime(float delta)
	{
		_currentTime += delta * _timeStep;
		// Handle turn-based logic here
	}

	public float GetCurrentTime()
	{
		return _currentTime;
	}
}
