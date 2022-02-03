using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;

public class p2control : MonoBehaviour
{
    RaycastHit hit;
    Vector3 clHex;

    GameObject pFinder;
    [SerializeField] GameObject pathPref;
    GameObject[] destroy = new GameObject[5];
    int plDist;
    GameObject[] hexes = new GameObject[20];
    public GameObject[] nearHex = new GameObject[7];
    public Vector3[] path = new Vector3[6];

    public bool bShootPrep = false;
    [SerializeField] GameObject shootProbePref;
    GameObject shootProbeB, shootProbeF;
    public Vector3[] shootHexes = new Vector3[4] { Vector3.down, Vector3.down, Vector3.down, Vector3.down };  //can't stream GameObjects!
    Vector3 dir = new Vector3(-1, -1, -1);
    Ray detectRay = new Ray(Vector3.up, Vector3.down);
    RaycastHit hit2;
    int s = 0;

    public bool bPasAtk = false;  //passive attack

    Vector3 pfCor; //pathfinder Y-height correction
    int pfStepNo = 0;

    Text pNick, pGold;
    [SerializeField] Sprite[] p2logo;
    [SerializeField] Sprite p2a;
    [SerializeField] Sprite p2p;

    GameObject btnA, btnP, btnI1, btnI2, btnNT;
    GameObject toolTip;
    [Multiline] public string[] tips;
    GameObject itemTip;

    [SerializeField] GameObject bomb;
    int bombCntr = 0;
    [SerializeField] GameObject slime;
    GameObject s0 = null; GameObject s1 = null; GameObject s2 = null; GameObject s3 = null; GameObject s4 = null;
    bool bSlimePlaced = false;
    Vector3[] slimePos = new Vector3[10]; //for synch

    public bool bItem1a = false; public bool bItem2a = false;  //bombs
    public bool bItem1b = false; public bool bItem2b = false;  //boots
    public bool bItem1c = false; public bool bItem2c = false;  //slime

    Vector3[] itemTargets = new Vector3[3] { Vector3.down, Vector3.down, Vector3.down };

    int turnNo = 1;


    void Awake()
    {
        hexes = map.mapS.hexes;        

        plDist = gameObject.GetComponent<stats>().movDist;

        pFinder = GameObject.Find("pathFinder");
        clHex = pFinder.transform.position;
        pfCor = pFinder.transform.position + Vector3.down;

        btnA = GameObject.Find("ScreenCanvas/skillPanel/butAct");
        btnP = GameObject.Find("ScreenCanvas/skillPanel/butPas");
        btnI1 = GameObject.Find("ScreenCanvas/itemPanel/butItem1");
        btnI2 = GameObject.Find("ScreenCanvas/itemPanel/butItem2");         
        toolTip = GameObject.Find("ScreenCanvas/tipPanel");
        itemTip = GameObject.Find("ScreenCanvas/itemTipPanel");
        btnNT = GameObject.Find("ScreenCanvas/butTurn");
    }

    void Start() //can't set in Awake
    {
        pNick = GameObject.Find("ScreenCanvas/infoPanel/nicknameText").GetComponent<Text>();
        pNick.text = PhotonNetwork.NickName;

        pGold = GameObject.Find("ScreenCanvas/infoPanel/goldText").GetComponent<Text>();
        map.mapS.gBar1 = GameObject.FindGameObjectWithTag("gBar1");
        map.mapS.gBar2 = GameObject.FindGameObjectWithTag("gBar2");
        map.mapS.gBar3 = GameObject.FindGameObjectWithTag("gBar3");
    }


