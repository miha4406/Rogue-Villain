using System.Collections;
using UnityEngine;

public class turnEnd : MonoBehaviour
{    
    [SerializeField] float plMovSp = 1f;
    [SerializeField] GameObject pl1;
    [SerializeField] GameObject pl2;
    [SerializeField] GameObject pl3;
    int pl1dist; 
    int pl2dist;
    int pl3dist; 

    public Vector3[] p1Path = new Vector3[6];    
    int mov1StepNo = 1;
    public int lastMov1 = 0;
    bool bMove1 = false;
        
    public Vector3[] p2Path = new Vector3[6];
    int mov2StepNo = 1;
    public int lastMov2 = 0;
    bool bMove2 = false;
    public bool pl2atk1 = false;   

    public Vector3[] p3Path = new Vector3[6];
    int mov3StepNo = 1;
    public int lastMov3 = 0;
    bool bMove3 = false;

    bool bWait = false;     //all controls blocked
    bool movSw = false;   
    public bool bMoveEnd = false;  //all movement ended
    public int turnNo = 1;

    void Awake()
    {
        pl1.GetComponent<stats>().movDist = 2;
        pl2.GetComponent<stats>().movDist = 1;
        pl3.GetComponent<stats>().movDist = 1;

        pl1dist = pl1.GetComponent<stats>().movDist;
        pl2dist = pl2.GetComponent<stats>().movDist;
        pl3dist = pl3.GetComponent<stats>().movDist;
    }



