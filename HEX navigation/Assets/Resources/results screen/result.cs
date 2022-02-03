using UnityEngine;

public class result : MonoBehaviour
{   
    int p1Gold, p2Gold, p3Gold;

    [SerializeField]GameObject[] frame;
    [SerializeField]GameObject[] firstPlace;
    [SerializeField]GameObject[] secondPlace;
    [SerializeField]GameObject[] thirdPlace;
    [SerializeField]GameObject draw;

    private void Start()
    {
        p1Gold = map.mapS.pl1.GetComponent<stats>().gold;
        p2Gold = map.mapS.pl2.GetComponent<stats>().gold;
        p3Gold = map.mapS.pl3.GetComponent<stats>().gold;

        Result();
    }
    

    void Result()
    {
        FirstPlace();

    }

    private void FirstPlace()
    {
        Debug.Log(p1Gold+p2Gold+p3Gold);
        /*PLayer 1 wins*/
        if (p1Gold > p2Gold && p1Gold > p3Gold)
        {
            frame[0].SetActive(true);
            firstPlace[0].SetActive(true);
               /*Player 2 second*/
            if (p2Gold > p3Gold)
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
        else if (p2Gold > p1Gold && p2Gold > p1Gold) //
        {
            frame[0].SetActive(true);
            firstPlace[1].SetActive(true);
            if (p3Gold > p1Gold)
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
        else if (p3Gold > p1Gold && p3Gold > p2Gold)  
        {
            frame[0].SetActive(true);
            firstPlace[2].SetActive(true);
            if (p2Gold > p1Gold)  
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

        /*Player1 & Player2 draw*/ /*not working prperly*/
        if (p1Gold == p2Gold && p2Gold > p3Gold)
        {
            frame[1].SetActive(true);
            firstPlace[3].SetActive(true);
            secondPlace[3].SetActive(true);
            thirdPlace[5].SetActive(true);
        }
        /*Player1 & Player3 draw*/
        else if (p1Gold == p3Gold && p3Gold > p2Gold)
        {
            frame[1].SetActive(true);
            firstPlace[3].SetActive(true);
            firstPlace[5].SetActive(true);
            thirdPlace[4].SetActive(true);
        }
        /*Player2 & Player3 draw*/
        else if (p2Gold == p3Gold && p3Gold > p1Gold)
        {
            frame[1].SetActive(true);
            firstPlace[4].SetActive(true);
            firstPlace[5].SetActive(true);
            thirdPlace[3].SetActive(true);
        }

        /*All PLayers Draw*/
        else if (p1Gold==p2Gold&&p2Gold==p3Gold)
        {
            frame[2].SetActive(true);
            draw.SetActive(true);
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
