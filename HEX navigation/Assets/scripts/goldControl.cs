﻿using System.Collections;
using UnityEngine;
using System.Linq;

public class goldControl : MonoBehaviour
{
    [SerializeField] GameObject gBar1;
    [SerializeField] GameObject gBar2;
    [SerializeField] GameObject gBar3;

    [SerializeField] GameObject pl1;
    [SerializeField] GameObject pl2;
    [SerializeField] GameObject pl3;

    bool b1skill = false;

    public GameObject[] hexes = new GameObject[20];
    public GameObject[] itemHexes = new GameObject[3]; // item hexes from itemControl

    public bool movEnd;  //sets "true" from turnEnd.cs
    int turnNo;

    void Awake()
    {
        gBar1.transform.position = hexes[10].transform.position;       
    }  

    
    void Update()
    {
        turnNo = gameObject.GetComponent<turnEnd>().turnNo;
        print("Turn "+turnNo);

        itemHexes = gameObject.GetComponent<itemControl>().itemHexes;

        if (turnNo>0) { gHex1(); }
        if (turnNo>5) { gHex2(); }
        if (turnNo>10){ gHex3(); }

        p1Challenge();

        p2Attack();
        p2Shoot();

        p3GoldGet();


        movEnd = false;
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

        if (movEnd && gBar1.active) //gBar1 pick up (2turns for pl1, 2bars for pl3)
        {
            if (gBar1.transform.position == pl1.transform.position) {
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

            if (gBar1.transform.position == pl2.transform.position) { pl2.GetComponent<stats>().gold++; gBar1.SetActive(false); }
            if (gBar1.transform.position == pl3.transform.position) { pl3.GetComponent<stats>().gold+=2; gBar1.SetActive(false); }           
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
        
        if (movEnd && gBar2.active) //gBar1 pick up (2turns for pl1, 2bars for pl3)
        {
            if (gBar2.transform.position == pl1.transform.position) {
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

            if (gBar2.transform.position == pl2.transform.position) { pl2.GetComponent<stats>().gold++; gBar2.SetActive(false); }
            if (gBar2.transform.position == pl3.transform.position) { pl3.GetComponent<stats>().gold+=2; gBar2.SetActive(false); }            
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

        if (movEnd && gBar3.active) //gBar1 pick up (2turns for pl1, 2bars for pl3)
        {
            if (gBar3.transform.position == pl1.transform.position) {
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

            if (gBar3.transform.position == pl2.transform.position) { pl2.GetComponent<stats>().gold++; gBar3.SetActive(false); }
            if (gBar3.transform.position == pl3.transform.position) { pl3.GetComponent<stats>().gold+=2; gBar3.SetActive(false); }
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
            if ( b1skill && (plA==pl1 && plB==pl1.GetComponent<stats>().actSkillObj[0]) )
            {
                print("pl1 challenge succeed!"); b1skill = false;  //only once!

                if (plB.GetComponent<stats>().gold >= 3) { plB.GetComponent<stats>().gold -=3; plA.GetComponent<stats>().gold +=3; }
                if (plB.GetComponent<stats>().gold == 2) { plB.GetComponent<stats>().gold -=2; plA.GetComponent<stats>().gold +=2; }
                if (plB.GetComponent<stats>().gold == 1) { plB.GetComponent<stats>().gold -=1; plA.GetComponent<stats>().gold +=1; }
            }
            else if ( b1skill && (plB==pl1 && plA==pl1.GetComponent<stats>().actSkillObj[0]) )
            {
                print("pl1 challenge succeed!"); b1skill = false;  //only once!

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

    public void hexInfo(Vector3 clHex)
    {
        if (clHex == gBar1?.transform.position || clHex == gBar2?.transform.position || clHex == gBar3?.transform.position)
        {
            print("GOLD HEX");
        }
        if(clHex == pl2.transform.position)
        {
            print("player2 HEX");
        }
        if (clHex == pl3.transform.position)
        {
            print("player3 HEX");
        }
        if (clHex == hexes[1].transform.position || clHex == hexes[12].transform.position || clHex == hexes[17].transform.position)
        {
            print("ITEM HEX");
        }
        else { print("PLAIN HEX"); }
    }

    ////////////////////////// SKILLS /////////////////////////

    public void p1Challenge()
    {
        if (movEnd && pl1.GetComponent<stats>().skillCD !=0 )
        {
            if (pl1.GetComponent<stats>().skillCD == 4) { b1skill = true; }
            if (pl1.GetComponent<stats>().skillCD == 3) { 
                b1skill = false; pl1.GetComponent<stats>().actSkillObj[0] = null;
            }

            pl1.GetComponent<stats>().skillCD--;
        }
    }

    public void p2Attack()
    {
        if (movEnd && gameObject.GetComponent<turnEnd>().pl2atk1)
        {
            if (pl2.GetComponent<stats>().pasSkillHex == pl1.transform.position) //attack on pl1
            {
                print("Attack on pl1!");
                if (pl1.GetComponent<stats>().gold >= 2) { pl1.GetComponent<stats>().gold -= 2; pl2.GetComponent<stats>().gold += 2; }
                else if (pl1.GetComponent<stats>().gold == 1) { pl1.GetComponent<stats>().gold -= 1; pl2.GetComponent<stats>().gold += 1; }
            }
            if (pl2.GetComponent<stats>().pasSkillHex == pl3.transform.position) //attack on pl3
            {
                print("Attack on pl3!");
                if (pl3.GetComponent<stats>().gold >= 2) { pl3.GetComponent<stats>().gold -= 2; pl2.GetComponent<stats>().gold += 2; }
                else if (pl3.GetComponent<stats>().gold == 1) { pl3.GetComponent<stats>().gold -= 1; pl2.GetComponent<stats>().gold += 1; }
            }

            gameObject.GetComponent<turnEnd>().pl2atk1 = false;
        }
    }
    public void p2Shoot() //after shoot prep
    {
        if (movEnd && pl2.GetComponent<stats>().actSkillObj[0] != null)
        {
            if (pl2.GetComponent<stats>().actSkillObj[0].transform.position == pl1.transform.position 
                || pl2.GetComponent<stats>().actSkillObj[0].transform.position == pl3.transform.position) {
                print("Can't shoot! Obstacle behind.");
            }
            else
            {
                for(int j=1; j<=3; j++)
                {
                    if (pl2.GetComponent<stats>().actSkillObj[j] != null)
                    {
                        if (pl2.GetComponent<stats>().actSkillObj[j].transform.position == pl1.transform.position)
                        {
                            print("Shoot in pl1!");
                            if (pl1.GetComponent<stats>().gold >= 2) { pl1.GetComponent<stats>().gold -= 2; pl2.GetComponent<stats>().gold += 2; }
                            else if (pl1.GetComponent<stats>().gold == 1) { pl1.GetComponent<stats>().gold -= 1; pl2.GetComponent<stats>().gold += 1; }
                        }
                        if (pl2.GetComponent<stats>().actSkillObj[j].transform.position == pl3.transform.position)
                        {
                            print("Shoot in pl3!");
                            if (pl3.GetComponent<stats>().gold >= 2) { pl3.GetComponent<stats>().gold -= 2; pl2.GetComponent<stats>().gold += 2; }
                            else if (pl3.GetComponent<stats>().gold == 1) { pl3.GetComponent<stats>().gold -= 1; pl2.GetComponent<stats>().gold += 1; }
                        }
                    }
                }
                pl2.transform.position = pl2.GetComponent<stats>().actSkillObj[0].transform.position;
            }
            
            pl2.GetComponent<stats>().actSkillObj = new GameObject[4];
        }
        if (movEnd && pl2.GetComponent<stats>().skillCD!=0) { pl2.GetComponent<stats>().skillCD--; }
    }

    public void p3GoldGet()
    {
        if (pl3.GetComponent<stats>().actSkillObj[0] != null)
        {
            print("pl3 gets gold!");

            pl3.GetComponent<stats>().gold++;

            pl3.GetComponent<stats>().actSkillObj[0] = null;
        }

        if (movEnd && pl3.GetComponent<stats>().skillCD!=0) { pl3.GetComponent<stats>().skillCD--; }
        
    }
}
