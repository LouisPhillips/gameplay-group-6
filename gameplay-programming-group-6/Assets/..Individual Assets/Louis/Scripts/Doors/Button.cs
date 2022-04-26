using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public PlayerControls2 player;
    public Transform button;
    public PlayerControls controls;
    public Animator CutSceneAnimation;
    private string cutscene = "CutsceneCamera";

    public bool inside;
    // Start is called before the first frame update
    void Awake()
    {
        controls = new PlayerControls();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        inside = true;
        player.canPressButton = true;
        player.buttonReminder.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        inside = false; 
        player.canPressButton = false;
        player.buttonReminder.SetActive(false);
    }
}
