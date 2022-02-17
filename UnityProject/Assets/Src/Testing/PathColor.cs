using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathColor : MonoBehaviour
{
    public GameObject[] paths;
    public SO.ColorSpec[] colors;
    // Start is called before the first frame update 
    void Start()
    {
        ChangeColor();
    }

    void ChangeColor(){
        for (int k = 0; k < paths.Length; k++){
            for(int i = 0; i < paths[k].gameObject.transform.childCount; i++)
            {
                for(int j = 0; j < paths[k].gameObject.transform.GetChild(i).childCount; j++)
                {
                    paths[k].gameObject.transform.GetChild(i).GetChild(j).gameObject.GetComponent<Renderer>().materials[0].SetColor("_Color", colors[k].color);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
