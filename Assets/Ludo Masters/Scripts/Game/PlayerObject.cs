/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayerObject
{

    // LUDO
    public GameObject dice;
    public GameObject[] pawns;
    public GameObject homeLockObjects;
    public bool canEnterHome = false;


    // END LUDO


    public string name;
    public Sprite avatar;
    public string id;
    public GameObject timer;
    public bool isActive = true;
    public GameObject AvatarObject;
    public GameObject ChatBubble;
    public GameObject ChatBubbleText;
    public GameObject ChatbubbleImage;
    public MyPlayerData data;
    public bool isBot = false;
    public int finishedPawns = 0;

    public PlayerObject(string name, string id, Sprite avatar)
    {
        this.name = name;
        this.id = id;
        this.avatar = avatar;
        this.timer = timer;
        if (!id.Contains("_BOT"))
        {
            this.isBot = false;
            getPlayerDataRequest(this.id);
        }
        else
        {
            this.isBot = true;
            this.data = new MyPlayerData();
            this.data.data = new Dictionary<string, UserDataRecord>();


            UserDataRecord record3 = new UserDataRecord();
            record3.Value = Random.Range(500, 1000).ToString();
            this.data.data.Add(MyPlayerData.GamesPlayedKey, record3);
            UserDataRecord record4 = new UserDataRecord();
            record4.Value = Random.Range(1, 250).ToString();
            this.data.data.Add(MyPlayerData.TwoPlayerWinsKey, record4);
            UserDataRecord record5 = new UserDataRecord();
            record5.Value = Random.Range(1, 250).ToString();
            this.data.data.Add(MyPlayerData.FourPlayerWinsKey, record5);
            UserDataRecord record = new UserDataRecord();
            record.Value = (Random.Range(10000, 50000) * 100).ToString();
            this.data.data.Add(MyPlayerData.TotalEarningsKey, record);
            UserDataRecord record2 = new UserDataRecord();
            record2.Value = (Random.Range(1, 10000) * 100).ToString();
            this.data.data.Add(MyPlayerData.CoinsKey, record2);

        }
    }

    public void getPlayerDataRequest(string id)
    {
        Debug.Log("Get player data request: " + id);
        GetUserDataRequest getdatarequest = new GetUserDataRequest()
        {
            PlayFabId = id,
        };

        PlayFabClientAPI.GetUserData(getdatarequest, (result) =>
        {

            Dictionary<string, UserDataRecord> data = result.Data;

            this.data = new MyPlayerData(data, false);


        }, (error) =>
        {
            Debug.Log("Data updated error " + error.ErrorMessage);
        }, null);
    }
}
