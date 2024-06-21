using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dome : MonoBehaviour
{
    // Start is called before the first frame update
    float[] coneShaderData;
	float coneCount;
	

	Material mMaterial;
  	MeshRenderer mMeshRenderer;
    void Start()
    {
        
        mMeshRenderer = GetComponent<MeshRenderer>();
    	mMaterial = mMeshRenderer.material;
		Debug.Log("Initialized Dome");

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetHeatmap(Dictionary<int,List<Cone>> coneData)
	{
		coneShaderData = new float[32*4];
		coneCount = 2;
		int i = 0;
		foreach(var item in coneData.Values)
		{
			if(i>coneCount)
			{
				break;
			}
			coneShaderData[i*4] = item[0].center[0];
			coneShaderData[i*4+ 1] = item[0].center[1];
			coneShaderData[i*4 + 2] = item[0].center[2];
			coneShaderData[i*4 + 3] =  (Mathf.Acos((float)item[0].eta)*Mathf.Rad2Deg)/2;
            Debug.Log(coneShaderData[i*4 + 3]);
			i+=1;
		}
		mMaterial.SetFloatArray("_coneData", coneShaderData);
    	mMaterial.SetFloat("_coneCount", coneCount);
	}
}
