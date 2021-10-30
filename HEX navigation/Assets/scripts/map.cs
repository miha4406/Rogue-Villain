using UnityEngine;

public class map : MonoBehaviour
{
    public static map mapS;  //map class singleton

    [SerializeField] public GameObject[] hexes = new GameObject[20];

    public GameObject gBar1;
    public GameObject gBar2;
    public GameObject gBar3;

    [SerializeField] public GameObject hexInfoPanel;



    private void Awake()
    {
        mapS = this;
    }

}
