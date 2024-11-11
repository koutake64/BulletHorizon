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

    // ルーム情報を扱う辞書(ルーム名：情報)
    Dictionary<string,RoomInfo> roomsList = new Dictionary<string, RoomInfo>();

    private List<Room> allRoomButtons = new List<Room>();

    public Text playerNameText;

    // 名前テキスト格納リスト
    private List<Text> allPlayerNames = new List<Text>();

    // 名前テキストの親オブジェクト
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
        loadingText.text = "ネットワークに接続中…";

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

        loadingText.text = "ロビーに参加中…";

        // Master Clientと同じレベルをロード
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        LobbyMenuDisplay();

        // 辞書の初期化
        roomsList.Clear();

        // 名前random
        PhotonNetwork.NickName = Random.Range(0, 1000).ToString();

        // 名前が入力済みか確認してUI更新
        ConfirmationName();
    }

    // ルーム作成用ボタン
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

            // ルーム作成
            PhotonNetwork.CreateRoom(enterRoomName.text, options);

            CloseMenuUI();

            // ロードパネル表示
            loadingText.text = "ルーム作成中";
            loadingPanel.SetActive(true);
        }
    }

    // ルームに参加時に呼ばれる関数
    public override void OnJoinedRoom()
    {
        CloseMenuUI();
        roomPanel.SetActive(true );

        // ルームの名前を反映
        roomName.text = PhotonNetwork.CurrentRoom.Name;

        // ルームにいるプレイヤー情報を取得
        GetAllPlayer();

        // マスターか判定しボタン表示
        CheckRoomMaster();
    }

    // ルーム退出
    public void LeaveRoom()
    {
        // ルームから退出
        PhotonNetwork.LeaveRoom();

        // UI
        CloseMenuUI();
        loadingText.text = "退出中";
        loadingPanel.SetActive(true);
    }

    // ルーム退出時に呼び出される関数
    public override void OnLeftRoom()
    {
        LobbyMenuDisplay();
    }

    // ルーム作成できなかった時に呼ばれる関数
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        CloseMenuUI();
        erroeText.text = "ルーム作成の作成に失敗しました。" + message;

        erroePanel.SetActive(true);
    }

    // ルーム一覧パネルを開く関数
    public void FindRoom()
    {
        CloseMenuUI();
        roomListPanel.SetActive(true);
    }

    //　ルームリストに更新があった時に呼ばれる関数
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // ルームボタンUI初期化
        RoomUIinitialization();

        // 辞書に登録
        UpdateRoomList(roomList);
    }

    // ルーム情報を辞書に登録
    public void UpdateRoomList(List<RoomInfo> roomList)
    {
        // 辞書にルーム登録
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
        // ルームボタン表示関数
        RoomListDisplay(roomsList);
    }

    public void RoomListDisplay(Dictionary<string,RoomInfo> cachedRoomList)
    {
        foreach(var roomInfo in cachedRoomList)
        {
            // ボタン作成
            Room newButton = Instantiate(originalButton);

            // 生成したボタンにルーム情報設定
            newButton.RegisterRoomDetails(roomInfo.Value);

            // 親の設定
            newButton.transform.SetParent(roomButtonContent.transform);

            allRoomButtons.Add(newButton);
        }
    }

    public void RoomUIinitialization()
    {
        foreach (Room rm in allRoomButtons)
        {
            // 削除
            Destroy(rm.gameObject);
        }

        // リストの初期化
        allRoomButtons.Clear();
    }

    // ルーム参加
    public void JoinRoom(RoomInfo roomInfo)
    {
        PhotonNetwork.JoinRoom(roomInfo.Name);

        // UI
        CloseMenuUI();

        loadingText.text = "ルーム参加中";
        loadingPanel.SetActive(true);
    }

    // ルームにいるプレイヤー情報を取得
    public void GetAllPlayer()
    {
        // 名前テキストUI初期化
        InitializePlayerList();

        // プレイヤー表示関数
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
        // ルームに参加している人数分UI作成
        foreach(var players in PhotonNetwork.PlayerList) 
        {
            // UI生成関数
            PlayerTextGeneration(players);
        }
    }

    public void PlayerTextGeneration(Player players)
    {
        // UI生成
        Text newPlayerText = Instantiate(playerNameText);

        // テキストに名前を反映
        newPlayerText.text = players.NickName;

        // 親オブジェクトの設定
        newPlayerText.transform.SetParent(playerNameContent.transform);

        // リストに登録
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

    // 名前登録関数
    public void SetName()
    {
        // 入力フィールドに文字が入力されているかどうか
        if(!string.IsNullOrEmpty(nameInput.text))
        {
            // ユーザー名登録
            PhotonNetwork.NickName = nameInput.text;
            
            // 保存
            PlayerPrefs.SetString("playerName", nameInput.text);

            // UI
            LobbyMenuDisplay();

            setName = true;
        }
    }

    // プレイヤーがルームに入った時に呼び出される関数
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
       PlayerTextGeneration(newPlayer);
    }

    // プレイヤーがルームを離れるか、非アクティブになった時に呼び出される関数
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

    // マスターが切り替わった時に呼び出される関数
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(true);
        }
    }

    // 遷移移動
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
        UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif
    }
}