using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using Unity.VisualScripting;
using UnityEditor;
using Unity.Burst.Intrinsics;


public class Detector : MonoBehaviour
{
	// Use this for initialization
	public List<GameObject> layersList;
	public GameObject domeRef;
	private Dictionary<int, List<Event>> fiberData;
	
	private Dictionary<int, GammaRay> gammaRays;

	private Dictionary<int, List<Cone>> coneData;
	private GameObject coneRef;

	public bool enableDome;
	public bool initDome;

	void Start()
	{
        layersList = new List<GameObject>();
        foreach (Transform t in transform)
        {
			if(t.gameObject.GetComponent<Layer>() != null)
			{
                layersList.Add(t.gameObject);
            }
        }

        fiberData = new Dictionary<int, List<Event>>();
		ReadFiberData("Assets/Data/fiber.txt");
		ReadConeData("Assets/Data/cone.txt");
		SetEvent(7);
		InitCone();
		SetCone(57);
		enableDome = true;
		initDome = false;
    }

	// Update is called once per frame
	void Update()
	{
		if(enableDome && !initDome)
		{
			domeRef.GetComponent<Dome>().SetHeatmap(coneData);
			initDome = true;
		}
	}
	void ReadFiberData(string filepath)
	{
		StreamReader sr = new StreamReader(filepath);
		sr.ReadLine();
		string line = sr.ReadLine(); //  ignore first line
		int lastDataIdx = -1;
		List<Event> currList = new List<Event>();
		Event currEvent;
		fiberData = new Dictionary<int, List<Event>>();

		while(line!=null)
		{
			currEvent = new Event(line);
			int dataIdx = currEvent.eventId;
			if(lastDataIdx != dataIdx && currList.Count!=0)
			{
                fiberData.Add(lastDataIdx,currList);
				currList = new List<Event>();
            }
			lastDataIdx = dataIdx;
			currList.Add(currEvent);

			line = sr.ReadLine();
		}
		if(currList.Count!=0)
		{
			fiberData.Add(lastDataIdx,currList);
		}
		Debug.Log(fiberData.Count);
		sr.Close();
	}
	
	void ReadGammaRayData(string filepath)
	{
		StreamReader sr = new StreamReader(filepath);
		string line = sr.ReadLine();
		GammaRay currRay;

		while(line!=null)
		{
			currRay = new GammaRay(line);
			int dataIdx = currRay.eventId;
			gammaRays.Add(dataIdx,currRay);

			line = sr.ReadLine();
		}
		// Debug.Log(fiberData.Count);
		sr.Close();
	}

	void ReadConeData(string filepath)
	{
		StreamReader sr = new StreamReader(filepath);
		string line = sr.ReadLine();
		int lastDataIdx = -1;
		List<Cone> currList = new List<Cone>();
		Cone currCone;
		coneData = new Dictionary<int, List<Cone>>();

		while(line!=null)
		{
			currCone = new Cone(line);
			int dataIdx = currCone.eventId;
			if(lastDataIdx != dataIdx && currList.Count!=0)
			{
                coneData.Add(lastDataIdx,currList);
				currList = new List<Cone>();
            }
			lastDataIdx = dataIdx;
			currList.Add(currCone);

			line = sr.ReadLine();
		}
		if(currList.Count!=0)
		{
			coneData.Add(lastDataIdx,currList);
		}
		Debug.Log(coneData.Count);
		sr.Close();
	}
	
	void SetEvent(int idx)
	{
		for(int i = 0;i<fiberData[idx].Count;i++)
		{
			Event curr = fiberData[idx][i];
			if(curr.type == "WLS_Slow")
			{
				SetLayer(curr.layer,"WLS_Sheet_" + curr.axis.ToString().ToUpper(),curr.fiberId,curr.intensity);
			}
			else if(curr.type == "TKR")
			{
				SetLayer(curr.layer,"TKR_Sheet_" + curr.axis.ToString().ToUpper(),curr.fiberId,curr.intensity);
			}
		}
	}

		
	void SetLayer(int layer,string sheetName,int wireIdx,double intensity)
	{
		layersList[layer].GetComponent<Layer>().SetSheet(sheetName, wireIdx, intensity);
	}
	
	void InitCone()
	{
		UnityEngine.Object temp = Resources.Load("Prefabs/Cone");
		if(temp==null)
        {
            Debug.Log("Cone prefab not found");
        }
		coneRef = (GameObject)Instantiate(temp);
		coneRef.transform.SetPositionAndRotation(new Vector3(0, 0, 0), Quaternion.Euler(90, 0, 0));
	}
	
	void SetCone(int idx)
	{
		Vector3 A = new Vector3(0,-1,0);
		Vector3 B = coneData[idx][0].center;
		B = Vector3.Normalize(B);
	

		float dotprod = Vector4.Dot(A,B);
		float crossprod =(float) Math.Sqrt(Vector3.Cross(A,B).sqrMagnitude);

		Matrix4x4 G = Matrix4x4.zero;
		G[0,0] = dotprod;
		G[0,1] = -crossprod;
		G[1,0] = crossprod;
		G[1,1] = dotprod;
		G[2,2] = 1;
		G[3,3] = 1;

		Debug.Log(G);
		Vector3 v = Vector3.Normalize(B -dotprod*A);

		Vector3 w = Vector3.Cross(B,A);

		Matrix4x4 F = Matrix4x4.zero;

		F[0,0] = A[0];
		F[0,1] = A[1];
		F[0,2] = A[2];

		F[1,0] = v[0];
		F[1,1] = v[1];
		F[1,2] = v[2];

		F[2,0] = w[0];
		F[2,1] = w[1];
		F[2,2] = w[2];
		
		F[3,3] = 1;
		
		Matrix4x4 U = Matrix4x4.Inverse(F)*(G*F);

		coneRef.transform.localScale =  new Vector3(1,2,1);
		coneRef.transform.rotation = RotationMatrixToQuaternion(U);
	}



	public static Quaternion RotationMatrixToQuaternion(Matrix4x4 m)
        {
            // Adapted from: http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/index.htm
            Quaternion q = new Quaternion();
            q.w =Mathf.Sqrt(Mathf.Max(0,1+ m[0,0]+ m[1,1]+ m[2,2]))/2;
            q.x =Mathf.Sqrt(Mathf.Max(0,1+ m[0,0]- m[1,1]- m[2,2]))/2;
            q.y =Mathf.Sqrt(Mathf.Max(0,1- m[0,0]+ m[1,1]- m[2,2]))/2;
            q.z =Mathf.Sqrt(Mathf.Max(0,1- m[0,0]- m[1,1]+ m[2,2]))/2;
            q.x *=Mathf.Sign(q.x *(m[2,1]- m[1,2]));
            q.y *=Mathf.Sign(q.y *(m[0,2]- m[2,0]));
            q.z *=Mathf.Sign(q.z *(m[1,0]- m[0,1]));
            return q;
        }
 
}