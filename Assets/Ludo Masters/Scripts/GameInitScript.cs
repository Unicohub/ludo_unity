/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInitScript : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {


        if (GameManager.Instance.roomOwner)
        {
            PhotonNetwork.RaiseEvent(198, null, true, null);
        }
        else
        {
            // for (int i = 0; i < GameManager.Instance.initPositions.Length; i++) {
            //     GameManager.Instance.balls[i + 1].GetComponent<Rigidbody>().transform.position = GameManager.Instance.initPositions[i];
            //     GameManager.Instance.balls[i + 1].SetActive(true);
            // }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
