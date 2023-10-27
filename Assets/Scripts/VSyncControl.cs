using System;
using UnityEngine;

public class VSyncControl : MonoBehaviour
{
    public int VSyncCount = 0;

    private void Awake()
    {
        QualitySettings.vSyncCount = VSyncCount;
    }
    
    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.V)) return;
        // Toggle VSync between 0, 1 and 2
        // 0 = Off - 1 = Every V Blank - 2 = Every Second V Blank
        VSyncCount = (VSyncCount + 1) % 3;
        ChangeVSyncCount(VSyncCount);
    }
    
    public void ChangeVSyncCount(int value)
    {
        VSyncCount = value;
        QualitySettings.vSyncCount = VSyncCount;
    }
}
