using System.Collections;
using UnityEngine;

public class p2control : MonoBehaviour
{
    RaycastHit hit;
    Vector3 clHex;

    [SerializeField] GameObject pFinder;
    [SerializeField] GameObject pathPref;
    int plDist;
    GameObject[] hexes = new GameObject[20];
    public GameObject[] nearHex = new GameObject[7];
    public Vector3[] path = new Vector3[6];

    public bool bShootPrep = false;
    [SerializeField] GameObject shootProbePref;
    GameObject shootProbeB, shootProbeF;
    public GameObject[] shootHexes = new GameObject[4];
    Vector3 dir = new Vector3(-1, -1, -1);
    Ray detectRay = new Ray(Vector3.up, Vector3.down);
    RaycastHit hit2;
    int s = 0;

    Vector3 pfCor; //pathfinder Y-height correction
    int pfStepNo = 0;

    void Awake()
    {
        plDist = gameObject.GetComponent<stats>().movDist;

        hexes = GameObject.Find("GameLogic").GetComponent<goldControl>().hexes;

        clHex = pFinder.transform.position;

        pfCor = pFinder.transform.position + Vector3.down;
        
    }
   

    void OnEnable()
    {
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

        shootHexes = new GameObject[4];
    }

    void OnDisable()
    {
        if (bShootPrep && shootHexes[0]!=null) { gameObject.GetComponent<stats>().actSkillObj = shootHexes; }
        bShootPrep = false;
    }


    void Update()
    {
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
            }
        }
        if (clHex == pfCor)
        {
            CancelInvoke("pfRoutine"); pfStepNo = 0;

            for (int i = 1; i <= plDist; i++) //path highlights
            {
                Instantiate(pathPref, path[i], transform.rotation);
            }
        }

        if (bShootPrep) { pl2shootPrep(); }

    }

    void nearHexSearch() // +correction to hex pos (x,0,z)!!!
    {
        int j = 1;
        for (int i = 1; i <= hexes.Length - 1; i++)
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

        for (int i = 1; i <= nearHex.Length - 1; i++)
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
            shootHexes = new GameObject[4];

            dir = (path[1] - gameObject.transform.position).normalized;

            shootProbeB = Instantiate(shootProbePref, gameObject.transform.position+Vector3.up, Quaternion.identity);                        

        }  

        if (shootProbeB != null)
        {            
            detectRay = new Ray(shootProbeB.transform.position, Vector3.down);
            Debug.DrawRay(shootProbeB.transform.position, Vector3.down * 2, Color.black);

            shootProbeB.transform.Translate(-dir * Time.deltaTime *5);

            if (Physics.Raycast(detectRay, out hit2)){
                if(hit2.collider.gameObject.tag=="ground" && hit2.collider.gameObject.transform.position != gameObject.transform.position)
                {
                    shootHexes[0] = hit2.collider.gameObject;
                    //bShoot = false; //stops translate and undo attack prep

                    Destroy(shootProbeB);
                    shootProbeF = Instantiate(shootProbePref, gameObject.transform.position+Vector3.up, Quaternion.identity);
                    s = 0;
                }
            }

            if(shootProbeB.gameObject.transform.position.z>5 || shootProbeB.gameObject.transform.position.z<-5 
                || shootProbeB.gameObject.transform.position.x>5 || shootProbeB.gameObject.transform.position.z<-5)
            {                
                if (shootHexes[0] == null) { print("Can't shoot! No space behind."); }                

                Destroy(shootProbeB);
                shootProbeF = Instantiate(shootProbePref, gameObject.transform.position + Vector3.up, Quaternion.identity);
                s = 0;
            }
            
        }

        if (shootProbeF != null)  //on shootProbeB destoy
        {
            detectRay = new Ray(shootProbeF.transform.position, Vector3.down);
            Debug.DrawRay(shootProbeF.transform.position, Vector3.down * 2, Color.black);

            shootProbeF.transform.Translate(dir * Time.deltaTime * 5);

            if (Physics.Raycast(detectRay, out hit2))
            {
                if (hit2.collider.gameObject.tag == "ground" && hit2.collider.gameObject.transform.position != gameObject.transform.position)
                {
                    if (hit2.collider.gameObject != shootHexes[s])
                    {
                        s++;
                        shootHexes[s] = hit2.collider.gameObject;
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

}

