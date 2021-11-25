using UnityEngine;
using System.Linq;
using Photon.Pun;
using System.Collections;


public class itemControl : MonoBehaviour
{
    GameObject pl1, pl2, pl3;   

    GameObject[] hexes;
    public GameObject[] itemHexes = new GameObject[3];   //public!
    ExitGames.Client.Photon.Hashtable rProp = new ExitGames.Client.Photon.Hashtable();
    [SerializeField] Material itemMat;
    [SerializeField] Material groundMat;
        
    GameObject gBar1, gBar2, gBar3;

    public bool movEnd;  //sets "true" from turnEnd.cs
    public bool goldEnd; //sets "true" from goldControl.cs
    int turnNo;

    int[] r1 = new int[] { 1, 4, 7, 10 };
    int[] r2 = new int[] { 2, 5, 8, 11 };
    int[] r3 = new int[] { 3, 6, 9, 12 };

    bool bSw1 = true;


    void Awake()
    {
        if (PhotonNetwork.IsMasterClient) { this.enabled = true; }

        pl1 = GameObject.FindGameObjectWithTag("player1");
        pl2 = GameObject.FindGameObjectWithTag("player2");
        pl3 = GameObject.FindGameObjectWithTag("player3");

        hexes = map.mapS.hexes;

        gBar1 = map.mapS.gBar1;
        gBar2 = map.mapS.gBar2;
        gBar3 = map.mapS.gBar3;
                
    }


    void Start()
    {
        itemHexes = new GameObject[3] { map.mapS.hexes[1], map.mapS.hexes[12], map.mapS.hexes[17] };

        rProp["tNo"] = 1; 
        rProp["iHex0"] = itemHexes[0].transform.position; rProp["iHex1"] = itemHexes[1].transform.position; rProp["iHex2"] = itemHexes[2].transform.position;
        PhotonNetwork.CurrentRoom.SetCustomProperties(rProp);
    }

    
    void Update()
    {
        if (!GetComponent<PhotonView>().IsMine) { return; } //host only

        turnNo = gameObject.GetComponent<turnEnd>().turnNo;

        if (bSw1) { itemHexChange(); } 

        if (movEnd && goldEnd)  
        {
            foreach (GameObject x in itemHexes)   //item pick up
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

            item1use(); item2use();
                       
            Invoke("GLsynchWait", 1.5f);  //wait for GameLogic synch
            turnEnd.turnEndS.turnNo++;    //next turn      
            rProp["tNo"] = turnNo+1; PhotonNetwork.CurrentRoom.SetCustomProperties(rProp);
            Invoke("pl1startDelay", 2f);  //wait for ownership return before run pl1


            movEnd = false; goldEnd = false;
        }        
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
            foreach (GameObject itemHex in itemHexes) { itemHex.GetComponent<Renderer>().material = itemMat; }

            rProp["iHex0"] = itemHexes[0].transform.position; rProp["iHex1"] = itemHexes[1].transform.position; rProp["iHex2"] = itemHexes[2].transform.position;
            PhotonNetwork.CurrentRoom.SetCustomProperties(rProp);

            GetComponent<PhotonView>().RPC("RPC_itemSynh", RpcTarget.OthersBuffered);          

            bSw1 = false;
        }        
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
                map.mapS.bombClear();
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
                map.mapS.bombClear();
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
                map.mapS.bombClear();
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
                map.mapS.bombClear();
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
                map.mapS.bombClear();
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
                map.mapS.bombClear();
            }            
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


    [PunRPC] public void RPC_pl1start()
    {
        pl1.GetComponent<plControl>().enabled = true;
    }

    void GLsynchWait()
    {
        pl1.GetComponent<PhotonView>().TransferOwnership(turnEnd.turnEndS.roomPlayers[1]);
        pl2.GetComponent<PhotonView>().TransferOwnership(turnEnd.turnEndS.roomPlayers[2]);
        pl3.GetComponent<PhotonView>().TransferOwnership(turnEnd.turnEndS.roomPlayers[3]);
    }

    void pl1startDelay()
    {        
        GetComponent<PhotonView>().RPC("RPC_pl1start", pl1.GetComponent<PhotonView>().Owner);
    }

    [PunRPC] IEnumerator RPC_itemSynh()
    {
        yield return new WaitForSeconds(1f);

        map.mapS.bNewItem = true;       
    }

}
