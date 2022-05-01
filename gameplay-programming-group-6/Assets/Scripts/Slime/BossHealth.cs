using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BossHealth : MonoBehaviour
{
    public SlimeBoss player;
    public Image fill;
    private Slider slider;
    public float maxHealth = 30f;
    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        float fillValue = player.health / maxHealth;
        slider.value = fillValue;

    }
}
