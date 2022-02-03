using Photon.Pun;
using UnityEngine;

public class stats : MonoBehaviour, IPunObservable
{
    public Vector3 plPos;  

    public Vector3[] path = new Vector3[6];

    public int movDist; //players mobility

    public int hitCount = 0; //needs for collision

    public int gold = 0; //players gold

    public Vector3 pasSkillHex = Vector3.down; //needs for passive skills

    public Vector3[] actSkillTrg = new Vector3[4];  //active skill target(s)  

    public int skillCD = 0;  //active skill cooldown

    public int item1 = 0; 
    public int item2 = 0; 
 
    public Vector3[] item1Targets = new Vector3[3] { Vector3.down, Vector3.down, Vector3.down };
    public Vector3[] item2Targets = new Vector3[3] { Vector3.down, Vector3.down, Vector3.down };

    public bool bMyTurn;

    
    void Awake()
    {
        pasSkillHex = Vector3.down;
        actSkillTrg = new Vector3[4] { Vector3.down, Vector3.down, Vector3.down, Vector3.down };  //can't stream GameObject!

        item1Targets = new Vector3[3] { Vector3.down, Vector3.down, Vector3.down };
        item2Targets = new Vector3[3] { Vector3.down, Vector3.down, Vector3.down };

        bMyTurn = false;
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) // send the others our data
        {
            stream.SendNext(plPos);
            stream.SendNext(path);
            stream.SendNext(movDist);
            stream.SendNext(hitCount);
            stream.SendNext(gold);
            stream.SendNext(pasSkillHex);
            stream.SendNext(actSkillTrg);  
            stream.SendNext(skillCD); 
            stream.SendNext(item1);
            stream.SendNext(item2);
            stream.SendNext(item1Targets);
            stream.SendNext(item2Targets);
            stream.SendNext(bMyTurn);
        }
        else  // receive data
        {
            this.plPos = (Vector3)stream.ReceiveNext();
            this.path = (Vector3[])stream.ReceiveNext();
            this.movDist = (int)stream.ReceiveNext();
            this.hitCount = (int)stream.ReceiveNext();
            this.gold = (int)stream.ReceiveNext();
            this.pasSkillHex = (Vector3)stream.ReceiveNext();
            this.actSkillTrg = (Vector3[])stream.ReceiveNext(); 
            this.skillCD = (int)stream.ReceiveNext();
            this.item1 = (int)stream.ReceiveNext();
            this.item2 = (int)stream.ReceiveNext();
            this.item1Targets = (Vector3[])stream.ReceiveNext();
            this.item2Targets = (Vector3[])stream.ReceiveNext();         
            this.bMyTurn = (bool)stream.ReceiveNext();
        }
    }

    private void FixedUpdate()
    {
        if (GetComponent<PhotonView>().IsMine)
        {
            plPos = transform.position;
        }
        else
        {
            transform.position = plPos;
        }
    }
}



