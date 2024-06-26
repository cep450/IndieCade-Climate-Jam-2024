using Godot;
using System;

public class Sim {


    /*
     * 
     */

    // used to determine what can use what types of connections 
    public enum TransitType {
        PEDESTRIAN = 0,
        BICYCLE = 1,
        CAR = 2
    }

    SimGrid grid;
    SimEmissionsMeter emissionsMeter;
    SimSupportPool supportPool;
    List<SimAgent> agents;

}