using Godot;
using System;

public partial class SimVehicleType : Node {

    /*
     *  Flyweight pattern: information about a particular type of vehicle, that multiple vehicles will use.
     */ 

    
    public string name;


    public Sim.TransitType transitType; // determines type of connections it can move on 
    public float emissionsPerTick;      // emissions when in use 
    public float maxSpeed;
    public int agentCapacity;           // how many agents can travel in it at a time

}