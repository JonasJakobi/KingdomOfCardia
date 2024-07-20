using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsistentSettings : SingletonPersistent<ConsistentSettings>
{
    public static float generalVolume = 1f;

    public static float musicVolume = 0.2f;

    public static float sfxVolume = 1f;

    public static bool tutorialEnabled = true;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
