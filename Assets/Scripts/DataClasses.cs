using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using Unity.VisualScripting;
public class Event
{
	public int eventId;
	public int layer;
	public string type;
	public char axis;
	public int fiberId;
	public double x_y;
	public double z;
	public double energy;
	public double intensity;
	public double time;

	public Event(string input)
	{
        string[] data = input.Split(" ");
		eventId = Int32.Parse(data[0]);
		layer = Int32.Parse(data[1]);
		type = data[2];
		axis = Char.Parse(data[3]);
        fiberId = Int32.Parse(data[4]);
        x_y = Double.Parse(data[5]);
        z = Double.Parse(data[6]);
        energy = Double.Parse(data[7]);
        intensity = Double.Parse(data[8]);
        time = Double.Parse(data[9]);
    }
}

public class GammaRay
{
	public int eventId;
	public string type;
	public Vector3 start;
	public Vector3 direction;
	public double energy;

	public GammaRay(string input)
	{
        string[] data = input.Split(" ");
		eventId = Int32.Parse(data[0]);
		type = data[1];
		start.x = float.Parse(data[2]);
		start.y = float.Parse(data[4]); // y in our basis is z
		start.z = float.Parse(data[3]);
		direction.x = float.Parse(data[5]);
		direction.y = float.Parse(data[7]); // y in our basis is z
		direction.z = float.Parse(data[6]);
        energy = Double.Parse(data[7]);
    }
}


public class Cone
{
	public int eventId;
	public string type;
	public Vector3 center;
	public double eta;
	public double deta;
	
	public Cone(string input)
	{
        string[] data = input.Split(" ");
		eventId = Int32.Parse(data[0]);
		type = "Gamma_Cone";
		center.x = float.Parse(data[1]);
		center.y = float.Parse(data[3]); // y in our basis is z
		center.z = float.Parse(data[2]);
		eta = float.Parse(data[4]);
		deta = float.Parse(data[5]);
    }
}