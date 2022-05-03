using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class IntroScript : MonoBehaviour
{
    public Camera CutsceneCamera;
    public Camera PlayerCamera;
    public float timelineduration;
    PlayerControls playerControls;
    PlayerControls camControls;
    PlayerControls animatorControls; 

    // Start is called before the first frame update
    void Start()
    {
        playerControls = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().controls;
        camControls = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CamControls>().controls;
        animatorControls = GameObject.FindGameObjectWithTag("Player").GetComponent<AnimatorController>().controls;
        playerControls.Disable();
        camControls.Disable();
        animatorControls.Disable();
        CutsceneCamera.enabled = true;
        PlayerCamera.enabled = false;

        StartCoroutine(TimeDelay());
        playerControls.Enable();
        camControls.Enable();
        animatorControls.Enable();
    }

    IEnumerator TimeDelay()
    {
        yield return new WaitForSeconds(timelineduration);
        PlayerCamera.enabled = true;
        CutsceneCamera.enabled = false;
    }
}