    void OnEnable()
    {
        //print("pl2 enabled");        
        if (GetComponent<PhotonView>().IsMine)
        {
            //turnNo = turnEnd.turnEndS.turnNo; 
            if (PhotonNetwork.CurrentRoom.CustomProperties["tNo"] != null) { turnNo = (int)PhotonNetwork.CurrentRoom.CustomProperties["tNo"]; }
            GameObject.Find("ScreenCanvas/turnText").GetComponent<Text>().text = turnNo.ToString();

            GetComponent<stats>().bMyTurn = true;
        }

        if (gameObject.GetComponent<stats>().movDist!=2) { gameObject.GetComponent<stats>().movDist = 2; } //1
        plDist = gameObject.GetComponent<stats>().movDist; //renew movDist


        //GameObject.Find("ScreenCanvas/charPanel/textPx").GetComponent<Text>().text = "P2";
        map.mapS.enemyLogo1.sprite = map.mapS.pl1logo[0]; map.mapS.enemyLogo2.sprite = map.mapS.pl3logo[0];        
        map.mapS.nick1.text = map.mapS.pl1.GetComponent<PhotonView>().Owner.NickName.ToString();
        map.mapS.nick2.text = map.mapS.pl3.GetComponent<PhotonView>().Owner.NickName.ToString();
        map.mapS.gold1.text = map.mapS.pl1.GetComponent<stats>().gold.ToString();
        map.mapS.gold2.text = map.mapS.pl3.GetComponent<stats>().gold.ToString();


        foreach (GameObject hex in hexes) //fix synch inaccuracy
        {
            if (hex != null)
            {
                if (Vector3.Distance(hex.transform.position, transform.position) < 0.3f)
                {
                    transform.position = hex.transform.position;
                }
            }
        }

        foreach (GameObject x in hexes)
        {
            if (x != null)
            {
                if (Vector3.Distance(x.transform.position, transform.position) < plDist+0.5f)
                {
                    x.transform.Find("hex").GetComponent<Renderer>().material.color = Color.red;
                }
                else
                {
                    x.transform.Find("hex").GetComponent<Renderer>().material.color = Color.gray;
                }
            }
        }

        path = new Vector3[6] { Vector3.zero, Vector3.down, Vector3.down, Vector3.down, Vector3.down, Vector3.down };
        pFinder.transform.position = transform.position + Vector3.up;
        clHex = transform.position;

        shootHexes = new Vector3[4] { Vector3.down, Vector3.down, Vector3.down, Vector3.down };
        bPasAtk = false;

        //logo & skill buttons
        GameObject.Find("ScreenCanvas/charPanel").GetComponent<Image>().sprite = p2logo[1];
        btnA.GetComponent<Image>().sprite = p2a;
        btnP.GetComponent<Image>().sprite = p2p;
        btnP.SetActive(true);   //testing
        btnP.GetComponent<Button>().interactable = true;

        btnA.GetComponent<Button>().onClick.RemoveAllListeners();
        btnA.GetComponent<Button>().onClick.AddListener(() => {
            if (!GetComponent<PhotonView>().IsMine) { return; }
            bShootPrep = !bShootPrep;            
        });
        btnP.GetComponent<Button>().onClick.RemoveAllListeners();
        btnP.GetComponent<Button>().onClick.AddListener(() => {
            if (!GetComponent<PhotonView>().IsMine) { return; }  
            bPasAtk = !bPasAtk;            
        });
        btnA.GetComponent<Button>().interactable = true;
        if (gameObject.GetComponent<stats>().skillCD != 0)
        {
            btnA.GetComponent<Button>().interactable = false;
            btnA.GetComponentInChildren<Text>().text = gameObject.GetComponent<stats>().skillCD.ToString();
        }
        else { btnA.GetComponentInChildren<Text>().text = ""; }

        btnNT.GetComponent<Button>().interactable = true;
        btnNT.GetComponent<Button>().onClick.RemoveAllListeners();
        btnNT.GetComponent<Button>().onClick.AddListener(() => {
            btnNT.GetComponent<Button>().interactable = false;
            Invoke("nextTurnDelay", 2f);  //synch
        });

        //item buttons
        btnI1.GetComponent<Button>().onClick.RemoveAllListeners();
        btnI2.GetComponent<Button>().onClick.RemoveAllListeners();

        if (gameObject.GetComponent<stats>().item1 != 0) {
            btnI1.GetComponent<Button>().interactable = true;
            btnI1.GetComponent<Image>().sprite = map.mapS.itemIcons[GetComponent<stats>().item1];
        }else { btnI1.GetComponent<Button>().interactable = false; btnI1.GetComponent<Image>().sprite = null; }
        if (gameObject.GetComponent<stats>().item2 != 0) {
            btnI2.GetComponent<Button>().interactable = true;
            btnI2.GetComponent<Image>().sprite = map.mapS.itemIcons[GetComponent<stats>().item2];
        }else { btnI2.GetComponent<Button>().interactable = false; btnI2.GetComponent<Image>().sprite = null; }


        if (gameObject.GetComponent<stats>().item1==1 || gameObject.GetComponent<stats>().item1==2 || gameObject.GetComponent<stats>().item1==3)  //bomb buttons
        {
            btnI1.GetComponent<Button>().onClick.AddListener(() => {
                bItem1a = !bItem1a;
                itemTargets = new Vector3[3] { Vector3.down, Vector3.down, Vector3.down };
                bombCntr = 0;
                map.mapS.bombClear();
            });
        }
        if (gameObject.GetComponent<stats>().item2==1 || gameObject.GetComponent<stats>().item2==2 || gameObject.GetComponent<stats>().item2==3)  
        {
            btnI2.GetComponent<Button>().onClick.AddListener(() => {
                bItem2a = !bItem2a;
                itemTargets = new Vector3[3] { Vector3.down, Vector3.down, Vector3.down };
                bombCntr = 0;
                map.mapS.bombClear();
            });
        }


        if (gameObject.GetComponent<stats>().item1==4 || gameObject.GetComponent<stats>().item1==5 || gameObject.GetComponent<stats>().item1==6)  //boots buttons
        {
            btnI1.GetComponent<Button>().onClick.AddListener(() => {
                bItem1b = !bItem1b;
                if (gameObject.GetComponent<stats>().movDist == 1) { gameObject.GetComponent<stats>().movDist += (gameObject.GetComponent<stats>().item1-3); }
                else { gameObject.GetComponent<stats>().movDist = 1; }

                plDist = gameObject.GetComponent<stats>().movDist; //renew movDist
                foreach (GameObject x in hexes)
                {
                    if (x != null)
                    {
                        if (Vector3.Distance(x.transform.position, transform.position) < plDist + 0.5f)
                        {
                            x.transform.Find("hex").GetComponent<Renderer>().material.color = Color.red;
                        }
                        else
                        {
                            x.transform.Find("hex").GetComponent<Renderer>().material.color = Color.gray;
                        }
                    }
                }

                if (bItem1b) { map.mapS.GetComponent<AudioSource>().PlayOneShot(map.mapS.itemClips[1]); }  //se
            });
        }
        if (gameObject.GetComponent<stats>().item2==4 || gameObject.GetComponent<stats>().item2==5 || gameObject.GetComponent<stats>().item2==6)  
        {
            btnI2.GetComponent<Button>().onClick.AddListener(() => {
                bItem2b = !bItem2b;
                if (gameObject.GetComponent<stats>().movDist == 1) { gameObject.GetComponent<stats>().movDist += (gameObject.GetComponent<stats>().item2-3); }
                else { gameObject.GetComponent<stats>().movDist = 1; }

                plDist = gameObject.GetComponent<stats>().movDist; //renew movDist
                foreach (GameObject x in hexes)
                {
                    if (x != null)
                    {
                        if (Vector3.Distance(x.transform.position, transform.position) < plDist + 0.5f)
                        {
                            x.transform.Find("hex").GetComponent<Renderer>().material.color = Color.red;
                        }
                        else
                        {
                            x.transform.Find("hex").GetComponent<Renderer>().material.color = Color.gray;
                        }
                    }
                }

                if (bItem2b) { map.mapS.GetComponent<AudioSource>().PlayOneShot(map.mapS.itemClips[1]); }  //se
            });
        }


        if (gameObject.GetComponent<stats>().item1==7 || gameObject.GetComponent<stats>().item1==8 || gameObject.GetComponent<stats>().item1==9)  //slime buttons
        {
            btnI1.GetComponent<Button>().onClick.AddListener(() => {
                bItem1c = !bItem1c;
                itemTargets = new Vector3[3] { Vector3.down, Vector3.down, Vector3.down };
                bSlimePlaced = false;
                foreach (GameObject slime in GameObject.FindGameObjectsWithTag("slimePref"))
                {
                    if (slime.GetComponent<slimeScript>().crTurn==turnNo && slime.GetComponent<slimeScript>().user==gameObject) { Destroy(slime); }
                }   
            });
        }
        if (gameObject.GetComponent<stats>().item2==7 || gameObject.GetComponent<stats>().item2==8 || gameObject.GetComponent<stats>().item2==9) 
        {            
            btnI2.GetComponent<Button>().onClick.AddListener(() => {
                bItem2c = !bItem2c;
                itemTargets = new Vector3[3] { Vector3.down, Vector3.down, Vector3.down };
                bSlimePlaced = false;
                foreach (GameObject slime in GameObject.FindGameObjectsWithTag("slimePref"))
                {
                    if (slime.GetComponent<slimeScript>().crTurn==turnNo && slime.GetComponent<slimeScript>().user==gameObject) { Destroy(slime); }
                }   
            });
        }


        if (gameObject.GetComponent<stats>().item1==10 || gameObject.GetComponent<stats>().item1==11 || gameObject.GetComponent<stats>().item1==12)  //jewel button
        {
            if (gameObject.GetComponent<stats>().item1Targets[0] == Vector3.down) { gameObject.GetComponent<stats>().item1Targets[0] = gameObject.transform.position; }
            btnI1.GetComponent<Button>().onClick.RemoveAllListeners(); //do nothing
        }
        if (gameObject.GetComponent<stats>().item2==10 || gameObject.GetComponent<stats>().item2==11 || gameObject.GetComponent<stats>().item2==12)  
        {
            if (gameObject.GetComponent<stats>().item2Targets[0] == Vector3.down) { gameObject.GetComponent<stats>().item2Targets[0] = gameObject.transform.position; }
            btnI2.GetComponent<Button>().onClick.RemoveAllListeners(); //do nothing
        }

        GetComponent<PhotonView>().RPC("RPC_AC", RpcTarget.AllBuffered, "base.pl2-stand"); //anim
    }

