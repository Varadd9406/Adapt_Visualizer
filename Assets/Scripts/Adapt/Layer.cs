using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Layer : MonoBehaviour
{
    public GameObject wlsSheetX, wlsSheetY;
    public GameObject trackerSheetX, tracketSheetY;

    private void Start()
    {

    }
    private void Update()
    {

    }

    public void SetSheet(string sheetType,int wireIdx,double intensity)
    {
        if (sheetType == "WLS_Sheet_X")
        {
            wlsSheetX.GetComponent<Sheet>().SetWires(wireIdx, intensity);
        }
        else if (sheetType == "WLS_Sheet_Y")
        {
            wlsSheetY.GetComponent<Sheet>().SetWires(wireIdx, intensity);
        }
        else if (sheetType == "TKR_Sheet_X")
        {
            trackerSheetX.GetComponent<Sheet>().SetWires(wireIdx, intensity);
        }
        else if (sheetType == "TKR_Sheet_Y")
        {
            tracketSheetY.GetComponent<Sheet>().SetWires(wireIdx, intensity);
        }
        else
        {
            Debug.LogError("Sheet Does not exist");
        }
    }


}

