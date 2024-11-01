using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;


public class GameManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    // �v���C���[�����i�[���郊�X�g�̍쐬
    public List<PlayerInfo> playerList = new List<PlayerInfo>();

    // �C�x���g�쐬
    public enum EventCodes : byte
    {
        NewPlayer,
        ListPlayers,
        UpdateStat
    }

    // �Q�[����ԍ쐬
    public enum GameState
    {
        Playing,
        Ending
    }

    // �Q�[����Ԋi�[
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

            // �Q�[���̏�Ԃ����߂�
            state = GameState.Playing;
        }
    }

    private void Update()
    {
        // Tab
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // �X�V���J��
            ShowScoreBoard();

        }
        else if(Input.GetKeyUp(KeyCode.Tab))
        {
            uiManager.ChangeScoreUI();
        }
    }

    // �R�[���o�b�N�֐�
    public void OnEvent(EventData photonEvent)
    {
        // �J�X�^���C�x���g�Ȃ̂�����
        if(photonEvent.Code < 200)
        {
            // �C�x���g�R�[�h�i�[
            EventCodes eventCode = (EventCodes)photonEvent.Code;

            // �����Ă����C�x���g�f�[�^���i�[����
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

    // �R�[���o�b�N�o�^�E����
    public override void OnEnable()
    {
        // �R�[���o�b�N�o�^
        PhotonNetwork.AddCallbackTarget(this);
    }

    // �R���|�[�l���g���I�t�ɂ�ƌĂ΂��
    public override void OnDisable()
    {
        // �R�[���o�b�N����
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    // �V�K���[�U�[���l�b�g���[�N�o�R�Ń}�X�^�[�Ɏ����̏��𑗂�
    public void NewPlayerGet(string name)
    {
        // �z��ɂ��傤�ق����i�[����
        object[] info = new object[4];
        info[0] = name;
        info[1] = PhotonNetwork.LocalPlayer.ActorNumber;
        info[2] = 0;
        info[3] = 0;

        // �V�K���[�U�[�����C�x���g
        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.NewPlayer,
            info,
            new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
            new SendOptions { Reliability = true }
            );
    }

    // �����Ă����V�v���C���[�̏������X�g�Ɋi�[����
    public void NewPlayerSet(object[] data) // �}�X�^�[���Ă΂��
    {
        // �v���C���[�̏���ϐ��Ɋi�[
        PlayerInfo player = new PlayerInfo((string)data[0], (int)data[1], (int)data[2], (int)data[3]);

        // ���X�g�Ƀv���C���[�����i�[
        playerList.Add(player);

        // �擾�����v���C���[�������[�����̑S�v���C���[�ɑ��M����
        ListPlayerGet();
    }

    // �擾�����v���C���[��񂤂����[�����̑S�v���C���[�ɑ��M����
    public void ListPlayerGet()
    {
        // ���M���郆�[�U�[�����i�[
        object[] info = new object[playerList.Count + 1];

        info[0] = state;

        for(int i = 0; i < playerList.Count; i++)
        {
            // �z��
            object[] temp = new object[4];

            // ���[�U�[�����i�[����
            temp[0] = playerList[i].name;
            temp[1] = playerList[i].actor;
            temp[2] = playerList[i].kills;
            temp[3] = playerList[i].death;

            info[i + 1] = temp;
        }

        // ��񋤗L�C�x���g����
        PhotonNetwork.RaiseEvent(
           (byte)EventCodes.ListPlayers,
           info,
           new RaiseEventOptions { Receivers = ReceiverGroup.All },
           new SendOptions { Reliability = true }
           );
    }

    // �V�����v���C���[�������X�g�Ɋi�[����
    public void ListPlayersSet(object[] data)
    {
        // �v���C���[�Ǘ����X�g�̏�����
        playerList.Clear();

        // �Q�[����Ԋi�[
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

    // �L������f�X�����擾���ăC�x���g����������֐�
    public void ScoreGet(int actor, int stat, int amount)
    {
        // �����̒l��z��Ɋi�[����
        object[] package = new object[] { actor, stat, amount };

        // �L���f�X�C�x���g����
        PhotonNetwork.RaiseEvent(
          (byte)EventCodes.UpdateStat,
          package,
          new RaiseEventOptions { Receivers = ReceiverGroup.All },
          new SendOptions { Reliability = true }
          );
    }

    // �󂯎�����f�[�^�����X�g�ɔ��f
    public void ScoreSet(object[] data)
    {
        int actor = (int)data[0];
        int stat = (int)data[1];    // 0�̓L�� 1��������f�X
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
        // �X�R�A�J��
        uiManager.ChangeScoreUI();

        // ���ݕ\������Ă���X�R�A�폜
        foreach(PlayerInformation info in playerInfoList)
        {
            Destroy(info.gameObject);
        }

        playerInfoList.Clear();

        // �Q�����Ă��郆�[�U�[�̐������[�v
        foreach(PlayerInfo player in playerList)
        {
            // �X�R�A�\��UI�쐬���Ċi�[
            PlayerInformation newPlayerDisplay = Instantiate(uiManager.info, uiManager.info.transform.parent);

            // UI�Ƀv���C���[���𔽉f
            newPlayerDisplay.SetPlayerDetailes(player.name,player.kills,player.death);

            // �\��
            newPlayerDisplay.gameObject.SetActive(true);

            // ���X�g�Ɋi�[
            playerInfoList.Add(newPlayerDisplay);
        }
    }

    // �Q�[���N���A������B���������m�F����
    public void TargetScoreCheck()
    {
        bool clear = false;

        // �N���A�����B���҂͂���̂�
        foreach(PlayerInfo player in playerList)
        {
            // �L��������
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
                // �Q�[���̏�Ԃ�ύX
                state = GameState.Ending;

                // �Q�[���v���C��Ԃ����L
                ListPlayerGet();
            }
        }
    }


    // �Q�[����Ԃ𔻒肷��֐�
    public void StateCheck()
    {
        if(state == GameState.Ending) 
        {
            // �Q�[���I���֐�
            EndGame();
        }
    }

    public void EndGame()
    {
        // �l�b�g���[�N�I�u�W�F�N�g�̍폜
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.DestroyAll();
        }

        // �Q�[���I���p�l����\������
        uiManager.OpenEndPanel();

        // �X�R�A�p�l�����X�V���Ȃ���\��
        ShowScoreBoard();

        // �J�[�\���\��
        Cursor.lockState = CursorLockMode.None;

        // �I����̏���
        Invoke("ProcessingAfterCompleted", waitAfterEnding);
    }

    private void ProcessingAfterCompleted()
    {
        // �V�[�������ݒ����
        PhotonNetwork.AutomaticallySyncScene = false;

        // ���[���𔲂���
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        SceneManager.LoadScene(0);
    }
}

// �v���C���[�̏��������N���X�̍쐬
[System.Serializable]
public class PlayerInfo
{
    public string name;
    public int actor, kills, death;

    // �ϐ��̈����̒l���i�[����
    public PlayerInfo(string _name, int _actor, int _kills, int _death)
    {
        name = _name;
        actor = _actor;
        kills = _kills;
        death = _death;
    }
}