﻿using UnityEngine;
using System.Collections;

public class View_IslandSelect : ViewBase {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        
	}

    public void OnClickedIsland()
    {
        FishData.GetInstance().GameBegin = true;
        gameObject.SetActive(false);
    }
}
