using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneSwitch : MonoBehaviour
{

    public GameObject TimelineObject;
    public PlayableDirector director;
    public Camera CutsceneCamera;
    public Camera PlayerCamera;
    public bool used = false;
    public float timelineduration;

    void Awake()
    {
        director = TimelineObject.GetComponent<PlayableDirector>();
        CutsceneCamera.enabled = false;
    }

    public void PlayTimeline()
    {
        CutsceneCamera.enabled = true;
        PlayerCamera.enabled = false;
        director.Play();
        StartCoroutine(TimeDelay());
    }

    IEnumerator TimeDelay()
    {
        yield return new WaitForSeconds(24);
        PlayerCamera.enabled = true;
        CutsceneCamera.enabled = false;
    }
}
