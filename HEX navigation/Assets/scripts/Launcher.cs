using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

namespace m4netgame2
{ 
    public class Launcher : MonoBehaviourPunCallbacks
    {       
        [SerializeField] private byte maxPlayerPerRoom = 3; 
        [SerializeField] InputField nameInput;
        const string playerNamePrefKey = "PlayerName";

        [SerializeField] private GameObject controlPanel; //title screen
        [SerializeField] private GameObject progessLabel;
        [SerializeField] GameObject slider;
        [SerializeField] GameObject p1, p2, p3;
        [SerializeField] GameObject pl1descr, pl2descr, pl3descr;
        [SerializeField] GameObject btnTest, btnPlay;

        string gameVersion = "3";
        bool isConnecting;

        ExitGames.Client.Photon.Hashtable plProp = new ExitGames.Client.Photon.Hashtable();


        //CALLBACKS
        public override void OnConnectedToMaster()
        {
            Debug.Log("PUN: connected to Master server.");
            if (isConnecting)
            {
                PhotonNetwork.JoinRandomRoom();
                isConnecting = false;
            }
        }
        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("PUN: disconnected from server. " + cause);
            isConnecting = false;

            progessLabel.SetActive(false);
            controlPanel.SetActive(true);    
        }
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("PUN: no rooms avalible. Creating new one.");
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayerPerRoom });
        }
        public override void OnJoinedRoom()
        {
            Debug.Log("PUN: client joined room. ActorNumber is " + PhotonNetwork.LocalPlayer.ActorNumber);

            // #Critical: We only load if we are the first player, else we rely on `PhotonNetwork.AutomaticallySyncScene` to sync our instance scene.
            //if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            //{
            //    Debug.Log("Loading scene. NickName: " +PhotonNetwork.NickName);
            //    PhotonNetwork.LoadLevel("s1");
            //}

            plProp["myChar"] = 0; PhotonNetwork.LocalPlayer.SetCustomProperties(plProp);  //ini
        }


        //START
        private void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;           
        }


        void Start()
        {            
            progessLabel.SetActive(false);
            controlPanel.SetActive(true);

            NameInputIni();


            //test connect
            btnTest.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (slider.GetComponent<Slider>().value == 1) {
                    PhotonNetwork.LoadLevel("s1");
                };
            });


            //multiplayer connect
            btnPlay.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (isUsed() == true)  //set gray
                {
                    if ((int)slider.GetComponent<Slider>().value == 1)
                    {
                        p1.GetComponent<BoxCollider2D>().enabled = false;
                        p1.GetComponent<SpriteRenderer>().sprite = p1.GetComponent<plChoose>().plSpr[3];
                    }
                    else if ((int)slider.GetComponent<Slider>().value == 2)
                    {
                        p2.GetComponent<BoxCollider2D>().enabled = false;
                        p2.GetComponent<SpriteRenderer>().sprite = p2.GetComponent<plChoose>().plSpr[3];
                    }
                    else if ((int)slider.GetComponent<Slider>().value == 3)
                    {
                        p3.GetComponent<BoxCollider2D>().enabled = false;
                        p3.GetComponent<SpriteRenderer>().sprite = p3.GetComponent<plChoose>().plSpr[3];
                    }

                    slider.GetComponent<Slider>().value = 0;
                }
                else  //wait game start
                {
                    plProp["myChar"] = (int)slider.GetComponent<Slider>().value; PhotonNetwork.LocalPlayer.SetCustomProperties(plProp);

                    p1.GetComponent<BoxCollider2D>().enabled = false;
                    p2.GetComponent<BoxCollider2D>().enabled = false;
                    p3.GetComponent<BoxCollider2D>().enabled = false;

                    if ((int)slider.GetComponent<Slider>().value == 1)
                    {
                        PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);  //char1 owner will host

                        InvokeRepeating("checkOthers", 3f, 2f);
                    }
                }

            });

        }

        
        void Update()
        {
            if (slider.GetComponent<Slider>().value == 1)
            {
                pl1descr.active = true;
                pl2descr.active = false;
                pl3descr.active = false;
            }
            else if (slider.GetComponent<Slider>().value == 2)
            {
                pl2descr.active = true;
                pl1descr.active = false;
                pl3descr.active = false;
            }
            else if (slider.GetComponent<Slider>().value == 3)
            {
                pl3descr.active = true;
                pl2descr.active = false;
                pl1descr.active = false;
            }
        }


        public void Connect()
        {
            progessLabel.SetActive(true);
            controlPanel.SetActive(false);

            if (PhotonNetwork.IsConnected) { PhotonNetwork.JoinRandomRoom(); }            
            else
            {
                isConnecting = PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }

            p1.GetComponent<BoxCollider2D>().enabled = true;
            p2.GetComponent<BoxCollider2D>().enabled = true;
            p3.GetComponent<BoxCollider2D>().enabled = true;
            
        }

        void NameInputIni()
        {
            string defaultName = string.Empty;
            InputField _inputField = nameInput.GetComponent<InputField>();
            if (_inputField != null)
            {
                if (PlayerPrefs.HasKey(playerNamePrefKey))
                {
                    defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                    _inputField.text = defaultName;
                }
            }
            PhotonNetwork.NickName = defaultName;
        }

        public void SetPlayerName()
        {
            string value = nameInput.GetComponent<InputField>().text;
            
            if (string.IsNullOrEmpty(value))  // #Important
            {
                Debug.LogError("PlayerName is null or empty");
                return;
            }
            PhotonNetwork.NickName = value;
            PlayerPrefs.SetString(playerNamePrefKey, value);

            if (slider.GetComponent<Slider>().value == 1)
            {
                GameObject.FindGameObjectWithTag("Statics").GetComponent<Statics>().pref1owner = PhotonNetwork.NickName;
            }
            else if (slider.GetComponent<Slider>().value == 2)
            {
                GameObject.FindGameObjectWithTag("Statics").GetComponent<Statics>().pref2owner = PhotonNetwork.NickName;
            }
            else if (slider.GetComponent<Slider>().value == 3)
            {
                GameObject.FindGameObjectWithTag("Statics").GetComponent<Statics>().pref3owner = PhotonNetwork.NickName;
            }
        }
             

        public bool isUsed()
        {
            foreach (var others in PhotonNetwork.PlayerListOthers)
            {
                if ((int)others.CustomProperties["myChar"] == (int)slider.GetComponent<Slider>().value)
                {
                    return true;
                }                
            }

            return false;
        }


        void checkOthers()
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount < 3) { return; }  //need 3 players to start

            foreach (var others in PhotonNetwork.PlayerListOthers)
            {
                if ((int)others.CustomProperties["myChar"] == 0)
                {
                    return;
                }
            }

            PhotonNetwork.LoadLevel("s1");
        }

    }


}