    void OnDisable()
    {
        GameObject.Find("ScreenCanvas/charPanel").GetComponent<Image>().sprite = p2logo[0];        

        gameObject.GetComponent<stats>().path = new Vector3[6] { Vector3.zero, Vector3.down, Vector3.down, Vector3.down, Vector3.down, Vector3.down };
        gameObject.GetComponent<stats>().path = path;
                
        if (bPasAtk) { 
            gameObject.GetComponent<stats>().pasSkillHex = path[1];  bPasAtk = false;
            //GetComponent<PhotonView>().RPC("RPC_AC", RpcTarget.AllBuffered, "base.pl2-skill1"); //anim
        }

        if (bShootPrep && shootHexes[0]!=Vector3.down) { 
            gameObject.GetComponent<stats>().actSkillTrg = shootHexes;
            gameObject.GetComponent<stats>().skillCD = 4;
            //GetComponent<PhotonView>().RPC("RPC_AC", RpcTarget.AllBuffered, "base.pl2-skill2"); //anim
        }
        bShootPrep = false;       


        if (bItem1a && itemTargets[0]!=Vector3.down)
        {
            gameObject.GetComponent<stats>().item1Targets = itemTargets;

            bItem1a = false; itemTargets = new Vector3[3] { Vector3.down, Vector3.down, Vector3.down };
        }
        if (bItem2a && itemTargets[0] != Vector3.down)
        {
            gameObject.GetComponent<stats>().item2Targets = itemTargets;

            bItem2a = false; itemTargets = new Vector3[3] { Vector3.down, Vector3.down, Vector3.down };
        }

        if (bItem1b) 
        {
            gameObject.GetComponent<stats>().item1 = 0;
            bItem1b = false;
        }
        if (bItem2b)
        {
            gameObject.GetComponent<stats>().item2 = 0;
            bItem1b = false;
        }

        if (bItem1c && itemTargets[0]!=Vector3.down)
        {
            if (gameObject.GetComponent<stats>().item1==7 || bSlimePlaced==true)
            {
                gameObject.GetComponent<stats>().item1Targets = itemTargets; //needs to stop movement
                //gameObject.GetComponent<stats>().item1 = 0; 

                bItem1c = false; itemTargets = new Vector3[3] { Vector3.down, Vector3.down, Vector3.down };                
            }
        }
        if (bItem2c && itemTargets[0]!=Vector3.down)
        {
            if (gameObject.GetComponent<stats>().item2==7 || bSlimePlaced==true)
            {                         
                gameObject.GetComponent<stats>().item2Targets = itemTargets;                

                bItem2c = false; itemTargets = new Vector3[3] { Vector3.down, Vector3.down, Vector3.down };
            }
        }
        bSlimePlaced = false;

        for (int k = 0; k < slimePos.Length; k++) { slimePos[k] = Vector3.down; }
        int i = 0;
        foreach (GameObject slime in GameObject.FindGameObjectsWithTag("slimePref"))
        {
            if (slime.GetComponent<slimeScript>().user == gameObject && slime.GetComponent<slimeScript>().crTurn == turnNo)
            {
                slimePos[i] = slime.transform.position; i++;
            }
        }        
        GetComponent<PhotonView>().RPC("RPC_newSlimes", RpcTarget.OthersBuffered, slimePos);

    }


