using Godot;
using System;

public partial class SimClock : Node
{

	/*
	 *	Keep track of simulation clock. Allows for pausing and changing timescale. Controls when simulation ticks happen.
	 */

	private double _timeCounter;
	private double _secondsPerTick = 1.0; //how much real time passes per simulation tick
	private double[] _timeScales = {1.0, 2.0, 3.0}; 	// for speeding up or slowing the game 
	private int _timeScaleIndex = 0;
	private bool _paused = false;
	private int _ticksElapsed;

	public int Tick { get => _ticksElapsed; private set{} }
	

	public override void _Process(double delta)
	{
		if(!_paused) {
			_timeCounter += delta * _timeScales[_timeScaleIndex];
			if(_timeCounter >= _secondsPerTick) {
				_timeCounter = _timeCounter - _secondsPerTick;
				_ticksElapsed++;

				Sim.Instance.SimulationTick();
				Sim.Instance.VisualTick();
				
				GD.Print("tick");
			}
		}

		//TODO if we want to account for lag/dropping ticks/queueing ticks/ect 

	}

	public void TogglePause() {
		if(_paused) {
			UnPause();
		} else {
			Pause();
		}
	}

	public void Pause() {
		_paused = true;
	}

	public void UnPause() {
		_paused = false;
	}

	public void SpeedUpTime() {
		if(_paused) {
			UnPause();
			_timeScaleIndex = 0;
		}
		_timeScaleIndex = Math.Min(_timeScaleIndex + 1, _timeScales.Length);
	}
	public void SlowDownTime() {
		if(_timeScaleIndex <= 0) {
			Pause();
		} else {
			_timeScaleIndex--;
		}
	}
}
