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

        [SerializeField] private GameObject controlPanel;
        [SerializeField] private GameObject progessLabel;
        [SerializeField] GameObject slider;
        [SerializeField] GameObject pl1descr;
        [SerializeField] GameObject pl2descr;
        [SerializeField] GameObject pl3descr;
        [SerializeField] GameObject btn1, btn2, btn3; //new buttons

        string gameVersion = "1";
        bool isConnecting;
                

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
            Debug.Log("PUN: client joined room.");

            // #Critical: We only load if we are the first player, else we rely on `PhotonNetwork.AutomaticallySyncScene` to sync our instance scene.
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                Debug.Log("Loading scene. NickName: " +PhotonNetwork.NickName);

                PhotonNetwork.LoadLevel("s1");
            }
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

            btn1.GetComponent<Button>().onClick.AddListener( () => {
                slider.GetComponent<Slider>().value = 1;
            } );
            btn2.GetComponent<Button>().onClick.AddListener(() => {
                slider.GetComponent<Slider>().value = 2;
            });
            btn3.GetComponent<Button>().onClick.AddListener(() => {
                slider.GetComponent<Slider>().value = 3;
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
             

    }


}



