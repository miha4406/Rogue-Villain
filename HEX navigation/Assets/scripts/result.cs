using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class result : MonoBehaviour
{
    /*plControl*/
    int p1controller;
    /*p2control*/
    int p2controller;
    /*p3control*/
    int p3controller;
  


    [SerializeField]GameObject[] firstPlace;
    [SerializeField]GameObject[] secondPlace;
    [SerializeField]GameObject[] thirdPlace;

    private void Start()
    {
        
    }

    private void Awake()
    {
        p1controller = map.mapS.pl1.GetComponent<stats>().gold;
        p2controller = map.mapS.pl2.GetComponent<stats>().gold;
        p3controller = map.mapS.pl3.GetComponent<stats>().gold;
        Result();
    }


    void Result()
    {
        FirstPlace();

    }

    private void FirstPlace()
    {
        Debug.Log(p1controller+p2controller+p3controller);
        if (p1controller > p2controller && p1controller > p3controller)
        {
            firstPlace[0].SetActive(true);
            if (p2controller > p3controller)
            {
                secondPlace[1].SetActive(true);
                thirdPlace[2].SetActive(true);
            }
            else
            {
                secondPlace[2].SetActive(true);
                thirdPlace[1].SetActive(true);
            }

        }
        else if (p2controller > p3controller)
        {
            firstPlace[1].SetActive(true);
            if (p3controller > p1controller)
            {
                secondPlace[2].SetActive(true);
                thirdPlace[0].SetActive(true);
            }
            else
            {
                secondPlace[0].SetActive(true);
                thirdPlace[2].SetActive(true);
            }
        }
        else if (p3controller > p1controller && p3controller > p2controller)
        {
            firstPlace[2].SetActive(true);
            if (p2controller > p1controller)
            {
                secondPlace[1].SetActive(true);
                thirdPlace[0].SetActive(true);
            }
            else
            {
                secondPlace[0].SetActive(true);
                thirdPlace[1].SetActive(true);
            }
        }
    }

   /* public void SecondPlace()
    {
        if (p1controller > p2controller && p1controller > p3controller)
        {
            //GameObject.Find("ScreenCanvas/resultPanel/secondPlace/pl1").GetComponent<Image>().sprite = secondPlace[1];
            secondPlace[0].SetActive(true);
            secondPlace[0].SetActive(false);
            
        }
        else if (p2controller > p1controller && p2controller > p3controller)
        {
           // GameObject.Find("ScreenCanvas/resultPanel/secondPlace/pl2").GetComponent<Image>().sprite = secondPlace[2];
           secondPlace[1].SetActive(true);
        }
        else if (p3controller > p1controller && p3controller > p2controller)
        {
            //GameObject.Find("ScreenCanvas/resultPanel/secondPlace/pl3").GetComponent<Image>().sprite = secondPlace[3];
            secondPlace[2].SetActive(true);
        }
    }

    public void ThirdPlace()
    {
        if (p1controller > p2controller && p1controller > p3controller)
        {
            //GameObject.Find("ScreenCanvas/resultPanel/thirdPlace/pl1").GetComponent<Image>().sprite = thirdPlace[1];
            thirdPlace[0].SetActive(true);
        }
        else if (p2controller > p1controller && p2controller > p3controller)
        {
            //GameObject.Find("ScreenCanvas/resultPanel/thirdPlace/pl2").GetComponent<Image>().sprite = thirdPlace[2];
            thirdPlace[1].SetActive(true);
        }
        else if (p3controller > p1controller && p3controller > p2controller)
        {
            //GameObject.Find("ScreenCanvas/resultPanel/thirdPlace/pl3").GetComponent<Image>().sprite = thirdPlace[3];
            thirdPlace[2].SetActive(true);
        }
    }
*/
}
