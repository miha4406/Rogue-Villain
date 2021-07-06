using System.Collections;
using UnityEngine;
using System.Linq;

public class itemControl : MonoBehaviour
{
    [SerializeField] GameObject pl1;
    [SerializeField] GameObject pl2;
    [SerializeField] GameObject pl3;

    GameObject[] hexes;
    public GameObject[] itemHexes = new GameObject[3];
    [SerializeField] Color iColor;
    [SerializeField] Color groundColor;

    [SerializeField] GameObject gBar1;
    [SerializeField] GameObject gBar2;
    [SerializeField] GameObject gBar3;

    public bool movEnd;  //sets "true" from turnEnd.cs
    int turnNo;

    bool bSw1 = true;

    void Start()
    {
        hexes = gameObject.GetComponent<goldControl>().hexes;
    }

    
    void Update()
    {
        turnNo = gameObject.GetComponent<turnEnd>().turnNo;

        if (movEnd)  //item pick up
        {
            foreach (GameObject x in itemHexes)
            {
                if (x.transform.position == pl1.transform.position)
                {
                    if (pl1.GetComponent<stats>().item1 == 0) { pl1.GetComponent<stats>().item1 = Random.Range(1, 6); }
                    else if (pl1.GetComponent<stats>().item2 == 0) {
                        pl1.GetComponent<stats>().item2 = Random.Range(1, 6);
                        while (pl1.GetComponent<stats>().item2 == pl1.GetComponent<stats>().item1) { pl1.GetComponent<stats>().item2 = Random.Range(1, 6); }                        
                    }
                }
                if (x.transform.position == pl2.transform.position)
                {
                    if (pl2.GetComponent<stats>().item1 == 0) { pl2.GetComponent<stats>().item1 = Random.Range(1, 6); }
                    else if (pl2.GetComponent<stats>().item2 == 0) { 
                        pl2.GetComponent<stats>().item2 = Random.Range(1, 6);
                        while (pl2.GetComponent<stats>().item2 == pl2.GetComponent<stats>().item1) { pl2.GetComponent<stats>().item2 = Random.Range(1, 6); }
                    }
                }
                if (x.transform.position == pl3.transform.position)
                {
                    if (pl3.GetComponent<stats>().item1 == 0) { pl3.GetComponent<stats>().item1 = Random.Range(1, 6); }
                    else if (pl3.GetComponent<stats>().item2 == 0) { 
                        pl3.GetComponent<stats>().item2 = Random.Range(1, 6);
                        while (pl3.GetComponent<stats>().item2 == pl3.GetComponent<stats>().item1) { pl3.GetComponent<stats>().item2 = Random.Range(1, 6); }
                    }
                }
            }
        }

        if (bSw1) { itemHexChange(); }
        //itemHexChange();


        movEnd = false;
    }
    void itemHexChange()
    {
        if (turnNo==6)
        {
            GameObject[] oldItemHexes = new GameObject[3];
            oldItemHexes = itemHexes;
            foreach(GameObject hex in oldItemHexes) { hex.GetComponent<Renderer>().material.color = groundColor; }

            for (int i = 0; i <=2; i++)
            {
                itemHexes[i] = hexes.Except(oldItemHexes).Where(hex => hex != null && (hex.transform.position!=gBar1.transform.position && hex.transform.position!=gBar2.transform.position && hex.transform.position!=gBar3.transform.position)
                && (Vector3.Distance(hex.transform.position, itemHexes[0].transform.position)>1.3f && Vector3.Distance(hex.transform.position, itemHexes[1].transform.position)>1.3f && Vector3.Distance(hex.transform.position, itemHexes[2].transform.position)>1.3f))
                    .OrderBy(n => Random.value).FirstOrDefault();  //dist!
            }
            bSw1 = false;
        }
        
        foreach(GameObject itemHex in itemHexes) { itemHex.GetComponent<Renderer>().material.color = iColor; }
    }
}
