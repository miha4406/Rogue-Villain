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
    [SerializeField] Material itemMat;
    [SerializeField] Material groundMat;

    [SerializeField] GameObject gBar1;
    [SerializeField] GameObject gBar2;
    [SerializeField] GameObject gBar3;

    public bool movEnd;  //sets "true" from turnEnd.cs
    int turnNo;

    int[] r1 = new int[] { 1, 4, 7, 10 };
    int[] r2 = new int[] { 2, 5, 8, 11 };
    int[] r3 = new int[] { 3, 6, 9, 12 };

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
                    jewelCheck(pl1);

                    if (pl1.GetComponent<stats>().item1 == 0) { item1Generator(pl1); }                    
                    else if (pl1.GetComponent<stats>().item2 == 0) { item2Generator(pl1); }
                }
                if (x.transform.position == pl2.transform.position)
                {
                    jewelCheck(pl2);

                    if (pl2.GetComponent<stats>().item1 == 0) { item1Generator(pl2); }
                    else if (pl2.GetComponent<stats>().item2 == 0) { item2Generator(pl2); }
                }
                if (x.transform.position == pl3.transform.position)
                {
                    jewelCheck(pl3);

                    if (pl3.GetComponent<stats>().item1 == 0) { item1Generator(pl3); }
                    else if (pl3.GetComponent<stats>().item2 == 0) { item2Generator(pl3); }
                }
            }
        }

        if(movEnd) { item1use(); }
        if(movEnd) { item2use(); }

        if(bSw1) { itemHexChange(); }        

        movEnd = false;
    }
    void itemHexChange()
    {
        if (turnNo==6 || turnNo==11)
        {
            GameObject[] oldItemHexes = new GameObject[3];
            oldItemHexes = itemHexes;
            foreach(GameObject hex in oldItemHexes) { hex.GetComponent<Renderer>().material = groundMat; }

            for (int i = 0; i <=2; i++)
            {
                itemHexes[i] = hexes.Except(oldItemHexes).Where(hex => hex != null && (hex.transform.position!=gBar1.transform.position && hex.transform.position!=gBar2.transform.position && hex.transform.position!=gBar3.transform.position)
                && (Vector3.Distance(hex.transform.position, itemHexes[0].transform.position)>1.3f && Vector3.Distance(hex.transform.position, itemHexes[1].transform.position)>1.3f && Vector3.Distance(hex.transform.position, itemHexes[2].transform.position)>1.3f))
                    .OrderBy(n => Random.value).FirstOrDefault();  //dist!
            }
            bSw1 = false;
        }
        
        foreach(GameObject itemHex in itemHexes) { itemHex.GetComponent<Renderer>().material = itemMat; }
    }

    ///////////////////////////ITEMS///////////////////////////
    void item1Generator(GameObject player)
    {
        int gen = 0;

        if (turnNo<6)
        {
            gen = Random.Range(1, 101);

            if (gen <= 75) { player.GetComponent<stats>().item1 = r1.OrderBy(n => Random.value).FirstOrDefault(); }
            else { player.GetComponent<stats>().item1 = r2.OrderBy(n => Random.value).FirstOrDefault(); }  //r2
        }
        else if (turnNo>=6 && turnNo <=10)
        {
            gen = Random.Range(1, 101);

            if (gen <= 60) { player.GetComponent<stats>().item1 = r1.OrderBy(n => Random.value).FirstOrDefault(); }
            else if (gen>90) { player.GetComponent<stats>().item1 = r3.OrderBy(n => Random.value).FirstOrDefault(); }  //r3
            else { player.GetComponent<stats>().item1 = r2.OrderBy(n => Random.value).FirstOrDefault(); } //r2
        }
        else if (turnNo>10)
        {
            gen = Random.Range(1, 101);

            if (gen <= 30) { player.GetComponent<stats>().item1 = r1.OrderBy(n => Random.value).FirstOrDefault(); }
            else if (gen > 70) { player.GetComponent<stats>().item1 = r3.OrderBy(n => Random.value).FirstOrDefault(); }  //r3
            else { player.GetComponent<stats>().item1 = r2.OrderBy(n => Random.value).FirstOrDefault(); } //r2
        }
    }
    void item2Generator(GameObject player)
    {
        int gen = 0;

        if (turnNo < 6)
        {
            gen = Random.Range(1, 101);

            if (gen <= 75) { player.GetComponent<stats>().item2 = r1.OrderBy(n => Random.value).FirstOrDefault(); }
            else { player.GetComponent<stats>().item2 = r2.OrderBy(n => Random.value).FirstOrDefault(); }  //r2
        }
        else if (turnNo >= 6 && turnNo <= 10)
        {
            gen = Random.Range(1, 101);

            if (gen <= 60) { player.GetComponent<stats>().item2 = r1.OrderBy(n => Random.value).FirstOrDefault(); }
            else if (gen > 90) { player.GetComponent<stats>().item2 = r3.OrderBy(n => Random.value).FirstOrDefault(); }  //r3
            else { player.GetComponent<stats>().item2 = r2.OrderBy(n => Random.value).FirstOrDefault(); } //r2
        }
        else if (turnNo > 10)
        {
            gen = Random.Range(1, 101);

            if (gen <= 30) { player.GetComponent<stats>().item2 = r1.OrderBy(n => Random.value).FirstOrDefault(); }
            else if (gen > 70) { player.GetComponent<stats>().item2 = r3.OrderBy(n => Random.value).FirstOrDefault(); }  //r3
            else { player.GetComponent<stats>().item2 = r2.OrderBy(n => Random.value).FirstOrDefault(); } //r2
        }
    }


    void jewelCheck(GameObject player)
    {
        if (player.GetComponent<stats>().item1==10 || player.GetComponent<stats>().item1==11 || player.GetComponent<stats>().item1==12) { //jewel item1
            if (player.transform.position != player.GetComponent<stats>().item1Targets[0])
            {
                player.GetComponent<stats>().gold += player.GetComponent<stats>().item1 - 9;
                player.GetComponent<stats>().item1 = 0;
                player.GetComponent<stats>().item1Targets[0] = Vector3.down;
            }
        }
        if (player.GetComponent<stats>().item2==10 || player.GetComponent<stats>().item2==11 || player.GetComponent<stats>().item2==12) { //jewel item2
            if (player.transform.position != player.GetComponent<stats>().item2Targets[0])
            {
                player.GetComponent<stats>().gold += player.GetComponent<stats>().item2 - 9;
                player.GetComponent<stats>().item2 = 0;
                player.GetComponent<stats>().item2Targets[0] = Vector3.down;
            }
        }
    }


    void item1use()
    {
        if (pl1.GetComponent<stats>().item1==1 || pl1.GetComponent<stats>().item1==2 || pl1.GetComponent<stats>().item1==3)  //bombs use (pl1)
        {
            if (pl1.GetComponent<stats>().item1Targets[0] != Vector3.down)
            {
                foreach (Vector3 hex in pl1.GetComponent<stats>().item1Targets)
                {
                    if (pl2.transform.position == hex)
                    {
                        print("pl2 was bombed!");
                        if (pl2.GetComponent<stats>().gold > 0) { pl2.GetComponent<stats>().gold--; pl1.GetComponent<stats>().gold++; }
                    }
                    if (pl3.transform.position == hex)
                    {
                        print("pl3 was bombed!");
                        if (pl3.GetComponent<stats>().gold > 0) { pl3.GetComponent<stats>().gold--; pl1.GetComponent<stats>().gold++; }
                    }
                }
                pl1.GetComponent<stats>().item1 = 0;
                bombClear();
            }            
        }       
        if (pl2.GetComponent<stats>().item1==1 || pl2.GetComponent<stats>().item1==2 || pl2.GetComponent<stats>().item1==3)  //bombs use (pl2)
        {
            if (pl2.GetComponent<stats>().item1Targets[0] != Vector3.down)
            {
                foreach (Vector3 hex in pl2.GetComponent<stats>().item1Targets)
                {
                    if (pl1.transform.position == hex)
                    {
                        print("pl1 was bombed!");
                        if (pl1.GetComponent<stats>().gold > 0) { pl1.GetComponent<stats>().gold--; pl2.GetComponent<stats>().gold++; }
                    }
                    if (pl3.transform.position == hex)
                    {
                        print("pl3 was bombed!");
                        if (pl3.GetComponent<stats>().gold > 0) { pl3.GetComponent<stats>().gold--; pl2.GetComponent<stats>().gold++; }
                    }
                }
                pl2.GetComponent<stats>().item1 = 0;
                bombClear();
            }            
        }     
        if (pl3.GetComponent<stats>().item1==1 || pl3.GetComponent<stats>().item1==2 || pl3.GetComponent<stats>().item1==3)  //bombs use (pl3)
        {
            if (pl3.GetComponent<stats>().item1Targets[0] != Vector3.down)
            {
                foreach (Vector3 hex in pl3.GetComponent<stats>().item1Targets)
                {
                    if (pl1.transform.position == hex)
                    {
                        print("pl1 was bombed!");
                        if (pl1.GetComponent<stats>().gold > 0) { pl1.GetComponent<stats>().gold--; pl3.GetComponent<stats>().gold++; }
                    }
                    if (pl2.transform.position == hex)
                    {
                        print("pl2 was bombed!");
                        if (pl2.GetComponent<stats>().gold > 0) { pl2.GetComponent<stats>().gold--; pl3.GetComponent<stats>().gold++; }
                    }
                }
                pl3.GetComponent<stats>().item1 = 0;
                bombClear();
            }            
        }     
    }
    void item2use()
    {
        if (pl1.GetComponent<stats>().item2==1 || pl1.GetComponent<stats>().item2==2 || pl1.GetComponent<stats>().item2==3)  //bombs use (pl1)
        {
            if (pl1.GetComponent<stats>().item2Targets[0] != Vector3.down)
            {
                foreach (Vector3 hex in pl1.GetComponent<stats>().item2Targets)
                {
                    if (pl2.transform.position == hex)
                    {
                        print("pl2 was bombed!");
                        if (pl2.GetComponent<stats>().gold > 0) { pl2.GetComponent<stats>().gold--; pl1.GetComponent<stats>().gold++; }
                    }
                    if (pl3.transform.position == hex)
                    {
                        print("pl3 was bombed!");
                        if (pl3.GetComponent<stats>().gold > 0) { pl3.GetComponent<stats>().gold--; pl1.GetComponent<stats>().gold++; }
                    }
                }
                pl1.GetComponent<stats>().item2 = 0;
                bombClear();
            }            
        }     
        if (pl2.GetComponent<stats>().item2==1 || pl2.GetComponent<stats>().item2==2 || pl2.GetComponent<stats>().item2==3)  //bombs use (pl2)
        {
            if (pl2.GetComponent<stats>().item2Targets[0] != Vector3.down)
            {
                foreach (Vector3 hex in pl2.GetComponent<stats>().item2Targets)
                {
                    if (pl1.transform.position == hex)
                    {
                        print("pl1 was bombed!");
                        if (pl1.GetComponent<stats>().gold > 0) { pl1.GetComponent<stats>().gold--; pl2.GetComponent<stats>().gold++; }
                    }
                    if (pl3.transform.position == hex)
                    {
                        print("pl3 was bombed!");
                        if (pl3.GetComponent<stats>().gold > 0) { pl3.GetComponent<stats>().gold--; pl2.GetComponent<stats>().gold++; }
                    }
                }
                pl2.GetComponent<stats>().item2 = 0;
                bombClear();
            }            
        }     
        if (pl3.GetComponent<stats>().item2==1 || pl3.GetComponent<stats>().item2==2 || pl3.GetComponent<stats>().item2==3)  //bombs use (pl3)
        {
            if (pl3.GetComponent<stats>().item2Targets[0] != Vector3.down)
            {
                foreach (Vector3 hex in pl3.GetComponent<stats>().item2Targets)
                {
                    if (pl1.transform.position == hex)
                    {
                        print("pl1 was bombed!");
                        if (pl1.GetComponent<stats>().gold > 0) { pl1.GetComponent<stats>().gold--; pl3.GetComponent<stats>().gold++; }
                    }
                    if (pl2.transform.position == hex)
                    {
                        print("pl2 was bombed!");
                        if (pl2.GetComponent<stats>().gold > 0) { pl2.GetComponent<stats>().gold--; pl3.GetComponent<stats>().gold++; }
                    }
                }
                pl3.GetComponent<stats>().item2 = 0;
                bombClear();
            }            
        }     
    }

    public void bombClear()
    {
        foreach (GameObject bomb in GameObject.FindGameObjectsWithTag("bombPref"))
        {
            Destroy(bomb);  
        }        
    }

    public bool slimeObstacle(Vector3 pos)
    {
        foreach (GameObject slime in GameObject.FindGameObjectsWithTag("slimePref"))
        {
            if (Vector3.Distance(pos, slime.transform.position)<0.5f 
                && slime.GetComponent<slimeScript>().crTurn+1==turnNo ) { return true; }
        }

        return false;
    }


}