    void Update()
    {
        if (!GetComponent<PhotonView>().IsMine)
        {
            return;
        }

        pGold.text = gameObject.GetComponent<stats>().gold.ToString();

        pfCor = pFinder.transform.position + Vector3.down;

        Ray mRay = Camera.main.ScreenPointToRay(Input.mousePosition);  //click detect
        if (Input.GetMouseButtonDown(0)) 
        {
            if (Physics.Raycast(mRay, out hit))
            {
                clHex = hit.collider.transform.position;

                path = new Vector3[6] { Vector3.zero, Vector3.down, Vector3.down, Vector3.down, Vector3.down, Vector3.down };
                pfCor = transform.position;

                InvokeRepeating("pfRoutine", 0f, 0.1f);

                destroy = GameObject.FindGameObjectsWithTag("pathPref");
                for (int i = 0; i < destroy.Length; i++) { Destroy(destroy[i]); }

                map.mapS.GetComponent<AudioSource>().PlayOneShot(map.mapS.otherClips[0]);  //se
            }
        }
        if (clHex == pfCor)
        {
            CancelInvoke("pfRoutine"); pfStepNo = 0;

            for (int i = 1; i <= plDist; i++) //path highlights
            {
                Instantiate(pathPref, path[i], transform.rotation);
            }

            map.mapS.hexInfo(clHex, transform.position); //hex info

            clHex = new Vector3(-10, -10, -10);
        }

        if (bShootPrep) { pl2shootPrep(); }

        if (bItem1a || bItem1c) { item1Prep(); }
        if (bItem2a || bItem2c) { item2Prep(); }

        if (Input.GetKeyDown(KeyCode.KeypadPlus))  //pass turn (CHECK BUTTON TOO)
        {
            //GetComponent<PhotonView>().RPC("RPC_pl3start", RpcTarget.All);
            GetComponent<PhotonView>().RPC("RPC_pl3start", GameObject.FindGameObjectWithTag("player3").GetComponent<PhotonView>().Owner);
            GetComponent<p2control>().enabled = false;
        }

        curBtn();

        blockBtn();
    }


