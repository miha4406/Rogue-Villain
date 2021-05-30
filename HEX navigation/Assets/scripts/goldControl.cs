using System.Collections;
using UnityEngine;
using System.Linq;

public class goldControl : MonoBehaviour
{
    [SerializeField] GameObject gBar1;
    [SerializeField] GameObject gBar2;
    [SerializeField] GameObject gBar3;

    [SerializeField] GameObject pl1;
    [SerializeField] GameObject pl2;
    [SerializeField] GameObject pl3;
   

    public GameObject[] hexes = new GameObject[20];
    public GameObject[] itemHexes = new GameObject[3]; // item hexes - h1, h12, h17

    public bool movEnd;  //sets "true" from turnEnd.cs
    int turnNo;

    void Awake()
    {
        gBar1.transform.position = hexes[10].transform.position;       
    }  

    
    void Update()
    {
        turnNo = gameObject.GetComponent<turnEnd>().turnNo;
        print("Turn "+turnNo);

        if (turnNo>0) { gHex1(); }
        if (turnNo>5) { gHex2(); }
        if (turnNo>10){ gHex3(); }

        movEnd = false;

    }

    void gHex1()
    {
        if (gBar1.active == false) //new gBar1 add
        {
            gBar1.transform.position = hexes.Where(hex => hex != null && (hex.transform.position != gBar1.transform.position
            && hex.transform.position != gBar2.transform.position && hex.transform.position != gBar3.transform.position))
                .Except(itemHexes).OrderBy(n => Random.value).FirstOrDefault().transform.position;
            gBar1.SetActive(true);
        }

        if (movEnd && gBar1.active)  //gold pick up
        {
            if (gBar1.transform.position == pl1.transform.position) { pl1.GetComponent<stats>().gold++; gBar1.SetActive(false); }
            if (gBar1.transform.position == pl2.transform.position) { pl2.GetComponent<stats>().gold++; gBar1.SetActive(false); }
            if (gBar1.transform.position == pl3.transform.position) { pl3.GetComponent<stats>().gold++; gBar1.SetActive(false); }           
        }
     
    }
    void gHex2()
    {
        if (gBar2.active == false) //new gBar2 add
        {
            gBar2.transform.position = hexes.Where(hex => hex != null && (hex.transform.position != gBar1.transform.position
            && hex.transform.position != gBar2.transform.position && hex.transform.position != gBar3.transform.position))
                .Except(itemHexes).OrderBy(n => Random.value).FirstOrDefault().transform.position;
            gBar2.SetActive(true);
        }
        
        if (movEnd && gBar2.active)
        {
            if (gBar2.transform.position == pl1.transform.position) { pl1.GetComponent<stats>().gold++; gBar2.SetActive(false); }
            if (gBar2.transform.position == pl2.transform.position) { pl2.GetComponent<stats>().gold++; gBar2.SetActive(false); }
            if (gBar2.transform.position == pl3.transform.position) { pl3.GetComponent<stats>().gold++; gBar2.SetActive(false); }            
        }
    }
    void gHex3()
    {
        if (gBar3.active == false) //new gBar2 add
        {
            gBar3.transform.position = hexes.Where(hex => hex != null && (hex.transform.position != gBar1.transform.position
            && hex.transform.position != gBar2.transform.position && hex.transform.position != gBar3.transform.position))
                .Except(itemHexes).OrderBy(n => Random.value).FirstOrDefault().transform.position;
            gBar3.SetActive(true);
        }

        if (movEnd && gBar3.active)
        {
            if (gBar3.transform.position == pl1.transform.position) { pl1.GetComponent<stats>().gold++; gBar3.SetActive(false); }
            if (gBar3.transform.position == pl2.transform.position) { pl2.GetComponent<stats>().gold++; gBar3.SetActive(false); }
            if (gBar3.transform.position == pl3.transform.position) { pl3.GetComponent<stats>().gold++; gBar3.SetActive(false); }
        }
    }

    public void goldSwap(GameObject plA, GameObject plB, GameObject plC)  //gold exchange, used in plCollision (turnEnd.cs)
    {
        if (plA.transform.position==plB.transform.position && plB.transform.position==plC.transform.position)
        {
            int avg = (pl1.GetComponent<stats>().gold + pl2.GetComponent<stats>().gold + pl3.GetComponent<stats>().gold) / 3;
            pl1.GetComponent<stats>().gold = avg;
            pl2.GetComponent<stats>().gold = avg;
            pl3.GetComponent<stats>().gold = avg;
           
        }
        else if (plA.transform.position == plB.transform.position)  //will swap EVERY time on collision
        {
            int t = plA.GetComponent<stats>().gold;
            plA.GetComponent<stats>().gold = plB.GetComponent<stats>().gold;
            plB.GetComponent<stats>().gold = t;
        }
    }
}
