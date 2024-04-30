using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserPreferences : MonoBehaviour
{
    public static UserPreferences Instance;

    public string _username;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    void Start()
    {
        Screen.autorotateToPortrait = true;

        Screen.autorotateToPortraitUpsideDown = false;

        Screen.autorotateToLandscapeLeft = false;

        Screen.autorotateToLandscapeRight = false;

        Screen.orientation = ScreenOrientation.Portrait;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
