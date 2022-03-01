using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class turnEnd : MonoBehaviour 
{
    public static turnEnd turnEndS;  //turnEnd class singleton
    PhotonView pv;

    [SerializeField] float plMovSp = 1f;
    GameObject pl1, pl2, pl3;
    int pl1dist, pl2dist, pl3dist;

    Vector3[] p1Path = new Vector3[6];
    int mov1StepNo = 1;
    int lastMov1 = 0;
    bool bMove1 = false;

    Vector3[] p2Path = new Vector3[6];
    int mov2StepNo = 1;
    int lastMov2 = 0;
    bool bMove2 = false;

    Vector3[] p3Path = new Vector3[6];
    int mov3StepNo = 1;
    int lastMov3 = 0;
    bool bMove3 = false;

    public bool? bInput = null;     //if "true", we check for bMyTurn. sets "true" by Host (pl1)
    bool bTurnEnd = false;   //turn end seq. started
    bool bColEnd = false;    //all collisions ended
    public int turnNo = 1;

    public Player[] roomPlayers = new Player[4];

    Animator pl1anim, pl2anim, pl3anim;


    void Awake()
    {
        if (PhotonNetwork.IsMasterClient) { this.enabled = true; }

        turnEndS = this;
        pv = GetComponent<PhotonView>();

        pl1 = GameObject.FindGameObjectWithTag("player1");
        pl2 = GameObject.FindGameObjectWithTag("player2");
        pl3 = GameObject.FindGameObjectWithTag("player3");

        pl1.GetComponent<stats>().movDist = 2;
        pl2.GetComponent<stats>().movDist = 1;
        pl3.GetComponent<stats>().movDist = 1;

        roomPlayers[1] = pl1.GetComponent<PhotonView>().Controller;
        roomPlayers[2] = pl2.GetComponent<PhotonView>().Controller;
        roomPlayers[3] = pl3.GetComponent<PhotonView>().Controller;

        pl1anim = pl1.GetComponentInChildren<Animator>();
        pl2anim = pl2.GetComponentInChildren<Animator>();
        pl3anim = pl3.GetComponentInChildren<Animator>();
    }

    //private void OnEnable()
    //{
    //    print(this + " enabled!");
    //}


    void Update()
    {
        if (!GetComponent<PhotonView>().IsMine) { return; } //host only

        
        pl1dist = pl1.GetComponent<stats>().movDist;
        pl2dist = pl2.GetComponent<stats>().movDist;
        pl3dist = pl3.GetComponent<stats>().movDist;


        if (bInput == true)
        {
            if (pl1.GetComponent<stats>().bMyTurn == false
            && pl2.GetComponent<stats>().bMyTurn == false
            && pl3.GetComponent<stats>().bMyTurn == false)
            {
                Invoke("endTurn", 2f);  //synch
                bInput = false;
            }
        }
        ///////////////////////////////////


        //char movement         
        if (bMove1) { pl1move(mov1StepNo); }
        if (bMove2) { pl2move(mov2StepNo); }
        if (bMove3) { pl3move(mov3StepNo); }

        if (turnNo > 15) {
        //    Camera.main.transform.LookAt(Vector3.up*10); //to avoid map clicks
        //    map.mapS.resultPanel.SetActive(true);
        //    GameObject.Find("ScreenCanvas/resultPanel/text2").GetComponent<Text>().text =
        //        "(player1) " + pl1.GetComponent<PhotonView>().Owner.NickName.ToString() + ": " + pl1.GetComponent<stats>().gold.ToString() + "\n"
        //        + "(player2) " + pl2.GetComponent<PhotonView>().Owner.NickName.ToString() + ": " + pl2.GetComponent<stats>().gold.ToString() + "\n"
        //        + "(player3) " + pl3.GetComponent<PhotonView>().Owner.NickName.ToString() + ": " + pl3.GetComponent<stats>().gold.ToString();
        }

        
        //COLLISIONS
        if (bTurnEnd)
        {
            if (!bMove1 && !bMove2 && !bMove3)
            {  //end of move
                if ((pl1.transform.position == pl2.transform.position) || (pl1.transform.position == pl3.transform.position) || (pl2.transform.position == pl3.transform.position))
                {
                    if (pl1.transform.position == pl2.transform.position) { StartCoroutine(plCollision(pl1, pl2, pl3)); }
                    if (pl1.transform.position == pl3.transform.position) { StartCoroutine(plCollision(pl1, pl3, pl2)); }
                    if (pl2.transform.position == pl3.transform.position) { StartCoroutine(plCollision(pl2, pl3, pl1)); }
                }
                //else { bColEnd = true; }
                else {
                    Invoke("endTurnDelay", 2f);
                } //time to synch

                movEndCheck();
            }
            if (pl1.GetComponent<stats>().hitCount != 0)
            {
                // print("pl1 hits"); 
                if (lastMov1 - pl1.GetComponent<stats>().hitCount >= 0)
                {
                    pl1.transform.LookAt(p1Path[lastMov1 -pl1.GetComponent<stats>().hitCount +1]);       
                    pl1.transform.position = Vector3.MoveTowards(pl1.transform.position, p1Path[lastMov1 -pl1.GetComponent<stats>().hitCount], plMovSp * 1.05f * Time.deltaTime);
                    pv.RPC("pl1AC", RpcTarget.AllBuffered, "base.pl1-bump");
                }
            }
            if (pl2.GetComponent<stats>().hitCount != 0)
            {
                // print("pl2 hits"); 
                if (lastMov2 - pl2.GetComponent<stats>().hitCount >= 0)
                {
                    pl2.transform.LookAt(p2Path[lastMov2 -pl2.GetComponent<stats>().hitCount +1]);
                    pl2.transform.position = Vector3.MoveTowards(pl2.transform.position, p2Path[lastMov2 -pl2.GetComponent<stats>().hitCount], plMovSp * Time.deltaTime);
                    pv.RPC("pl2AC", RpcTarget.AllBuffered, "base.pl2-bump");
                }
            }
            if (pl3.GetComponent<stats>().hitCount != 0)
            {
                // print("pl3 hits"); 
                if (lastMov3 - pl3.GetComponent<stats>().hitCount >= 0)
                {
                    pl3.transform.LookAt(p3Path[lastMov3 -pl3.GetComponent<stats>().hitCount +1]);
                    pl3.transform.position = Vector3.MoveTowards(pl3.transform.position, p3Path[lastMov3 - pl3.GetComponent<stats>().hitCount], plMovSp * 0.95f * Time.deltaTime);
                    pv.RPC("pl3AC", RpcTarget.AllBuffered, "base.pl3-bump");
                }
            }
        }
    }
    IEnumerator plCollision(GameObject plA, GameObject plB, GameObject plC)
    {
        bColEnd = false;

        yield return new WaitForSeconds(0.2f);

        if (plA.transform.position == plB.transform.position)
        {
            CancelInvoke("endTurnDelay");

            plA.GetComponent<stats>().hitCount++;
            plB.GetComponent<stats>().hitCount++;
            if (plB.transform.position == plC.transform.position)
            {
                plC.GetComponent<stats>().hitCount++;
            }                       

            StopAllCoroutines();
            StartCoroutine(hitCountClear(plA, plB, plC));  //turn ends if no new collisions

            gameObject.GetComponent<goldControl>().goldSwap(plA, plB, plC);
        }

    }
    IEnumerator hitCountClear(GameObject plA, GameObject plB, GameObject plC)
    {
        yield return new WaitForSeconds(plMovSp * 1.5f); //speed!
        plA.GetComponent<stats>().hitCount = 0;
        plB.GetComponent<stats>().hitCount = 0;
        plC.GetComponent<stats>().hitCount = 0;
        
        pv.RPC("pl1AC", RpcTarget.AllBuffered, "base.pl1-stand");
        pv.RPC("pl2AC", RpcTarget.AllBuffered, "base.pl2-stand");
        pv.RPC("pl3AC", RpcTarget.AllBuffered, "base.pl3-stand");

        Invoke("endTurnDelay", 2f);  //time to synch
    }


    public void endTurn()  //runs from Update now  
    {
        //if (!GetComponent<PhotonView>().IsMine) { return; } //host only
        
        if (!pl1.GetComponent<PhotonView>().IsMine) { pl1.GetComponent<PhotonView>().RequestOwnership(); }
        if (!pl2.GetComponent<PhotonView>().IsMine) { pl2.GetComponent<PhotonView>().RequestOwnership(); }
        if (!pl3.GetComponent<PhotonView>().IsMine) { pl3.GetComponent<PhotonView>().RequestOwnership(); }

        p1Path = pl1.GetComponent<stats>().path;
        p1Path[0] = pl1.transform.position; //can return to start pos
        bMove1 = true;

        p2Path = pl2.GetComponent<stats>().path;
        p2Path[0] = pl2.transform.position;
        bMove2 = true;

        p3Path = pl3.GetComponent<stats>().path;
        p3Path[0] = pl3.transform.position;
        bMove3 = true;

        bTurnEnd = true;
    }

    void endTurnDelay()
    {
        bColEnd = true;
    }

    void movEndCheck()
    {
        if (bColEnd)
        {  //run once            
            gameObject.GetComponent<goldControl>().movEnd = true;
            gameObject.GetComponent<itemControl>().movEnd = true;
            bColEnd = false; bTurnEnd = false;
        }
    }


    void pl1move(int stepNo)
    {
        int iID = 0;
        iID = pl1.GetComponent<stats>().item1;  //active item1 use
        if ((iID == 1 || iID == 2 || iID == 3) && pl1.GetComponent<stats>().item1Targets[0] != Vector3.down)
        {
            for (int i = 1; i <= 5; i++) { p1Path[i] = Vector3.down; }
        }
        if ((iID == 7 || iID == 8 || iID == 9) && pl1.GetComponent<stats>().item1Targets[0] != Vector3.down)
        {
            for (int i = 1; i <= 5; i++) { p1Path[i] = Vector3.down; }

            pl1.GetComponent<stats>().item1 = 0;
            pl1.GetComponent<stats>().item1Targets = new Vector3[3] { Vector3.down, Vector3.down, Vector3.down };
        }
        iID = pl1.GetComponent<stats>().item2;  //active item2 use
        if ((iID == 1 || iID == 2 || iID == 3) && pl1.GetComponent<stats>().item2Targets[0] != Vector3.down)
        {
            for (int i = 1; i <= 5; i++) { p1Path[i] = Vector3.down; }
        }
        if ((iID == 7 || iID == 8 || iID == 9) && pl1.GetComponent<stats>().item2Targets[0] != Vector3.down)
        {
            for (int i = 1; i <= 5; i++) { p1Path[i] = Vector3.down; }

            pl1.GetComponent<stats>().item2 = 0;
            pl1.GetComponent<stats>().item2Targets = new Vector3[3] { Vector3.down, Vector3.down, Vector3.down };
        }


        if (p1Path[stepNo] != Vector3.down && (stepNo <= pl1dist) && !gameObject.GetComponent<itemControl>().slimeObstacle(p1Path[stepNo]))
        {
            pl1.transform.position = Vector3.MoveTowards(pl1.transform.position, p1Path[stepNo], plMovSp * 1.05f * Time.deltaTime);
            pl1.transform.LookAt(p1Path[stepNo]);
            if (pl1.transform.position == p1Path[stepNo]) { mov1StepNo++; }

            pv.RPC("pl1AC", RpcTarget.AllBuffered, "base.pl1-walk");
        }
        else { bMove1 = false; lastMov1 = mov1StepNo - 1; mov1StepNo = 1;  pv.RPC("pl1AC", RpcTarget.AllBuffered, "base.pl1-stand"); }
    }

    void pl2move(int stepNo)
    {
        if (pl2.GetComponent<stats>().pasSkillHex != Vector3.down || pl2.GetComponent<stats>().actSkillTrg[0] != Vector3.down) {   //if p2 attack or shoot is used
            pl2.GetComponent<stats>().pasSkillHex = p2Path[1];
            for (int i = 1; i <= 5; i++) { p2Path[i] = Vector3.down; }
        }


        int iID = 0;
        iID = pl2.GetComponent<stats>().item1;  //active item1 use
        if ((iID == 1 || iID == 2 || iID == 3) && pl2.GetComponent<stats>().item1Targets[0] != Vector3.down)
        {
            for (int i = 1; i <= 5; i++) { p2Path[i] = Vector3.down; }
        }
        if ((iID == 7 || iID == 8 || iID == 9) && pl2.GetComponent<stats>().item1Targets[0] != Vector3.down)
        {
            for (int i = 1; i <= 5; i++) { p2Path[i] = Vector3.down; }

            pl2.GetComponent<stats>().item1 = 0;
            pl2.GetComponent<stats>().item1Targets = new Vector3[3] { Vector3.down, Vector3.down, Vector3.down };
        }
        iID = pl2.GetComponent<stats>().item2;  //active item2 use
        if ((iID == 1 || iID == 2 || iID == 3) && pl2.GetComponent<stats>().item2Targets[0] != Vector3.down)
        {
            for (int i = 1; i <= 5; i++) { p2Path[i] = Vector3.down; }
        }
        if ((iID == 7 || iID == 8 || iID == 9) && pl2.GetComponent<stats>().item2Targets[0] != Vector3.down)
        {
            for (int i = 1; i <= 5; i++) { p2Path[i] = Vector3.down; }

            pl2.GetComponent<stats>().item2 = 0;
            pl2.GetComponent<stats>().item2Targets = new Vector3[3] { Vector3.down, Vector3.down, Vector3.down };
        }


        if (p2Path[stepNo] != Vector3.down && (stepNo <= pl2dist) && !gameObject.GetComponent<itemControl>().slimeObstacle(p2Path[stepNo]))
        {
            pl2.transform.position = Vector3.MoveTowards(pl2.transform.position, p2Path[stepNo], plMovSp * 1.00f * Time.deltaTime);
            pl2.transform.LookAt(p2Path[stepNo]);
            if (pl2.transform.position == p2Path[stepNo]) { mov2StepNo++; }

            pv.RPC("pl2AC", RpcTarget.AllBuffered, "base.pl2-walk");
        }
        else { bMove2 = false; lastMov2 = mov2StepNo - 1; mov2StepNo = 1;  pv.RPC("pl2AC", RpcTarget.AllBuffered, "base.pl2-stand"); }
    }
    void pl3move(int stepNo)
    {
        if (pl3.GetComponent<stats>().actSkillTrg[0] != Vector3.down) {  //if p3 active skill is used
            for (int i = 1; i <= 5; i++) { p3Path[i] = Vector3.down; }
        }


        int iID = 0;
        iID = pl3.GetComponent<stats>().item1;  //active item1 use
        if ((iID == 1 || iID == 2 || iID == 3) && pl3.GetComponent<stats>().item1Targets[0] != Vector3.down)
        {
            for (int i = 1; i <= 5; i++) { p3Path[i] = Vector3.down; }
        }
        if ((iID == 7 || iID == 8 || iID == 9) && pl3.GetComponent<stats>().item1Targets[0] != Vector3.down)
        {
            for (int i = 1; i <= 5; i++) { p3Path[i] = Vector3.down; }

            pl3.GetComponent<stats>().item1 = 0;
            pl3.GetComponent<stats>().item1Targets = new Vector3[3] { Vector3.down, Vector3.down, Vector3.down };
        }
        iID = pl3.GetComponent<stats>().item2;  //active item2 use
        if ((iID == 1 || iID == 2 || iID == 3) && pl3.GetComponent<stats>().item2Targets[0] != Vector3.down)
        {
            for (int i = 1; i <= 5; i++) { p3Path[i] = Vector3.down; }
        }
        if ((iID == 7 || iID == 8 || iID == 9) && pl3.GetComponent<stats>().item2Targets[0] != Vector3.down)
        {
            for (int i = 1; i <= 5; i++) { p3Path[i] = Vector3.down; }

            pl3.GetComponent<stats>().item2 = 0;
            pl3.GetComponent<stats>().item2Targets = new Vector3[3] { Vector3.down, Vector3.down, Vector3.down };
        }


        if (p3Path[stepNo] != Vector3.down && (stepNo <= pl3dist) && !gameObject.GetComponent<itemControl>().slimeObstacle(p3Path[stepNo]))
        {
            pl3.transform.position = Vector3.MoveTowards(pl3.transform.position, p3Path[stepNo], plMovSp * Time.deltaTime);

            pl3.transform.LookAt(p3Path[stepNo]);

            if (pl3.transform.position == p3Path[stepNo]) { mov3StepNo++; }

            pv.RPC("pl3AC", RpcTarget.AllBuffered, "base.pl3-walk");
        }
        else { bMove3 = false; lastMov3 = mov3StepNo - 1; mov3StepNo = 1;  pv.RPC("pl3AC", RpcTarget.AllBuffered, "base.pl3-stand"); }
    }

    [PunRPC] void pl1AC(string newState)
    {
        pl1anim.Play(newState);

        if (newState.Contains("skill")) { map.mapS.GetComponent<AudioSource>().PlayOneShot(map.mapS.skillClips[0]); }        
    }
    [PunRPC] void pl2AC(string newState)
    {
        pl2anim.Play(newState);
                
        if (newState.Contains("skill1")) { map.mapS.GetComponent<AudioSource>().PlayOneShot(map.mapS.skillClips[1]); }
        else if (newState.Contains("skill2")) { map.mapS.GetComponent<AudioSource>().PlayOneShot(map.mapS.skillClips[2]); }
    }
    [PunRPC] void pl3AC(string newState)
    {
        pl3anim.Play(newState);

        if (newState.Contains("skill")) { map.mapS.GetComponent<AudioSource>().PlayOneShot(map.mapS.skillClips[3]); }
    }

    [PunRPC] void RPC_messageSystem(string evnt, int t1, int t2)
    {
        if (evnt == "p1-chlg")
        {
            //print("怪盗はプレイヤー" + t1 +"に挑戦を送りました。");
        }
        if (evnt == "p1-chlg-win")
        {
            print("怪盗の挑戦は成功しました。");
        }

        if (evnt == "p2-hit")
        {
            print("プレイヤー" + t1 + "は海賊に殴られました。");
        }

        if (evnt == "p2-shoot")
        {
            print("プレイヤー"+ t1 + "は海賊に撃たれました。");
        }
        if (evnt == "p2-shoot-fail")
        {
            print("海賊は撃てない！ 後ろに妨げのプレイヤーがいます。");
        }

        if (evnt == "p3-gold")
        {
            print("探検家は金塊を採掘しました。");            
        }

        if (evnt == "item-bomb")
        {
            print("プレイヤー" + t1 +"はプレイヤー"+ t2+"によって爆破されました。");
        }

    }
}
