using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using Photon.Pun;

public class goldControl : MonoBehaviour
{
    GameObject pl1, pl2, pl3;

    GameObject gBar1, gBar2, gBar3;

    bool b1skill = false;

    GameObject[] hexes = new GameObject[20];
    public GameObject[] itemHexes = new GameObject[3]; // item hexes from itemControl    

    public bool movEnd;  //sets "true" from turnEnd.cs
    int turnNo;

    PhotonView pv;

    void Awake()
    {
        if (PhotonNetwork.IsMasterClient) { this.enabled = true; }

        pl1 = GameObject.FindGameObjectWithTag("player1");
        pl2 = GameObject.FindGameObjectWithTag("player2");
        pl3 = GameObject.FindGameObjectWithTag("player3");

        hexes = map.mapS.hexes;             

        pv = GetComponent<PhotonView>();
    }


    private void Start()
    {
        //can't set in Awake
        gBar1 = map.mapS.gBar1;
        gBar2 = map.mapS.gBar2;
        gBar3 = map.mapS.gBar3;

        gBar1.transform.position = hexes[10].transform.position;
    }


    void Update()
    {
        //if (!GetComponent<PhotonView>().IsMine) { return; } //host only

        if (pl1.GetComponent<PhotonView>().Owner != pl3.GetComponent<PhotonView>().Owner) //all owned by Host
        {
            return;
        }

        turnNo = gameObject.GetComponent<turnEnd>().turnNo;        

        itemHexes = gameObject.GetComponent<itemControl>().itemHexes;

        if (turnNo>0) { gHex1(); }
        if (turnNo>5) { gHex2(); }
        if (turnNo>10){ gHex3(); }

        if (movEnd) { CheckPos(); }        

        if (movEnd) { p1Challenge(); }

        if (movEnd) { p2Attack(); }
        if (movEnd) { p2Shoot(); }

        if (movEnd) { p3GoldGet(); } //

        movEnd = false;
        gameObject.GetComponent<itemControl>().goldEnd = true;
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

        if (pl1.GetComponent<PhotonView>().Owner != pl3.GetComponent<PhotonView>().Owner) //all owned by Host
        {
            return;
        }

        if (movEnd && gBar1.active) //gBar1 pick up (2turns for pl1, 2bars for pl3)
        {
            if (gBar1.transform.position == pl1.transform.position) {

                pv.RPC("pl1AC", RpcTarget.AllBuffered, "base.pl1-gold");  //anim

                if ( pl1.GetComponent<stats>().pasSkillHex == Vector3.down ) {
                    pl1.GetComponent<stats>().pasSkillHex = pl1.transform.position;
                }
                else if(pl1.GetComponent<stats>().pasSkillHex == pl1.transform.position)
                { 
                    pl1.GetComponent<stats>().gold++; gBar1.SetActive(false);
                    pl1.GetComponent<stats>().pasSkillHex = Vector3.down;
                }                
            } else if (pl1.GetComponent<stats>().pasSkillHex != Vector3.down && pl1.GetComponent<stats>().pasSkillHex != pl1.transform.position) 
            { pl1.GetComponent<stats>().pasSkillHex = Vector3.down; }

            if (gBar1.transform.position == pl2.transform.position) {
                pv.RPC("pl2AC", RpcTarget.AllBuffered, "base.pl2-gold");  //anim
                pl2.GetComponent<stats>().gold++; gBar1.SetActive(false); 
            }

            if (gBar1.transform.position == pl3.transform.position) {
                pv.RPC("pl3AC", RpcTarget.AllBuffered, "base.pl3-gold");  //anim
                pl3.GetComponent<stats>().gold+=2; gBar1.SetActive(false); 
            }           
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

        if (pl1.GetComponent<PhotonView>().Owner != pl3.GetComponent<PhotonView>().Owner) //all owned by Host
        {
            return;
        }

        if (movEnd && gBar2.active) //gBar1 pick up (2turns for pl1, 2bars for pl3)
        {
            if (gBar2.transform.position == pl1.transform.position) {

                pv.RPC("pl1AC", RpcTarget.AllBuffered, "base.pl1-gold");  //anim

                if (pl1.GetComponent<stats>().pasSkillHex == Vector3.down)
                {
                    pl1.GetComponent<stats>().pasSkillHex = pl1.transform.position;
                }
                else if (pl1.GetComponent<stats>().pasSkillHex == pl1.transform.position)
                {
                    pl1.GetComponent<stats>().gold++; gBar2.SetActive(false);
                    pl1.GetComponent<stats>().pasSkillHex = Vector3.down;
                }
            } else if(pl1.GetComponent<stats>().pasSkillHex != Vector3.down && pl1.GetComponent<stats>().pasSkillHex != pl1.transform.position) 
            { pl1.GetComponent<stats>().pasSkillHex = Vector3.down; }

            if (gBar2.transform.position == pl2.transform.position) {
                pv.RPC("pl2AC", RpcTarget.AllBuffered, "base.pl2-gold");  //anim
                pl2.GetComponent<stats>().gold++; gBar2.SetActive(false); 
            }

            if (gBar2.transform.position == pl3.transform.position) {
                pv.RPC("pl3AC", RpcTarget.AllBuffered, "base.pl3-gold");  //anim
                pl3.GetComponent<stats>().gold+=2; gBar2.SetActive(false); 
            }            
        }
    }
    void gHex3()
    {
        if (gBar3.active == false) //new gBar3 add
        {
            gBar3.transform.position = hexes.Where(hex => hex != null && (hex.transform.position != gBar1.transform.position
            && hex.transform.position != gBar2.transform.position && hex.transform.position != gBar3.transform.position))
                .Except(itemHexes).OrderBy(n => Random.value).FirstOrDefault().transform.position;
            gBar3.SetActive(true);
        }

        if (pl1.GetComponent<PhotonView>().Owner != pl3.GetComponent<PhotonView>().Owner) //all owned by Host
        {
            return;
        }

        if (movEnd && gBar3.active) //gBar1 pick up (2turns for pl1, 2bars for pl3)
        {
            if (gBar3.transform.position == pl1.transform.position) {

                pv.RPC("pl1AC", RpcTarget.AllBuffered, "base.pl1-gold");  //anim

                if (pl1.GetComponent<stats>().pasSkillHex == Vector3.down)
                {
                    pl1.GetComponent<stats>().pasSkillHex = pl1.transform.position;
                }
                else if (pl1.GetComponent<stats>().pasSkillHex == pl1.transform.position)
                {
                    pl1.GetComponent<stats>().gold++; gBar3.SetActive(false);
                    pl1.GetComponent<stats>().pasSkillHex = Vector3.down;
                }
            }
            else if(pl1.GetComponent<stats>().pasSkillHex != Vector3.down && pl1.GetComponent<stats>().pasSkillHex != pl1.transform.position)  
            { pl1.GetComponent<stats>().pasSkillHex = Vector3.down; }

            if (gBar3.transform.position == pl2.transform.position) {
                pv.RPC("pl2AC", RpcTarget.AllBuffered, "base.pl2-gold");  //anim
                pl2.GetComponent<stats>().gold++; gBar3.SetActive(false); 
            }

            if (gBar3.transform.position == pl3.transform.position) {
                pv.RPC("pl3AC", RpcTarget.AllBuffered, "base.pl3-gold");  //anim
                pl3.GetComponent<stats>().gold+=2; gBar3.SetActive(false); 
            }
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
            GameObject pl1trg = null;  //can't stream GameObjects!
            if (pl1.GetComponent<stats>().actSkillTrg[0] != Vector3.down) {
                pl1trg = (pl1.GetComponent<stats>().actSkillTrg[0] == new Vector3(2f, 2f, 2f)) ? pl2 : pl3;
            }            

            if ( b1skill && (plA==pl1 && plB==pl1trg) )
            {
                GetComponent<PhotonView>().RPC("RPC_messageSystem", RpcTarget.AllBuffered, "p1-chlg-win", 0, 0);
                b1skill = false;  //only once!

                if (plB.GetComponent<stats>().gold >= 3) { plB.GetComponent<stats>().gold -=3; plA.GetComponent<stats>().gold +=3; }
                if (plB.GetComponent<stats>().gold == 2) { plB.GetComponent<stats>().gold -=2; plA.GetComponent<stats>().gold +=2; }
                if (plB.GetComponent<stats>().gold == 1) { plB.GetComponent<stats>().gold -=1; plA.GetComponent<stats>().gold +=1; }
            }
            else if ( b1skill && (plB==pl1 && plA==pl1trg) )
            {
                GetComponent<PhotonView>().RPC("RPC_messageSystem", RpcTarget.AllBuffered, "p1-chlg-win", 0, 0); 
                b1skill = false;  //only once!

                if (plA.GetComponent<stats>().gold >= 3) { plA.GetComponent<stats>().gold -=3; plB.GetComponent<stats>().gold +=3; }
                if (plA.GetComponent<stats>().gold == 2) { plA.GetComponent<stats>().gold -=2; plB.GetComponent<stats>().gold +=2; }
                if (plA.GetComponent<stats>().gold == 1) { plA.GetComponent<stats>().gold -=1; plB.GetComponent<stats>().gold +=1; }
            }
            else
            {
                int t = plA.GetComponent<stats>().gold;
                plA.GetComponent<stats>().gold = plB.GetComponent<stats>().gold;
                plB.GetComponent<stats>().gold = t;
            }
        }
    }    

    ////////////////////////// SKILLS /////////////////////////

    public void p1Challenge()
    {
        if (pl1.GetComponent<stats>().skillCD !=0 )
        {
            if (pl1.GetComponent<stats>().skillCD == 4) { b1skill = true; }
            if (pl1.GetComponent<stats>().skillCD == 3) { 
                b1skill = false; pl1.GetComponent<stats>().actSkillTrg[0] = Vector3.down;
            }

            pl1.GetComponent<stats>().skillCD--;
        }
    }

    public void p2Attack()
    {
        if (pl2.GetComponent<stats>().pasSkillHex!=Vector3.down)
        {
            if (pl2.GetComponent<stats>().pasSkillHex == pl1.transform.position) 
            {
                pl2.transform.LookAt(pl1.transform);                
                GetComponent<PhotonView>().RPC("RPC_messageSystem", RpcTarget.AllBuffered, "p2-hit", 1, 0);
                GetComponent<PhotonView>().RPC("pl2AC", RpcTarget.AllBuffered, "base.pl2-skill1"); //anim
                if (pl1.GetComponent<stats>().gold >= 2) { pl1.GetComponent<stats>().gold -= 2; pl2.GetComponent<stats>().gold += 2; }
                else if (pl1.GetComponent<stats>().gold == 1) { pl1.GetComponent<stats>().gold -= 1; pl2.GetComponent<stats>().gold += 1; }
            }
            if (pl2.GetComponent<stats>().pasSkillHex == pl3.transform.position) 
            {
                pl2.transform.LookAt(pl3.transform);
                GetComponent<PhotonView>().RPC("RPC_messageSystem", RpcTarget.AllBuffered, "p2-hit", 3, 0); 
                GetComponent<PhotonView>().RPC("pl2AC", RpcTarget.AllBuffered, "base.pl2-skill1"); //anim
                if (pl3.GetComponent<stats>().gold >= 2) { pl3.GetComponent<stats>().gold -= 2; pl2.GetComponent<stats>().gold += 2; }
                else if (pl3.GetComponent<stats>().gold == 1) { pl3.GetComponent<stats>().gold -= 1; pl2.GetComponent<stats>().gold += 1; }
            }

            pl2.GetComponent<stats>().pasSkillHex = Vector3.down;
        }
    }
    public void p2Shoot() //after shoot prep
    {
        if (pl2.GetComponent<stats>().actSkillTrg[0]!=Vector3.down)
        {
            if (pl2.GetComponent<stats>().actSkillTrg[0] == pl1.transform.position 
                || pl2.GetComponent<stats>().actSkillTrg[0] == pl3.transform.position) {                
                GetComponent<PhotonView>().RPC("RPC_messageSystem", RpcTarget.AllBuffered, "p2-shoot-fail", 0, 0);
            }
            else
            {
                for(int j=1; j<=3; j++)
                {
                    if (pl2.GetComponent<stats>().actSkillTrg[j] != Vector3.down)
                    {
                        if (pl2.GetComponent<stats>().actSkillTrg[j] == pl1.transform.position)
                        {
                            pl2.transform.LookAt(pl1.transform);                            
                            GetComponent<PhotonView>().RPC("RPC_messageSystem", RpcTarget.AllBuffered, "p2-shoot", 1, 0);
                            GetComponent<PhotonView>().RPC("pl2AC", RpcTarget.AllBuffered, "base.pl2-skill2"); //anim
                            if (pl1.GetComponent<stats>().gold >= 2) { pl1.GetComponent<stats>().gold -= 2; pl2.GetComponent<stats>().gold += 2; }
                            else if (pl1.GetComponent<stats>().gold == 1) { pl1.GetComponent<stats>().gold -= 1; pl2.GetComponent<stats>().gold += 1; }                            
                        }
                        if (pl2.GetComponent<stats>().actSkillTrg[j] == pl3.transform.position)
                        {
                            pl2.transform.LookAt(pl3.transform);
                            GetComponent<PhotonView>().RPC("RPC_messageSystem", RpcTarget.AllBuffered, "p2-shoot", 3, 0);
                            GetComponent<PhotonView>().RPC("pl2AC", RpcTarget.AllBuffered, "base.pl2-skill2"); //anim
                            if (pl3.GetComponent<stats>().gold >= 2) { pl3.GetComponent<stats>().gold -= 2; pl2.GetComponent<stats>().gold += 2; }
                            else if (pl3.GetComponent<stats>().gold == 1) { pl3.GetComponent<stats>().gold -= 1; pl2.GetComponent<stats>().gold += 1; }
                        }
                    }
                }
                pl2.transform.position = pl2.GetComponent<stats>().actSkillTrg[0];
            }
            
            pl2.GetComponent<stats>().actSkillTrg = new Vector3[4] { Vector3.down, Vector3.down, Vector3.down, Vector3.down };
        }
        if (movEnd && pl2.GetComponent<stats>().skillCD!=0) { pl2.GetComponent<stats>().skillCD--; }
    }

    public void p3GoldGet()
    {
        if (pl3.GetComponent<stats>().actSkillTrg[0] != Vector3.down)
        {
            GetComponent<PhotonView>().RPC("RPC_messageSystem", RpcTarget.AllBuffered, "p3-gold", 0, 0);

            pl3.GetComponent<stats>().gold++;

            pl3.GetComponent<stats>().actSkillTrg[0] = Vector3.down;
        }

        if (movEnd && pl3.GetComponent<stats>().skillCD!=0) { pl3.GetComponent<stats>().skillCD--; }        
    }

    public void CheckPos()  //if networking synchronisation problem
    {     
            foreach (GameObject hex in hexes) 
            {
                if (hex != null)
                {
                    if (Vector3.Distance(hex.transform.position, pl1.transform.position) < 0.3f)
                    {
                        pl1.transform.position = hex.transform.position;
                    }
                }
            }
            foreach (GameObject hex in hexes)
            {
                if (hex != null)
                {
                    if (Vector3.Distance(hex.transform.position, pl2.transform.position) < 0.3f)
                    {
                        pl2.transform.position = hex.transform.position;
                    }
                }
            }
            foreach (GameObject hex in hexes)
            {
                if (hex != null)
                {
                    if (Vector3.Distance(hex.transform.position, pl3.transform.position) < 0.3f)
                    {
                        pl3.transform.position = hex.transform.position;
                    }
                }
            }           
    }


    //[PunRPC] void 

}
