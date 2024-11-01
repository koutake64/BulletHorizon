using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;


public class GameManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    // プレイヤー情報を格納するリストの作成
    public List<PlayerInfo> playerList = new List<PlayerInfo>();

    // イベント作成
    public enum EventCodes : byte
    {
        NewPlayer,
        ListPlayers,
        UpdateStat
    }

    // ゲーム状態作成
    public enum GameState
    {
        Playing,
        Ending
    }

    // ゲーム状態格納
    public GameState state;

    UIManager uiManager;

    private List<PlayerInformation> playerInfoList = new List<PlayerInformation>();

    public int TargetNumber = 3;

    public float waitAfterEnding = 5f;

    private void Awake()
    {
        uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
    }

    private void Start()
    {
        if(!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            NewPlayerGet(PhotonNetwork.NickName); 

            // ゲームの状態を決める
            state = GameState.Playing;
        }
    }

    private void Update()
    {
        // Tab
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // 更新しつつ開く
            ShowScoreBoard();

        }
        else if(Input.GetKeyUp(KeyCode.Tab))
        {
            uiManager.ChangeScoreUI();
        }
    }

    // コールバック関数
    public void OnEvent(EventData photonEvent)
    {
        // カスタムイベントなのか判定
        if(photonEvent.Code < 200)
        {
            // イベントコード格納
            EventCodes eventCode = (EventCodes)photonEvent.Code;

            // 送られてきたイベントデータを格納する
            object[] data = (object[])photonEvent.CustomData;

            switch(eventCode)
            { 
                case EventCodes.NewPlayer:
                    NewPlayerSet(data);
                    break;

                case EventCodes.ListPlayers:
                    ListPlayersSet(data);
                    break;

                case EventCodes.UpdateStat:
                    ScoreSet(data);
                    break;

            }
        }
    }

    // コールバック登録・解除
    public override void OnEnable()
    {
        // コールバック登録
        PhotonNetwork.AddCallbackTarget(this);
    }

    // コンポーネントがオフにると呼ばれる
    public override void OnDisable()
    {
        // コールバック解除
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    // 新規ユーザーがネットワーク経由でマスターに自分の情報を送る
    public void NewPlayerGet(string name)
    {
        // 配列にじょうほうを格納する
        object[] info = new object[4];
        info[0] = name;
        info[1] = PhotonNetwork.LocalPlayer.ActorNumber;
        info[2] = 0;
        info[3] = 0;

        // 新規ユーザー発生イベント
        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.NewPlayer,
            info,
            new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
            new SendOptions { Reliability = true }
            );
    }

    // 送られてきた新プレイヤーの情報をリストに格納する
    public void NewPlayerSet(object[] data) // マスターが呼ばれる
    {
        // プレイヤーの情報を変数に格納
        PlayerInfo player = new PlayerInfo((string)data[0], (int)data[1], (int)data[2], (int)data[3]);

        // リストにプレイヤー情報を格納
        playerList.Add(player);

        // 取得したプレイヤー情報をルーム内の全プレイヤーに送信する
        ListPlayerGet();
    }

    // 取得したプレイヤー情報うぃルーム内の全プレイヤーに送信する
    public void ListPlayerGet()
    {
        // 送信するユーザー情報を格納
        object[] info = new object[playerList.Count + 1];

        info[0] = state;

        for(int i = 0; i < playerList.Count; i++)
        {
            // 配列
            object[] temp = new object[4];

            // ユーザー情報を格納する
            temp[0] = playerList[i].name;
            temp[1] = playerList[i].actor;
            temp[2] = playerList[i].kills;
            temp[3] = playerList[i].death;

            info[i + 1] = temp;
        }

        // 情報共有イベント発生
        PhotonNetwork.RaiseEvent(
           (byte)EventCodes.ListPlayers,
           info,
           new RaiseEventOptions { Receivers = ReceiverGroup.All },
           new SendOptions { Reliability = true }
           );
    }

    // 新しいプレイヤー情報をリストに格納する
    public void ListPlayersSet(object[] data)
    {
        // プレイヤー管理リストの初期化
        playerList.Clear();

        // ゲーム状態格納
        state = (GameState)data[0];

        for(int i = 1; i < data.Length; i++)
        {
            object[] info = (object[])data[i];

            PlayerInfo player = new PlayerInfo((string)info[0],
                (int)info[1],
                (int)info[2],
                (int)info[3]);

            playerList.Add(player);
        }

        StateCheck();
    }

    // キル数やデス数を取得してイベント発生させる関数
    public void ScoreGet(int actor, int stat, int amount)
    {
        // 引数の値を配列に格納する
        object[] package = new object[] { actor, stat, amount };

        // キルデスイベント発生
        PhotonNetwork.RaiseEvent(
          (byte)EventCodes.UpdateStat,
          package,
          new RaiseEventOptions { Receivers = ReceiverGroup.All },
          new SendOptions { Reliability = true }
          );
    }

    // 受け取ったデータをリストに反映
    public void ScoreSet(object[] data)
    {
        int actor = (int)data[0];
        int stat = (int)data[1];    // 0はキル 1だったらデス
        int amount = (int)data[2];

        for(int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].actor == actor)
            {
                switch(stat)
                {
                    case 0:
                        playerList[i].kills += amount;
                        break;

                    case 1:
                        playerList[i].death += amount;
                        break;
                }

                break;
            }
        }

        TargetScoreCheck();
    }

   public void ShowScoreBoard()
    {
        // スコア開く
        uiManager.ChangeScoreUI();

        // 現在表示されているスコア削除
        foreach(PlayerInformation info in playerInfoList)
        {
            Destroy(info.gameObject);
        }

        playerInfoList.Clear();

        // 参加しているユーザーの数分ループ
        foreach(PlayerInfo player in playerList)
        {
            // スコア表示UI作成して格納
            PlayerInformation newPlayerDisplay = Instantiate(uiManager.info, uiManager.info.transform.parent);

            // UIにプレイヤー情報を反映
            newPlayerDisplay.SetPlayerDetailes(player.name,player.kills,player.death);

            // 表示
            newPlayerDisplay.gameObject.SetActive(true);

            // リストに格納
            playerInfoList.Add(newPlayerDisplay);
        }
    }

    // ゲームクリア条件を達成したか確認する
    public void TargetScoreCheck()
    {
        bool clear = false;

        // クリア条件達成者はいるのか
        foreach(PlayerInfo player in playerList)
        {
            // キル数判定
            if(player.kills >= TargetNumber && TargetNumber > 0)
            {
                clear = true;
                break;
            }
        }

        if(clear)
        {
            if(PhotonNetwork.IsMasterClient&& state != GameState.Ending)
            {
                // ゲームの状態を変更
                state = GameState.Ending;

                // ゲームプレイ状態を共有
                ListPlayerGet();
            }
        }
    }


    // ゲーム状態を判定する関数
    public void StateCheck()
    {
        if(state == GameState.Ending) 
        {
            // ゲーム終了関数
            EndGame();
        }
    }

    public void EndGame()
    {
        // ネットワークオブジェクトの削除
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.DestroyAll();
        }

        // ゲーム終了パネルを表示する
        uiManager.OpenEndPanel();

        // スコアパネルを更新しながら表示
        ShowScoreBoard();

        // カーソル表示
        Cursor.lockState = CursorLockMode.None;

        // 終了後の処理
        Invoke("ProcessingAfterCompleted", waitAfterEnding);
    }

    private void ProcessingAfterCompleted()
    {
        // シーン同期設定解除
        PhotonNetwork.AutomaticallySyncScene = false;

        // ルームを抜ける
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        SceneManager.LoadScene(0);
    }
}

// プレイヤーの情報を扱うクラスの作成
[System.Serializable]
public class PlayerInfo
{
    public string name;
    public int actor, kills, death;

    // 変数の引数の値を格納する
    public PlayerInfo(string _name, int _actor, int _kills, int _death)
    {
        name = _name;
        actor = _actor;
        kills = _kills;
        death = _death;
    }
}