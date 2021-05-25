using System.Collections;
using UnityEngine;
using System.Linq;

public class goldControl : MonoBehaviour
{
    [SerializeField] GameObject gBar;

    [SerializeField] GameObject pl1;
    [SerializeField] GameObject pl2;
    [SerializeField] GameObject pl3;

    public GameObject[] hexes = new GameObject[20];
    public GameObject[] itemHexes = new GameObject[3]; // item hexes - h1, h12, h17

    public bool movEnd;
    int turnNo;

    void Awake()
    {
        gBar.transform.position = hexes[10].transform.position;
       
    }

    void Start()
    {
        
    }

    
    void Update()
    {
        turnNo = gameObject.GetComponent<turnEnd>().turnNo;
        print(turnNo);

        if (movEnd)
        {
            if (gBar.transform.position==pl1.transform.position) { pl1.GetComponent<stats>().gold++; gBar.SetActive(false); }
            if (gBar.transform.position==pl2.transform.position) { pl2.GetComponent<stats>().gold++; gBar.SetActive(false); }
            if (gBar.transform.position==pl3.transform.position) { pl3.GetComponent<stats>().gold++; gBar.SetActive(false); }

            movEnd = false;
        }

        if (turnNo == 6 && gBar.active == false)
        {
            print("add gold");

            var pos = hexes.Except(itemHexes).OrderBy(n=>UnityEngine.Random.value).FirstOrDefault().transform.position;
            
        }

    }
}
