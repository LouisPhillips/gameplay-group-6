using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBoost : MonoBehaviour
{
    bool collided;
    public PlayerControls2 player;
    public GameObject bubble;
    public bool collected;
    public float time = 0f;
    // Start is called before the first frame update
    void Start()
    {
        bubble.GetComponent<MeshRenderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!collected)
        {
            if (collided)
            {
                collected = true;
            }
        }



        if (collected)
        {
            time += Time.deltaTime;
            if (time < 10)
            {

                bubble.GetComponent<MeshRenderer>().enabled = true;
                player.canShieldBoost = true;
                bubble.gameObject.transform.position = new Vector3(player.gameObject.transform.position.x, player.gameObject.transform.position.y + 1.2f, player.gameObject.transform.position.z);
                player.takeNoDamage = true;
            }
            if (time > 10)
            {
                bubble.GetComponent<MeshRenderer>().enabled = false;
                player.canShieldBoost = false;
                player.takeNoDamage = false;
            }
            if (time < 15)
            {
                GetComponent<MeshRenderer>().enabled = false;
            }
            else
            {
                GetComponent<MeshRenderer>().enabled = true;
                collected = false;
                collided = false;
                time = 0f;
            }
        }

    }

    private void OnTriggerEnter(Collider player)
    {
        if (player.CompareTag("Player"))
        {
            collided = true;
        }
    }
}
