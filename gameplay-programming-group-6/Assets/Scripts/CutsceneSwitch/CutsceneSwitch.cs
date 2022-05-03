using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneSwitch : MonoBehaviour
{

    public GameObject TimelineObject;
    public PlayableDirector director;

    public GameObject BossObject;
    public PlayableDirector BossDirector;

    public Camera CutsceneCamera;
    public Camera PlayerCamera;
    public bool used = false;
    public float timelineduration;
    PlayerControls playerControls;
    PlayerControls camControls;
    PlayerControls animatorControls;

    public int SwitchCount;
    void Start()
    {
        director = TimelineObject.GetComponent<PlayableDirector>();
        playerControls = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().controls;
        camControls = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CamControls>().controls;
        animatorControls = GameObject.FindGameObjectWithTag("Player").GetComponent<AnimatorController>().controls;
        CutsceneCamera.enabled = false;
        SwitchCount = 0;
    }

    public void PlayTimeline()
    {
        CutsceneCamera.enabled = true;
        PlayerCamera.enabled = false;
        director.Play();
        playerControls.Disable();
        camControls.Disable();
        animatorControls.Disable();
        StartCoroutine(TimeDelay());

        if (SwitchCount == 3)
        {
            BossTimeline();
        }
    }

    public void BossTimeline()
    {
        BossDirector.Play();
        StartCoroutine(BossDelay());
    }

    IEnumerator TimeDelay()
    {
        yield return new WaitForSeconds(timelineduration);
        PlayerCamera.enabled = true;
        CutsceneCamera.enabled = false;
        playerControls.Enable();
        camControls.Enable();
        animatorControls.Enable();
    }

    IEnumerator BossDelay()
    {
        yield return new WaitForSeconds(4);
        PlayerCamera.enabled = true;
        CutsceneCamera.enabled = false;
    }
}
