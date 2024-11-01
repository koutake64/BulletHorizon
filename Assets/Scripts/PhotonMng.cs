using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.Demo.Asteroids;

public class PhotonMng : MonoBehaviourPunCallbacks
{
    public static PhotonMng instance;

    public GameObject loadingPanel;

    public Text loadingText;

    public GameObject Buttons;

    public GameObject createRoomPanel;

    public Text enterRoomName;

    public GameObject roomPanel;

    public Text roomName;

    public GameObject erroePanel;

    public Text erroeText;

    public GameObject roomListPanel;

    public Room originalButton;

    public GameObject roomButtonContent;

    public GameObject configPanel;

    // ���[��������������(���[�����F���)
    Dictionary<string,RoomInfo> roomsList = new Dictionary<string, RoomInfo>();

    private List<Room> allRoomButtons = new List<Room>();

    public Text playerNameText;

    // ���O�e�L�X�g�i�[���X�g
    private List<Text> allPlayerNames = new List<Text>();

    // ���O�e�L�X�g�̐e�I�u�W�F�N�g
    public GameObject playerNameContent;

    public GameObject nameInputPanel;

    public Text placeholderText;

    public InputField nameInput;

    private bool setName;

    public GameObject startButton;

    public string levelToPlay;

    private void Awake()
    {
        instance = this;
    }
    
    private void Start()
    {
        CloseMenuUI();

        loadingPanel.SetActive(true);
        loadingText.text = "�l�b�g���[�N�ɐڑ����c";

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void CloseMenuUI()
    {
        loadingPanel.SetActive(false);

        Buttons.SetActive(false);

        createRoomPanel.SetActive(false);

        roomPanel.SetActive(false);

        erroePanel.SetActive(false);

        roomListPanel.SetActive(false);

        nameInputPanel.SetActive(false);

        configPanel.SetActive(false);
    }

    public void LobbyMenuDisplay()
    {
        CloseMenuUI();
        Buttons.SetActive(true);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();

        loadingText.text = "���r�[�ɎQ�����c";

        // Master Client�Ɠ������x�������[�h
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        LobbyMenuDisplay();

        // �����̏�����
        roomsList.Clear();

        // ���Orandom
        PhotonNetwork.NickName = Random.Range(0, 1000).ToString();

        // ���O�����͍ς݂��m�F����UI�X�V
        ConfirmationName();
    }

    // ���[���쐬�p�{�^��
    public void OpenCreateRoomPanel()
    {
        CloseMenuUI();
        createRoomPanel.SetActive(true);
    }

    public void CreateRoomButton()
    {
        if(!string.IsNullOrEmpty(enterRoomName.text)) 
        {
            RoomOptions options = new RoomOptions();
            options.MaxPlayers = 8;

            // ���[���쐬
            PhotonNetwork.CreateRoom(enterRoomName.text, options);

            CloseMenuUI();

            // ���[�h�p�l���\��
            loadingText.text = "���[���쐬��";
            loadingPanel.SetActive(true);
        }
    }

    // ���[���ɎQ�����ɌĂ΂��֐�
    public override void OnJoinedRoom()
    {
        CloseMenuUI();
        roomPanel.SetActive(true );

        // ���[���̖��O�𔽉f
        roomName.text = PhotonNetwork.CurrentRoom.Name;

        // ���[���ɂ���v���C���[�����擾
        GetAllPlayer();

        // �}�X�^�[�����肵�{�^���\��
        CheckRoomMaster();
    }

    // ���[���ޏo
    public void LeaveRoom()
    {
        // ���[������ޏo
        PhotonNetwork.LeaveRoom();

        // UI
        CloseMenuUI();
        loadingText.text = "�ޏo��";
        loadingPanel.SetActive(true);
    }

    // ���[���ޏo���ɌĂяo�����֐�
    public override void OnLeftRoom()
    {
        LobbyMenuDisplay();
    }

    // ���[���쐬�ł��Ȃ��������ɌĂ΂��֐�
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        CloseMenuUI();
        erroeText.text = "���[���쐬�̍쐬�Ɏ��s���܂����B" + message;

        erroePanel.SetActive(true);
    }

    // ���[���ꗗ�p�l�����J���֐�
    public void FindRoom()
    {
        CloseMenuUI();
        roomListPanel.SetActive(true);
    }

    //�@���[�����X�g�ɍX�V�����������ɌĂ΂��֐�
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // ���[���{�^��UI������
        RoomUIinitialization();

