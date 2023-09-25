using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeCreenCtrl : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        setNotFullScreen();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void setNotFullScreen() {
        Screen.SetResolution(1440, 900, false);
    }
}
