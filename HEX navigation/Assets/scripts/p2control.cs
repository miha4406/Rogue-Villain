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

    bool bShootPrep = false;
    [SerializeField] GameObject shootProbePref;
    GameObject shootProbeB, shootProbeF;
    public Vector3[] shootHexes = new Vector3[4] { Vector3.down, Vector3.down, Vector3.down, Vector3.down };  //can't stream GameObjects!
    Vector3 dir = new Vector3(-1, -1, -1);
    Ray detectRay = new Ray(Vector3.up, Vector3.down);
    RaycastHit hit2;
    int s = 0;

    bool bPasAtk = false;  //passive attack

    Vector3 pfCor; //pathfinder Y-height correction
    int pfStepNo = 0;

    Text pNick, pGold;
    GameObject pl1, pl2, pl3;

    [SerializeField] Sprite p2logo;
    [SerializeField] Sprite p2a;
    [SerializeField] Sprite p2p;

    [SerializeField] GameObject bomb;
    int bombCntr = 0;
    [SerializeField] GameObject slime;
    GameObject s0 = null; GameObject s1 = null; GameObject s2 = null; GameObject s3 = null; GameObject s4 = null;
    bool bSlimePlaced = false;

    public bool bItem1a = false; public bool bItem2a = false;  //bombs
    public bool bItem1b = false; public bool bItem2b = false;  //boots
    public bool bItem1c = false; public bool bItem2c = false;  //slime

    Vector3[] itemTargets = new Vector3[3] { Vector3.down, Vector3.down, Vector3.down };

    int turnNo;


    void Awake()
    {
        pFinder = GameObject.Find("pathFinder");

        plDist = gameObject.GetComponent<stats>().movDist;

        hexes = map.mapS.hexes;

        clHex = pFinder.transform.position;

        pfCor = pFinder.transform.position + Vector3.down;        
    }

    void Start() //can't set in Awake
    {
        pNick = GameObject.Find("ScreenCanvas/Panel2/nicknameText").GetComponent<Text>();
        pNick.text = PhotonNetwork.NickName;
        pGold = GameObject.Find("ScreenCanvas/Panel2/goldText").GetComponent<Text>();

        pl1 = GameObject.FindGameObjectWithTag("player1");
        pl2 = gameObject;
        pl3 = GameObject.FindGameObjectWithTag("player3");
    }


    void OnEnable()
    {
        if (!GetComponent<PhotonView>().IsMine)
        {
            return;
        }

        turnNo = turnEnd.turnEndS.turnNo;

        if (gameObject.GetComponent<stats>().movDist!=1) { gameObject.GetComponent<stats>().movDist = 1; }
        plDist = gameObject.GetComponent<stats>().movDist; //renew movDist

        foreach (GameObject x in hexes)
        {
            if (x != null)
            {
                if (Vector3.Distance(x.transform.position, transform.position) < plDist+0.5f)
                {
                    x.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
                    x.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.yellow);
                }
                else
                {
                    x.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.black);
                }
            }
        }

        path = new Vector3[6] { Vector3.zero, Vector3.down, Vector3.down, Vector3.down, Vector3.down, Vector3.down };
        pFinder.transform.position = transform.position + Vector3.up;
        clHex = transform.position;

        shootHexes = new Vector3[4] { Vector3.down, Vector3.down, Vector3.down, Vector3.down };

        bPasAtk = false;

        GameObject.Find("ScreenCanvas/logoImage").GetComponent<Image>().sprite = p2logo;
        GameObject.Find("ScreenCanvas/butAct").GetComponent<Image>().sprite = p2a;
        GameObject.Find("ScreenCanvas/butPas").GetComponent<Image>().sprite = p2p;
        GameObject.Find("ScreenCanvas/butPas").GetComponent<Button>().interactable = true;
        GameObject.Find("ScreenCanvas/butAct").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("ScreenCanvas/butAct").GetComponent<Button>().onClick.AddListener(() => {
            bShootPrep = !bShootPrep;            
        });
        //GameObject.Find("ScreenCanvas/butPas").GetComponent<Button>().onClick.RemoveAllListeners(); //
        GameObject.Find("ScreenCanvas/butPas").GetComponent<Button>().onClick.AddListener(() => {
            if (!GetComponent<PhotonView>().IsMine) { return; }  //?
            bPasAtk = !bPasAtk;
            print(bPasAtk);
        });
        GameObject.Find("ScreenCanvas/butAct").GetComponent<Button>().interactable = true;
        if (gameObject.GetComponent<stats>().skillCD != 0)
        {
            GameObject.Find("ScreenCanvas/butAct").GetComponent<Button>().interactable = false;
            GameObject.Find("ScreenCanvas/butAct").GetComponentInChildren<Text>().text = gameObject.GetComponent<stats>().skillCD.ToString();
        }
        else { GameObject.Find("ScreenCanvas/butAct").GetComponentInChildren<Text>().text = ""; }


        if(gameObject.GetComponent<stats>().item1 != 0) {   //item buttons
            GameObject.Find("ScreenCanvas/butItem1").GetComponent<Button>().interactable = true;
            GameObject.Find("ScreenCanvas/butItem1").GetComponentInChildren<Text>().text = gameObject.GetComponent<stats>().item1.ToString();
        }
        else { GameObject.Find("ScreenCanvas/butItem1").GetComponent<Button>().interactable = false; }
        if (gameObject.GetComponent<stats>().item2 != 0)
        {   
            GameObject.Find("ScreenCanvas/butItem2").GetComponent<Button>().interactable = true;
            GameObject.Find("ScreenCanvas/butItem2").GetComponentInChildren<Text>().text = gameObject.GetComponent<stats>().item2.ToString();
        }
        else { GameObject.Find("ScreenCanvas/butItem2").GetComponent<Button>().interactable = false; }


        if (gameObject.GetComponent<stats>().item1==1 || gameObject.GetComponent<stats>().item1==2 || gameObject.GetComponent<stats>().item1==3)  //bomb buttons
        {
            GameObject.Find("ScreenCanvas/butItem1").GetComponent<Button>().onClick.RemoveAllListeners();
            GameObject.Find("ScreenCanvas/butItem1").GetComponent<Button>().onClick.AddListener(() => {
                bItem1a = !bItem1a;
                itemTargets = new Vector3[3] { Vector3.down, Vector3.down, Vector3.down };
                bombCntr = 0;
                GameObject.Find("GameLogic").GetComponent<itemControl>().bombClear();
            });
        }
        if (gameObject.GetComponent<stats>().item2==1 || gameObject.GetComponent<stats>().item2==2 || gameObject.GetComponent<stats>().item2==3)  
        {
            GameObject.Find("ScreenCanvas/butItem2").GetComponent<Button>().onClick.RemoveAllListeners();
            GameObject.Find("ScreenCanvas/butItem2").GetComponent<Button>().onClick.AddListener(() => {
                bItem2a = !bItem2a;
                itemTargets = new Vector3[3] { Vector3.down, Vector3.down, Vector3.down };
                bombCntr = 0;
                GameObject.Find("GameLogic").GetComponent<itemControl>().bombClear();
            });
        }


        if (gameObject.GetComponent<stats>().item1==4 || gameObject.GetComponent<stats>().item1==5 || gameObject.GetComponent<stats>().item1==6)  //boots buttons
        {
            GameObject.Find("ScreenCanvas/butItem1").GetComponent<Button>().onClick.RemoveAllListeners();
            GameObject.Find("ScreenCanvas/butItem1").GetComponent<Button>().onClick.AddListener(() => {
                bItem1b = !bItem1b;
                if (gameObject.GetComponent<stats>().movDist == 1) { gameObject.GetComponent<stats>().movDist += (gameObject.GetComponent<stats>().item1-3); }
                else { gameObject.GetComponent<stats>().movDist = 1; }

                foreach (GameObject x in hexes)
                {
                    if (x != null)
                    {
                        if (Vector3.Distance(x.transform.position, transform.position) < gameObject.GetComponent<stats>().movDist+0.5f)
                        {
                            x.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
                            x.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.yellow);
                        }
                        else
                        {
                            x.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.black);
                        }
                    }
                }
            });
        }
        if (gameObject.GetComponent<stats>().item2==4 || gameObject.GetComponent<stats>().item2==5 || gameObject.GetComponent<stats>().item2==6)  
        {
            GameObject.Find("ScreenCanvas/butItem2").GetComponent<Button>().onClick.RemoveAllListeners();
            GameObject.Find("ScreenCanvas/butItem2").GetComponent<Button>().onClick.AddListener(() => {
                bItem2b = !bItem2b;
                if (gameObject.GetComponent<stats>().movDist == 1) { gameObject.GetComponent<stats>().movDist += (gameObject.GetComponent<stats>().item2-3); }
                else { gameObject.GetComponent<stats>().movDist = 1; } 

                foreach (GameObject x in hexes)
                {
                    if (x != null)
                    {
                        if (Vector3.Distance(x.transform.position, transform.position) < gameObject.GetComponent<stats>().movDist + 0.5f)
                        {
                            x.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
                            x.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.yellow);
                        }
                        else
                        {
                            x.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.black);
                        }
                    }
                }
            });
        }


        if (gameObject.GetComponent<stats>().item1==7 || gameObject.GetComponent<stats>().item1==8 || gameObject.GetComponent<stats>().item1==9)  //slime buttons
        {
            GameObject.Find("ScreenCanvas/butItem1").GetComponent<Button>().onClick.RemoveAllListeners();
            GameObject.Find("ScreenCanvas/butItem1").GetComponent<Button>().onClick.AddListener(() => {
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
            GameObject.Find("ScreenCanvas/butItem2").GetComponent<Button>().onClick.RemoveAllListeners();
            GameObject.Find("ScreenCanvas/butItem2").GetComponent<Button>().onClick.AddListener(() => {
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
            GameObject.Find("ScreenCanvas/butItem1").GetComponent<Button>().onClick.RemoveAllListeners(); //do nothing
        }
        if (gameObject.GetComponent<stats>().item2==10 || gameObject.GetComponent<stats>().item2==11 || gameObject.GetComponent<stats>().item2==12)  
        {
            if (gameObject.GetComponent<stats>().item2Targets[0] == Vector3.down) { gameObject.GetComponent<stats>().item2Targets[0] = gameObject.transform.position; }
            GameObject.Find("ScreenCanvas/butItem2").GetComponent<Button>().onClick.RemoveAllListeners(); //do nothing
        }

    }

    void OnDisable()
    {
        if (!GetComponent<PhotonView>().IsMine)
        {
            return;
        }

        gameObject.GetComponent<stats>().path = new Vector3[6] { Vector3.zero, Vector3.down, Vector3.down, Vector3.down, Vector3.down, Vector3.down };
        gameObject.GetComponent<stats>().path = path;

        //print(bPasAtk);
        if (bPasAtk) { gameObject.GetComponent<stats>().pasSkillHex = path[1];  bPasAtk = false; }

        if (bShootPrep && shootHexes[0]!=Vector3.down) { 
            gameObject.GetComponent<stats>().actSkillTrg = shootHexes;
            gameObject.GetComponent<stats>().skillCD = 4;
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

    }


    void Update()
    {
        if (!GetComponent<PhotonView>().IsMine)
        {
            return;
        }

        pGold.text = "Gold: " + gameObject.GetComponent<stats>().gold.ToString();

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
            }
        }
        if (clHex == pfCor)
        {
            CancelInvoke("pfRoutine"); pfStepNo = 0;

            for (int i = 1; i <= plDist; i++) //path highlights
            {
                Instantiate(pathPref, path[i], transform.rotation);
            }

            clHex = new Vector3(-10, -10, -10);
        }

        if (bShootPrep) { pl2shootPrep(); }

        if (bItem1a || bItem1c) { item1Prep(); }
        if (bItem2a || bItem2c) { item2Prep(); }

        if (Input.GetKeyDown(KeyCode.Return))  //on Enter
        {
            GetComponent<PhotonView>().RPC("RPC_pl3start", RpcTarget.All);
        }
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
                if (shootHexes[0] == Vector3.down) { print("Can't shoot! No space behind."); }                

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
                        GameObject.Find("GameLogic").GetComponent<itemControl>().bombClear();

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
                    s1.transform.RotateAround(s0.transform.position, Vector3.up, 60f);                    
                    s2.transform.RotateAround(s0.transform.position, Vector3.up, 60f);                    
                }
                if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                {
                    s1.transform.RotateAround(s0.transform.position, Vector3.up, -60f);                    
                    s2.transform.RotateAround(s0.transform.position, Vector3.up, -60f);                    
                }
            }
            if (itemID==9 && !bSlimePlaced)
            {
                if (Input.GetAxis("Mouse ScrollWheel") < 0f)
                {
                    s1.transform.RotateAround(s0.transform.position, Vector3.up, 60f);
                    s2.transform.RotateAround(s0.transform.position, Vector3.up, 60f);
                    s3.transform.RotateAround(s0.transform.position, Vector3.up, 60f);
                    s4.transform.RotateAround(s0.transform.position, Vector3.up, 60f);
                }
                if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                {
                    s1.transform.RotateAround(s0.transform.position, Vector3.up, -60f);
                    s2.transform.RotateAround(s0.transform.position, Vector3.up, -60f);
                    s3.transform.RotateAround(s0.transform.position, Vector3.up, -60f);
                    s4.transform.RotateAround(s0.transform.position, Vector3.up, -60f);
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
                        GameObject.Find("GameLogic").GetComponent<itemControl>().bombClear();

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
                    s1.transform.RotateAround(s0.transform.position, Vector3.up, 60f);                    
                    s2.transform.RotateAround(s0.transform.position, Vector3.up, 60f);                    
                }
                if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                {
                    s1.transform.RotateAround(s0.transform.position, Vector3.up, -60f);                    
                    s2.transform.RotateAround(s0.transform.position, Vector3.up, -60f);                    
                }
            }
            if (itemID==9 && !bSlimePlaced)
            {
                if (Input.GetAxis("Mouse ScrollWheel") < 0f)
                {
                    s1.transform.RotateAround(s0.transform.position, Vector3.up, 60f);
                    s2.transform.RotateAround(s0.transform.position, Vector3.up, 60f);
                    s3.transform.RotateAround(s0.transform.position, Vector3.up, 60f);
                    s4.transform.RotateAround(s0.transform.position, Vector3.up, 60f);
                }
                if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                {
                    s1.transform.RotateAround(s0.transform.position, Vector3.up, -60f);
                    s2.transform.RotateAround(s0.transform.position, Vector3.up, -60f);
                    s3.transform.RotateAround(s0.transform.position, Vector3.up, -60f);
                    s4.transform.RotateAround(s0.transform.position, Vector3.up, -60f);
                }
            }

        }
    }


    [PunRPC] public void RPC_pl3start()
    {
        GameObject.FindGameObjectWithTag("player3").GetComponent<p3control>().enabled = true;
        GameObject.FindGameObjectWithTag("player2").GetComponent<p2control>().enabled = false;
    }

}

