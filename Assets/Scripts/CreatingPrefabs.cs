// This script creates a new menu item Examples>Create Prefab in the main menu.
// Use it to create Prefab(s) from the selected GameObject(s).
// It is placed in the root Assets folder.
using System.IO;
using UnityEngine;
using UnityEditor;

public class Example
{
    [MenuItem("Examples/Create WLS Sheet")]
    static void CreateWLSSheet()
    {
        int count = 225;
        Object temp = Resources.Load("Prefabs/WLS_Fiber");
        if(temp==null)
        {
            Debug.Log("Wire prefab not found");
        }
        Vector3 centerOfMass = Vector3.zero;
        for (int i = 0; i < count; i++)
        {
            centerOfMass += (new Vector3(i * 0.2f, 0, 0));
        }
        centerOfMass /= count;

        GameObject parentSheet = new GameObject("WLS_Sheet");
        parentSheet.transform.SetPositionAndRotation(centerOfMass, Quaternion.identity);

        for (int i =0;i<count;i++)
        {
            GameObject idk = (GameObject)PrefabUtility.InstantiatePrefab(temp);
            idk.transform.SetPositionAndRotation(new Vector3(i * 0.2f, 0, 0), Quaternion.identity);
            idk.transform.parent = parentSheet.transform;
        }
        parentSheet.transform.SetPositionAndRotation(new Vector3(0,0,0), Quaternion.identity);
    }
    [MenuItem("Examples/Create Tracker Sheet")]
    static void CreateTrackerSheet()
    {
        int count = 600;
        float radius = 0.075f;
        Object temp = Resources.Load("Prefabs/Tracker_Fiber");
        if (temp == null)
        {
            Debug.Log("Wire prefab not found");
        }
        Vector3 centerOfMass = Vector3.zero;
        bool up = false;
        for (int i = 0; i < count; i++)
        {
            if(up)
            {
                centerOfMass += (new Vector3(i * radius, 0, (Mathf.Sqrt(3.0f) * radius*2) / 2));
            }
            else
            {
                centerOfMass += (new Vector3(i * radius, 0, 0));
            }
            up = !up;
        }
        centerOfMass /= count;

        GameObject parentSheet = new GameObject("Tracker_Sheet");
        parentSheet.transform.SetPositionAndRotation(centerOfMass, Quaternion.identity);
        up = false;
        for (int i = 0; i < count; i++)
        {
            GameObject idk = (GameObject)PrefabUtility.InstantiatePrefab(temp);
            if(up)
            {
                idk.transform.SetPositionAndRotation(new Vector3(i * radius, 0,(Mathf.Sqrt(3.0f) * radius*2) / 2), Quaternion.Euler(0,0,0));
            }
            else
            {
                idk.transform.SetPositionAndRotation(new Vector3(i * radius, 0, 0), Quaternion.Euler(0, 0, 0));
            }
            idk.transform.parent = parentSheet.transform;
            up = !up;
        }
        parentSheet.transform.SetPositionAndRotation(new Vector3(0, 0, 0), Quaternion.Euler(90, 0, 0));
    }
}