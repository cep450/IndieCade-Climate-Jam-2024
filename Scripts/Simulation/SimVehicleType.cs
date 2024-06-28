using Godot;
using System;
using System.Collections.Generic;

public partial class SimVehicleType : Node
{

    /*
     *  Flyweight pattern: information about a particular type of vehicle, that multiple vehicles will use.
     */


    public string name;

    public float maxSpeed;
    public int agentCapacity;           // how many agents can travel in it at a time

    public enum TransportMode { Car, Bus, Bike, Pedestrian, Train }

    public TransportMode Mode { get; private set; }
    public float SpeedFactor { get; private set; }
    public float Emissions { get; private set; }
    public HashSet<SimEdge.TransportMode> SupportedEdges { get; private set; }

    public SimVehicleType(TransportMode mode, float speedFactor, float emissions, HashSet<SimEdge.TransportMode> supportedEdges)
    {
        Mode = mode;
        SpeedFactor = speedFactor;
        Emissions = emissions;
        SupportedEdges = supportedEdges;
    }
}