    void Update()
    {
        if (bWait) { print("WAIT"); }       

        if (!bWait)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                pl1.GetComponent<plControl>().enabled = true;
                pl2.GetComponent<p2control>().enabled = false;
                pl3.GetComponent<p3control>().enabled = false;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                pl1.GetComponent<plControl>().enabled = false;
                pl2.GetComponent<p2control>().enabled = true;
                pl3.GetComponent<p3control>().enabled = false;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                pl1.GetComponent<plControl>().enabled = false;
                pl2.GetComponent<p2control>().enabled = false;
                pl3.GetComponent<p3control>().enabled = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space)) {

            p1Path = pl1.GetComponent<plControl>().path;
            p1Path[0] = pl1.transform.position; //can return to start pos
            bMove1 = true;

            p2Path = pl2.GetComponent<p2control>().path;
            p2Path[0] = pl2.transform.position;
            bMove2 = true;

            p3Path = pl3.GetComponent<p3control>().path;
            p3Path[0] = pl3.transform.position;
            bMove3 = true;

            pl1.GetComponent<plControl>().enabled = false;
            pl2.GetComponent<p2control>().enabled = false;
            pl3.GetComponent<p3control>().enabled = false;

            movSw = true;            
        }        

        if (bMove1) { bWait = true; pl1move(mov1StepNo); }
        if (bMove2) { bWait = true; pl2move(mov2StepNo); }
        if (bMove3) { bWait = true; pl3move(mov3StepNo); }

        
     /////////////////////collisions//////////////////////////
        if (!bMove1 && !bMove2 && !bMove3) { 

            if (pl1.transform.position == pl2.transform.position) { StartCoroutine(plCollision(pl1, pl2, pl3)); }
            if (pl1.transform.position == pl3.transform.position) { StartCoroutine(plCollision(pl1, pl3, pl2)); }
            if (pl2.transform.position == pl3.transform.position) { StartCoroutine(plCollision(pl2, pl3, pl1)); }
            else { movEndCheck(); }
        }
        if (pl1.GetComponent<stats>().hitCount!=0)
        {
           // print("pl1 hits"); 
            if (lastMov1-pl1.GetComponent<stats>().hitCount >= 0) { pl1.transform.position = Vector3.MoveTowards(pl1.transform.position, p1Path[lastMov1-pl1.GetComponent<stats>().hitCount], plMovSp * 1.05f * Time.deltaTime); }       
        }
        if (pl2.GetComponent<stats>().hitCount!=0)
        {
           // print("pl2 hits"); 
            if (lastMov2-pl2.GetComponent<stats>().hitCount >= 0) { pl2.transform.position = Vector3.MoveTowards(pl2.transform.position, p2Path[lastMov2-pl2.GetComponent<stats>().hitCount], plMovSp * Time.deltaTime); }
        }
        if (pl3.GetComponent<stats>().hitCount != 0)
        {
          // print("pl3 hits"); 
            if (lastMov3-pl3.GetComponent<stats>().hitCount >= 0) { pl3.transform.position = Vector3.MoveTowards(pl3.transform.position, p3Path[lastMov3-pl3.GetComponent<stats>().hitCount], plMovSp * 0.95f * Time.deltaTime); }
        }
     //////////////////////////////////////////////////////////        
    
    }

    IEnumerator plCollision(GameObject plA, GameObject plB, GameObject plC)
    {
        movSw = false;

        yield return new WaitForSeconds(0.2f);

        if (plA.transform.position == plB.transform.position)
        {            
            plA.GetComponent<stats>().hitCount++; 
            plB.GetComponent<stats>().hitCount++;            
            if (plB.transform.position == plC.transform.position)
            {
                plC.GetComponent<stats>().hitCount++;
            }

            bWait = true;

            StopAllCoroutines();       
            //StopCoroutine(hitCountClear(plA, plB, plC));
            StartCoroutine( hitCountClear(plA, plB, plC) );

            gameObject.GetComponent<goldControl>().goldSwap(plA, plB, plC);
        }
      
    }  
    IEnumerator hitCountClear(GameObject plA, GameObject plB, GameObject plC)
    {        
        yield return new WaitForSeconds(plMovSp * 1.5f); //malo?
        plA.GetComponent<stats>().hitCount = 0;
        plB.GetComponent<stats>().hitCount = 0;
        plC.GetComponent<stats>().hitCount = 0;
        
        bWait = false;

        turnNo++;
        gameObject.GetComponent<goldControl>().movEnd = true;
    }
 

    void pl1move(int stepNo) 
    {
        //if (pl1.GetComponent<stats>().actSkillObj[0] != null){   //if p1 active skill is used
        //    for (int i = 1; i <= 5; i++) { p1Path[i] = Vector3.down; }
        //}

        if (p1Path[stepNo] != Vector3.down && (stepNo <= pl1dist))
        {
            pl1.transform.position = Vector3.MoveTowards(pl1.transform.position, p1Path[stepNo], plMovSp*1.05f *Time.deltaTime);

            pl1.transform.LookAt(p1Path[stepNo]);

            if (pl1.transform.position == p1Path[stepNo])
            {
                mov1StepNo++;
            }
        }
        else { bMove1 = false; lastMov1 = mov1StepNo-1;  mov1StepNo = 1; bWait = false; }       
    }

    void pl2move(int stepNo)
    {
        if (pl2atk1 || pl2.GetComponent<stats>().actSkillObj[0]!=null) {   //if p2 attack or shoot is used
            pl2.GetComponent<stats>().pasSkillHex = p2Path[1]; 
            for (int i=1; i<=5; i++) { p2Path[i] = Vector3.down; }
        } 

        if (p2Path[stepNo] != Vector3.down && (stepNo <= pl2dist))
        {
            pl2.transform.position = Vector3.MoveTowards(pl2.transform.position, p2Path[stepNo], plMovSp*1.00f *Time.deltaTime);

            pl2.transform.LookAt(p2Path[stepNo]);

            if (pl2.transform.position == p2Path[stepNo])
            {
                mov2StepNo++;
            }
        }
        else { bMove2 = false; lastMov2 = mov2StepNo-1; mov2StepNo = 1; bWait = false; }
    }
    void pl3move(int stepNo)
    {
        if ( pl3.GetComponent<stats>().actSkillObj[0] != null)  {  //if p3 active skill is used
            for (int i=1; i<=5; i++) { p3Path[i] = Vector3.down; }
        }

        if (p3Path[stepNo] != Vector3.down && (stepNo <= pl3dist))
        {
            pl3.transform.position = Vector3.MoveTowards(pl3.transform.position, p3Path[stepNo], plMovSp * Time.deltaTime);

            pl3.transform.LookAt(p3Path[stepNo]);

            if (pl3.transform.position == p3Path[stepNo])
            {
                mov3StepNo++;
            }
        }
        else { bMove3 = false; lastMov3 = mov3StepNo-1; mov3StepNo = 1; bWait = false; } 
    }

    void movEndCheck()
    {
        if (movSw) { 
            turnNo++;
            gameObject.GetComponent<goldControl>().movEnd=true; 
            movSw = false; 
        }
          
    }




}
