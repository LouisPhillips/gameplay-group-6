using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPlayer : MonoBehaviour
{
    #region Singleton

    public static GetPlayer call;

    private void Awake()
    {
        call = this;
    }
    #endregion

    public GameObject player;
}
