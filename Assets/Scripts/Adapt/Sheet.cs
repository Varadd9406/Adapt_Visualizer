using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheet : MonoBehaviour
{
    // Start is called before the first frame update
    public List<GameObject> wireList;

    void Start()
    {
        wireList = new List<GameObject>();
        foreach (Transform t in transform)
        {
            if (t.gameObject.GetComponent<Wire>() != null)
            {
                wireList.Add(t.gameObject);
            }
        }
        //Debug.Log(wireList.Count);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetWires(int wireIdx,double intensity)
    {
        wireList[wireIdx].GetComponent<Wire>().SetPhotons(intensity);
    }

}