    void nearHexSearch() // +correction to hex pos (x,0,z)!!!
    {
        int j = 1;
        for (int i = 1; i <= hexes.Length-1; i++)
        {
            if (Vector3.Distance(hexes[i].transform.position, pfCor) < 1.15f) //hex size!
            {
                if (hexes[i].transform.position != pfCor)
                {                    
                   nearHex[j] = hexes[i];
                   j++; 
                }
            }            
        }

    }


    void pathFind()
    {
        int s = 1;
        path[pfStepNo] = nearHex[1].transform.position;
        float nearest = Vector3.Distance(nearHex[1].transform.position, clHex);

        for (int i = 1; i <= nearHex.Length-1; i++)
        {
            if (Vector3.Distance(nearHex[i].transform.position, clHex) < nearest
            && Vector3.Distance(nearHex[i].transform.position, pfCor) < 1.5f) //hex size!
            {
                path[pfStepNo] = nearHex[i].transform.position;
                nearest = Vector3.Distance(nearHex[i].transform.position, clHex);
                s = i; //nearest hex number in nearHex[]
            }
        }

        pFinder.transform.position = nearHex[s].transform.position + Vector3.up;
    }
    void pfRoutine()
    {
        pfStepNo++;
        nearHexSearch();
        pathFind();       
    }

    void pl2shootPrep() //if bShoot //if path[1] //only once!
    {     
        if (Input.GetMouseButtonUp(0) && (path[1] != Vector3.down))  //on mouse click
        {
            shootHexes = new Vector3[4] { Vector3.down, Vector3.down, Vector3.down, Vector3.down };

            dir = (path[1] - gameObject.transform.position).normalized;

            shootProbeB = Instantiate(shootProbePref, gameObject.transform.position+Vector3.up, Quaternion.identity);                        

        }  

        if (shootProbeB != null)
        {            
            detectRay = new Ray(shootProbeB.transform.position, Vector3.down);
            //Debug.DrawRay(shootProbeB.transform.position, Vector3.down * 2, Color.black);

            shootProbeB.transform.Translate(-dir * Time.deltaTime *15);

            if (Physics.Raycast(detectRay, out hit2)){
                if(hit2.collider.gameObject.tag=="ground" && hit2.collider.gameObject.transform.position!=gameObject.transform.position)
                {
                    if (hit2.collider.gameObject!=null)
                    {
                        shootHexes[0] = hit2.collider.gameObject.transform.position;
                    }                                   

                    Destroy(shootProbeB);
                    shootProbeF = Instantiate(shootProbePref, gameObject.transform.position+Vector3.up, Quaternion.identity);
                    s = 0;
                }
            }

            if(shootProbeB.gameObject.transform.position.z>5 || shootProbeB.gameObject.transform.position.z<-5 
                || shootProbeB.gameObject.transform.position.x>5 || shootProbeB.gameObject.transform.position.z<-5)
            {                
                if (shootHexes[0] == Vector3.down) { print("撃てない！ 後ろにマスがありません。"); }                

                Destroy(shootProbeB);
                shootProbeF = Instantiate(shootProbePref, gameObject.transform.position + Vector3.up, Quaternion.identity);
                s = 0;
            }
            
        }

        if (shootProbeF != null)  //on shootProbeB destoy
        {
            detectRay = new Ray(shootProbeF.transform.position, Vector3.down);
            Debug.DrawRay(shootProbeF.transform.position, Vector3.down * 2, Color.black);

            shootProbeF.transform.Translate(dir * Time.deltaTime * 15);

            if (Physics.Raycast(detectRay, out hit2))
            {
                if (hit2.collider.gameObject.tag == "ground" && hit2.collider.gameObject.transform.position != gameObject.transform.position)
                {
                    if (hit2.collider.gameObject.transform.position != shootHexes[s])
                    {
                        s++;
                        shootHexes[s] = hit2.collider.gameObject.transform.position;
                    }
                }
            }

            if(shootProbeF.gameObject.transform.position.z>5 || shootProbeF.gameObject.transform.position.z<-5 
                || shootProbeF.gameObject.transform.position.x>5 || shootProbeF.gameObject.transform.position.z<-5)
            {            
               Destroy(shootProbeF);              
            }
        }
        
    }


