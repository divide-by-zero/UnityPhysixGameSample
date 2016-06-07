using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrawLine2 : MonoBehaviour
{
    private LineRenderer lr;
    private int cnt;
    // Use this for initialization
	void Start ()
	{
	    lr = GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if (Input.GetMouseButton(0))
	    {
	        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
	        pos.z = 0;
            lr.SetVertexCount(cnt+1);
            lr.SetPosition(cnt++,pos);
	    }
	}
}