        // �����ɓo�^
        UpdateRoomList(roomList);
    }

    // ���[�����������ɓo�^
    public void UpdateRoomList(List<RoomInfo> roomList)
    {
        // �����Ƀ��[���o�^
        for (int i = 0; i < roomList.Count; i++)
        {
            RoomInfo info = roomList[i];

            if (info.RemovedFromList)
            {
                roomsList.Remove(info.Name);
            }
            else
            {
                roomsList[info.Name] = info;
            }
        }
        // ���[���{�^���\���֐�
        RoomListDisplay(roomsList);
    }

    public void RoomListDisplay(Dictionary<string,RoomInfo> cachedRoomList)
    {
        foreach(var roomInfo in cachedRoomList)
        {
            // �{�^���쐬
            Room newButton = Instantiate(originalButton);

            // ���������{�^���Ƀ��[�����ݒ�
            newButton.RegisterRoomDetails(roomInfo.Value);

            // �e�̐ݒ�
            newButton.transform.SetParent(roomButtonContent.transform);

            allRoomButtons.Add(newButton);
        }
    }

    public void RoomUIinitialization()
    {
        foreach (Room rm in allRoomButtons)
        {
            // �폜
            Destroy(rm.gameObject);
        }

        // ���X�g�̏�����
        allRoomButtons.Clear();
    }

    // ���[���Q��
    public void JoinRoom(RoomInfo roomInfo)
    {
        PhotonNetwork.JoinRoom(roomInfo.Name);

        // UI
        CloseMenuUI();

        loadingText.text = "���[���Q����";
        loadingPanel.SetActive(true);
    }

    // ���[���ɂ���v���C���[�����擾
    public void GetAllPlayer()
    {
        // ���O�e�L�X�gUI������
        InitializePlayerList();

        // �v���C���[�\���֐�
        PlayerDisplay();
    }

    public void InitializePlayerList()
    {
        foreach(var rm in allPlayerNames)
        {
            Destroy(rm.gameObject);
        }

        allPlayerNames.Clear();
    }

    public void PlayerDisplay()
    {
        // ���[���ɎQ�����Ă���l����UI�쐬
        foreach(var players in PhotonNetwork.PlayerList) 
        {
            // UI�����֐�
            PlayerTextGeneration(players);
        }
    }

    public void PlayerTextGeneration(Player players)
    {
        // UI����
        Text newPlayerText = Instantiate(playerNameText);

        // �e�L�X�g�ɖ��O�𔽉f
        newPlayerText.text = players.NickName;

        // �e�I�u�W�F�N�g�̐ݒ�
        newPlayerText.transform.SetParent(playerNameContent.transform);

        // ���X�g�ɓo�^
        allPlayerNames.Add(newPlayerText);
    }

    public void ConfirmationName()
    {
        if (!setName)
        {
            CloseMenuUI();
            nameInputPanel.SetActive(true);

            if(PlayerPrefs.HasKey("playerName"))
            {
                placeholderText.text = PlayerPrefs.GetString("playerName");
                nameInput.text = PlayerPrefs.GetString("playerName");
            }
        }
        else
        {
            PhotonNetwork.NickName = PlayerPrefs.GetString("playerName");
        }
    }

    // ���O�o�^�֐�
    public void SetName()
    {
        // ���̓t�B�[���h�ɕ��������͂���Ă��邩�ǂ���
        if(!string.IsNullOrEmpty(nameInput.text))
        {
            // ���[�U�[���o�^
            PhotonNetwork.NickName = nameInput.text;
            
            // �ۑ�
            PlayerPrefs.SetString("playerName", nameInput.text);

            // UI
            LobbyMenuDisplay();

            setName = true;
        }
    }

    // �v���C���[�����[���ɓ��������ɌĂяo�����֐�
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
       PlayerTextGeneration(newPlayer);
    }

    // �v���C���[�����[���𗣂�邩�A��A�N�e�B�u�ɂȂ������ɌĂяo�����֐�
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        GetAllPlayer();
    }

    public void CheckRoomMaster()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(true);
        }
        else
        {
            startButton.SetActive(false);
        }
    }

    // �}�X�^�[���؂�ւ�������ɌĂяo�����֐�
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(true);
        }
    }

    // �J�ڈړ�
    public void PlayGame()
    {
        PhotonNetwork.LoadLevel(levelToPlay);
    }

    public void OpenConfigPanel()
    {
        CloseMenuUI();
        configPanel.SetActive(true);
    }

    public void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//�Q�[���v���C�I��
#else
    Application.Quit();//�Q�[���v���C�I��
#endif
    }
}