    void item1Prep()  //define target hexes for item in slot1
    {
        int itemID = gameObject.GetComponent<stats>().item1;        

        if (itemID==1 || itemID==2 || itemID==3)  //bombs
        { 
            Ray mRay = Camera.main.ScreenPointToRay(Input.mousePosition);              

            if (Input.GetMouseButtonUp(0) && bombCntr<itemID) 
            {
                if (Physics.Raycast(mRay, out hit))
                {
                    clHex = hit.collider.transform.position;

                    itemTargets[bombCntr] = clHex;
                    bombCntr++;                    

                    foreach(Vector3 pos in itemTargets)
                    {
                        if(pos != Vector3.down) { Instantiate(bomb, pos, Quaternion.identity); }
                    }                    
                }
            }

        }

        if (itemID==7 || itemID==8 || itemID==9)  //slime
        {
            Ray mRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Input.GetMouseButtonUp(0))
            {
                if (itemTargets[0] == Vector3.down)
                {
                    if (Physics.Raycast(mRay, out hit))
                    {
                        map.mapS.bombClear();

                        clHex = hit.collider.transform.position;

                        itemTargets[0] = clHex;
                        s0 = Instantiate(slime, itemTargets[0], Quaternion.identity);

                        if (itemID==8 && itemTargets[0]!=Vector3.down)  //slime 2 (3 hexes)
                        {
                            s1 = Instantiate(slime, itemTargets[0] + Vector3.right, Quaternion.identity);                           

                            s2 = Instantiate(slime, itemTargets[0] + Vector3.right, Quaternion.identity);
                            s2.transform.RotateAround(s0.transform.position, Vector3.up, -60f);                                             
                        }
                        else if (itemID==9 && itemTargets[0]!=Vector3.down)  //slime 3 (5 hexes)
                        {
                            s1 = Instantiate(slime, itemTargets[0] + Vector3.right, Quaternion.identity);

                            s2 = Instantiate(slime, itemTargets[0] + Vector3.right, Quaternion.identity);
                            s2.transform.RotateAround(s0.transform.position, Vector3.up, -60f);

                            s3 = Instantiate(slime, itemTargets[0] + Vector3.left, Quaternion.identity);

                            s4 = Instantiate(slime, itemTargets[0] + Vector3.left, Quaternion.identity);
                            s4.transform.RotateAround(s0.transform.position, Vector3.up, 60f);
                        }
                    }
                }
                else
                {  //on 2nd click
                    if (itemID==8 || itemID==9)
                    {                
                        bSlimePlaced = true;
                    }
                }
            }
            if (itemID==8 && !bSlimePlaced)
            {
                if (Input.GetAxis("Mouse ScrollWheel") < 0f)
                {
                    s1?.transform.RotateAround(s0.transform.position, Vector3.up, 60f);                    
                    s2?.transform.RotateAround(s0.transform.position, Vector3.up, 60f);                    
                }
                if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                {
                    s1?.transform.RotateAround(s0.transform.position, Vector3.up, -60f);                    
                    s2?.transform.RotateAround(s0.transform.position, Vector3.up, -60f);                    
                }
            }
            if (itemID==9 && !bSlimePlaced)
            {
                if (Input.GetAxis("Mouse ScrollWheel") < 0f)
                {
                    s1?.transform.RotateAround(s0.transform.position, Vector3.up, 60f);
                    s2?.transform.RotateAround(s0.transform.position, Vector3.up, 60f);
                    s3?.transform.RotateAround(s0.transform.position, Vector3.up, 60f);
                    s4?.transform.RotateAround(s0.transform.position, Vector3.up, 60f);
                }
                if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                {
                    s1?.transform.RotateAround(s0.transform.position, Vector3.up, -60f);
                    s2?.transform.RotateAround(s0.transform.position, Vector3.up, -60f);
                    s3?.transform.RotateAround(s0.transform.position, Vector3.up, -60f);
                    s4?.transform.RotateAround(s0.transform.position, Vector3.up, -60f);
                }
            }

        }
    }
    void item2Prep()  //define target hexes for item in slot2
    {
        int itemID = gameObject.GetComponent<stats>().item2;        

        if (itemID==1 || itemID==2 || itemID==3)  //bombs
        { 
            Ray mRay = Camera.main.ScreenPointToRay(Input.mousePosition);              

            if (Input.GetMouseButtonUp(0) && bombCntr<itemID) 
            {
                if (Physics.Raycast(mRay, out hit))
                {
                    clHex = hit.collider.transform.position;

                    itemTargets[bombCntr] = clHex;
                    bombCntr++;                    

                    foreach(Vector3 pos in itemTargets)
                    {
                        if(pos != Vector3.down) { Instantiate(bomb, pos, Quaternion.identity); }
                    }                    
                }
            }

        }

        if (itemID==7 || itemID==8 || itemID==9)  //slime
        {
            Ray mRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Input.GetMouseButtonUp(0))
            {
                if (itemTargets[0] == Vector3.down)
                {
                    if (Physics.Raycast(mRay, out hit))
                    {
                        map.mapS.bombClear();

                        clHex = hit.collider.transform.position;

                        itemTargets[0] = clHex;
                        s0 = Instantiate(slime, itemTargets[0], Quaternion.identity);

                        if (itemID==8 && itemTargets[0]!=Vector3.down)  //slime 2 (3 hexes)
                        {
                            s1 = Instantiate(slime, itemTargets[0] + Vector3.right, Quaternion.identity);                           

                            s2 = Instantiate(slime, itemTargets[0] + Vector3.right, Quaternion.identity);
                            s2.transform.RotateAround(s0.transform.position, Vector3.up, -60f);                                             
                        }
                        else if (itemID==9 && itemTargets[0]!=Vector3.down)  //slime 3 (5 hexes)
                        {
                            s1 = Instantiate(slime, itemTargets[0] + Vector3.right, Quaternion.identity);

                            s2 = Instantiate(slime, itemTargets[0] + Vector3.right, Quaternion.identity);
                            s2.transform.RotateAround(s0.transform.position, Vector3.up, -60f);

                            s3 = Instantiate(slime, itemTargets[0] + Vector3.left, Quaternion.identity);

                            s4 = Instantiate(slime, itemTargets[0] + Vector3.left, Quaternion.identity);
                            s4.transform.RotateAround(s0.transform.position, Vector3.up, 60f);
                        }
                    }
                }
                else
                {  //on 2nd click
                    if (itemID==8 || itemID==9)
                    {                
                        bSlimePlaced = true;
                    }
                }
            }
            if (itemID==8 && !bSlimePlaced)
            {
                if (Input.GetAxis("Mouse ScrollWheel") < 0f)
                {
                    s1?.transform.RotateAround(s0.transform.position, Vector3.up, 60f);                    
                    s2?.transform.RotateAround(s0.transform.position, Vector3.up, 60f);                    
                }
                if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                {
                    s1?.transform.RotateAround(s0.transform.position, Vector3.up, -60f);                    
                    s2?.transform.RotateAround(s0.transform.position, Vector3.up, -60f);                    
                }
            }
            if (itemID==9 && !bSlimePlaced)
            {
                if (Input.GetAxis("Mouse ScrollWheel") < 0f)
                {
                    s1?.transform.RotateAround(s0.transform.position, Vector3.up, 60f);
                    s2?.transform.RotateAround(s0.transform.position, Vector3.up, 60f);
                    s3?.transform.RotateAround(s0.transform.position, Vector3.up, 60f);
                    s4?.transform.RotateAround(s0.transform.position, Vector3.up, 60f);
                }
                if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                {
                    s1?.transform.RotateAround(s0.transform.position, Vector3.up, -60f);
                    s2?.transform.RotateAround(s0.transform.position, Vector3.up, -60f);
                    s3?.transform.RotateAround(s0.transform.position, Vector3.up, -60f);
                    s4?.transform.RotateAround(s0.transform.position, Vector3.up, -60f);
                }
            }

        }
    }


    void curBtn()
    {
        Vector3 curPos = Input.mousePosition;

        if (Vector3.Distance(btnA.transform.position, curPos) < 40f)
        {
            toolTip.transform.position = btnA.transform.position + new Vector3(25f, 140f, 0f);
            toolTip.GetComponentInChildren<Text>().text = tips[0];
            toolTip.SetActive(true);
        }
        else if (Vector3.Distance(btnP.transform.position, curPos) < 40f)
        {
            toolTip.transform.position = btnP.transform.position + new Vector3(25f, 140f, 0f);
            toolTip.GetComponentInChildren<Text>().text = tips[1];
            toolTip.SetActive(true);
        }
        else { toolTip.SetActive(false); }

        if (Vector3.Distance(btnI1.transform.position, curPos) < 30f)
        {
            itemTip.transform.position = btnI1.transform.position + new Vector3(50f, 100f, 0f);
            itemTip.GetComponentInChildren<Text>().text = map.mapS.itemTips[GetComponent<stats>().item1];
            itemTip.SetActive(true);
        }
        else if (Vector3.Distance(btnI2.transform.position, curPos) < 30f)
        {
            itemTip.transform.position = btnI2.transform.position + new Vector3(50f, 100f, 0f);
            itemTip.GetComponentInChildren<Text>().text = map.mapS.itemTips[GetComponent<stats>().item2];
            itemTip.SetActive(true);
        }
        else { itemTip.SetActive(false); }
    }

    void blockBtn()
    {
        if (bPasAtk)
        {
            btnA.GetComponent<Button>().interactable = false;
            btnI1.GetComponent<Button>().interactable = false;
            btnI2.GetComponent<Button>().interactable = false;
        }
        else if (bShootPrep)
        {
            btnP.GetComponent<Button>().interactable = false;
            btnI1.GetComponent<Button>().interactable = false;
            btnI2.GetComponent<Button>().interactable = false;
        }
        else if (bItem1a || bItem1b || bItem1c)
        {
            btnA.GetComponent<Button>().interactable = false;
            btnP.GetComponent<Button>().interactable = false;
            btnI2.GetComponent<Button>().interactable = false;
        }
        else if (bItem2a || bItem2b || bItem2c)
        {
            btnA.GetComponent<Button>().interactable = false;
            btnP.GetComponent<Button>().interactable = false;
            btnI1.GetComponent<Button>().interactable = false;
        }
        else
        {
            btnP.GetComponent<Button>().interactable = true;
            if (GetComponent<stats>().skillCD == 0) { btnA.GetComponent<Button>().interactable = true; }
            if (GetComponent<stats>().item1 != 0)   { btnI1.GetComponent<Button>().interactable = true; }
            if (GetComponent<stats>().item1 != 0)   { btnI2.GetComponent<Button>().interactable = true; }      
        }
    }


    [PunRPC] void RPC_AC(string newState)
    {
        GameObject.FindGameObjectWithTag("player2").GetComponentInChildren<Animator>().Play(newState);

        if (newState.Contains("skill1")) { map.mapS.GetComponent<AudioSource>().PlayOneShot(map.mapS.skillClips[1]); }
        else if (newState.Contains("skill2")) { map.mapS.GetComponent<AudioSource>().PlayOneShot(map.mapS.skillClips[2]); }
    }



    [PunRPC] void RPC_newSlimes(Vector3[] newSlimes)
    {
        foreach (GameObject hex in hexes)
        {
            foreach (Vector3 sl in newSlimes)
            {
                if (sl != Vector3.down && hex != null)
                {
                    if (Vector3.Distance(hex.transform.position, sl) < 0.5f)
                    {                        
                        Instantiate(this.slime, hex.transform.position, Quaternion.identity);  //sl?
                    }
                }
            }
        }
    }


    [PunRPC] public void RPC_pl3start()
    {
        GameObject.FindGameObjectWithTag("player3").GetComponent<p3control>().enabled = true;
        //GameObject.FindGameObjectWithTag("player2").GetComponent<p2control>().enabled = false;
    }

    private void nextTurnDelay()
    {        
        GetComponent<stats>().bMyTurn = false;
        GetComponent<p2control>().enabled = false;
        GetComponent<PhotonView>().RPC("RPC_pl3start", GameObject.FindGameObjectWithTag("player3").GetComponent<PhotonView>().Owner);
    }
}

