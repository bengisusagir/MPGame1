using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using System.Globalization;
using System.Security.Cryptography;
using System.Linq;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("Stats")]
    public bool gameEnded = false;
    public float timeToWin;
    public float invictibleDuration;
     
    private float tagPickupTime;

    [Header("Player")]
    public string playerPrefabLocation;
    public Transform[] spawnPoints;
    public chController[] players;
    public int playerWithTag;
    private int playersInGame;

    public static GameManager instance;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        players = new chController[PhotonNetwork.PlayerList.Length];
        photonView.RPC("GameT", RpcTarget.All);
    }
    [PunRPC]
    public void WinGame(int playerId)
    {
        gameEnded = true;
        chController player = GetPlayer(playerId);
        GameUI.instance.SetWinText(player.photonPlayer.NickName);

        Invoke("GoBackToMenu", 3.0f);
    } 
    [PunRPC]
     public void GoBackToMenu()

    {
        PhotonNetwork.LeaveRoom();
        NetworkManager.instance.ChangeScene("Menu");
        Destroy(NetworkManager.instance.gameObject);
    }
    [PunRPC]
    public void GameT()
    {
        playersInGame++;
        
        if(playersInGame == PhotonNetwork.PlayerList.Length)
        {
            SpawnPlayer();
        }
    }
    [PunRPC]
    public void GiveTag(int playerId, bool initialGive)
    {
        if (!initialGive)
            GetPlayer(playerWithTag).SetTag(false);

        playerWithTag = playerId;
        GetPlayer(playerId).SetTag(true);
        tagPickupTime = Time.time;
    }
    public bool CanGetTag()
    {
        if (Time.time > tagPickupTime + invictibleDuration)
            return true;

        else
            return false;
    }
    [PunRPC]
    void SpawnPlayer()
    {
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[Random.Range(0,spawnPoints.Length)].position,Quaternion.identity);
        chController playerScript = playerObj.GetComponent<chController>();

        playerScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    public chController GetPlayer(int playerId)
    {
        return players.First(x => x.id == playerId);
    }
    public chController GetPlayer(GameObject playerObj)
    {
        return players.First(x => x.gameObject == playerObj);
    }
}
