using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class IntroScript : MonoBehaviour
{
    public Camera CutsceneCamera;
    public Camera PlayerCamera;
    public bool used = false;
    public float timelineduration;


    // Start is called before the first frame update
    void Awake()
    {
        CutsceneCamera.enabled = true;
        PlayerCamera.enabled = false;

        StartCoroutine(TimeDelay());
    }

    IEnumerator TimeDelay()
    {
        yield return new WaitForSeconds(timelineduration);
        PlayerCamera.enabled = true;
        CutsceneCamera.enabled = false;
    }
}
