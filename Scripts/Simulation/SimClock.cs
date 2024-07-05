using Godot;
using System;

public partial class SimClock : Node
{

	/*
	 *	Keep track of simulation clock. Allows for pausing and changing timescale. Controls when simulation ticks happen.
	 */

	private float _currentTime;
	private float _timeStep = 1.0f;


	private bool paused = false;


	public override void _Process(double delta)
	{

		if(!paused) {
			//_currentTime += delta * _timeStep; //TODO
		}
		

	}



	public float GetCurrentTime()
	{
		return _currentTime;
	}


	public void TogglePause() {
		if(paused) {
			UnPause();
		} else {
			Pause();
		}
	}

	public void Pause() {
		//TODO
	}

	public void UnPause() {
		//TODO 
	}
}
