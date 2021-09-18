using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class map : MonoBehaviour
{
    public static map mapS;  //map class singleton

    [SerializeField] public GameObject[] hexes = new GameObject[20];

    [SerializeField] public GameObject gBar1;
    [SerializeField] public GameObject gBar2;
    [SerializeField] public GameObject gBar3;

    [SerializeField] public GameObject hexInfoPanel;



    private void Awake()
    {
        mapS = this;
    }

}
