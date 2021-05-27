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

    public bool movEnd;  //sets "true" from turnEnd.cs
    int turnNo;

    void Awake()
    {
        gBar.transform.position = hexes[10].transform.position;       
    }  

    
    void Update()
    {
        turnNo = gameObject.GetComponent<turnEnd>().turnNo;
        print("Turn "+turnNo);

        if (movEnd && gBar.active)  //gold pick up
        {
            if (gBar.transform.position==pl1.transform.position) { pl1.GetComponent<stats>().gold++; gBar.SetActive(false); }
            if (gBar.transform.position==pl2.transform.position) { pl2.GetComponent<stats>().gold++; gBar.SetActive(false); }
            if (gBar.transform.position==pl3.transform.position) { pl3.GetComponent<stats>().gold++; gBar.SetActive(false); }

            movEnd = false;
        }
        

        if ((turnNo==6 || turnNo==11) && gBar.active == false) //new gold add
        {
            print("add gold");

            gBar.SetActive(true);
            gBar.transform.position = hexes.Where(el => el != null).Except(itemHexes).OrderBy(n => Random.value).FirstOrDefault().transform.position;

        }       

    }
    public void goldSwap(GameObject plA, GameObject plB, GameObject plC)  //gold exchange, used in plCollision (turnEnd.cs)
    {
        if (plA.transform.position==plB.transform.position && plB.transform.position==plC.transform.position)
        {
            //do nothing?
        }
        else if (plA.transform.position == plB.transform.position)  //will swap EVERY time on collision
        {
            int t = plA.GetComponent<stats>().gold;
            plA.GetComponent<stats>().gold = plB.GetComponent<stats>().gold;
            plB.GetComponent<stats>().gold = t;
        }
    }
}
