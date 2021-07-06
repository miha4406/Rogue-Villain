﻿using UnityEngine.UI;
using UnityEngine;

public class p3control : MonoBehaviour
{
    RaycastHit hit;
    Vector3 clHex;

    [SerializeField] GameObject pFinder;
    [SerializeField] GameObject pathPref;
    GameObject[] destroy = new GameObject[5];
    int plDist;
    GameObject[] hexes = new GameObject[20];
    public GameObject[] nearHex = new GameObject[7];
    public Vector3[] path = new Vector3[6];

    public bool bGoldGet = false;

    Vector3 pfCor; //pathfinder Y-height correction
    int pfStepNo = 0;

    [SerializeField] Sprite p3logo;
    [SerializeField] Sprite p3a;

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
                if (Vector3.Distance(x.transform.position, transform.position) < plDist + 0.5f)
                {
                    x.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
                    x.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.green);
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

        GameObject.Find("ScreenCanvas/logoImage").GetComponent<Image>().sprite = p3logo;
        GameObject.Find("ScreenCanvas/butAct").GetComponent<Image>().sprite = p3a;
        GameObject.Find("ScreenCanvas/butPas").GetComponent<Button>().interactable = false;
        GameObject.Find("ScreenCanvas/butAct").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("ScreenCanvas/butAct").GetComponent<Button>().onClick.AddListener( () => { bGoldGet = !bGoldGet; } );
        GameObject.Find("ScreenCanvas/butAct").GetComponent<Button>().interactable = true;
        if (gameObject.GetComponent<stats>().skillCD != 0)
        {
            GameObject.Find("ScreenCanvas/butAct").GetComponent<Button>().interactable = false;
            GameObject.Find("ScreenCanvas/butAct").GetComponentInChildren<Text>().text = gameObject.GetComponent<stats>().skillCD.ToString();
        }
        else { GameObject.Find("ScreenCanvas/butAct").GetComponentInChildren<Text>().text = ""; }

    }

    void OnDisable()
    {
        if (bGoldGet) { 
            gameObject.GetComponent<stats>().actSkillObj[0] = gameObject;
            gameObject.GetComponent<stats>().skillCD = 3;
        }
        bGoldGet = false;


    }


    void Update()
    {
        pfCor = pFinder.transform.position + Vector3.down;

        Ray mRay = Camera.main.ScreenPointToRay(Input.mousePosition);   //click detect
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


}

