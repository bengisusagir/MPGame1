using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class GameUI : MonoBehaviour { 

    public PlayerUIContainer[] playerContainers;
    public TextMeshProUGUI winText;

    public static GameUI instance;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        
    }
    
    void InitializelPlayerUI()
    {
        for(int x = 0; x < playerContainers.Length; ++x)
        {
            PlayerUIContainer container = playerContainers[x];
            if (x < PhotonNetwork.PlayerList.Length)
            {
                container.obj.SetActive(true);
                container.nameText.text = PhotonNetwork.PlayerList[x].NickName;
                container.tagTimeSlider.maxValue = GameManager.instance.timeToWin;
                Debug.Log("aktif");
            }
            else
            {
                container.obj.SetActive(false);
                Debug.Log("yok");
            }
        }
       

    }

    private void Update()
    {
        UpdatePlayerUI();
    }

    void UpdatePlayerUI()
    {
        for (int x = 0; x < GameManager.instance.players.Length; ++x)
        {
            if (GameManager.instance.players[x] != null)
            {
                playerContainers[x].tagTimeSlider.value = GameManager.instance.players[x].curTagTime;
            }
        }
    }

    public void SetWinText(string winnerName)
    {
        winText.gameObject.SetActive(true);
        winText.text = winnerName + "wins";
    }
}
    


[System.Serializable]
    public class PlayerUIContainer
    {
        public GameObject obj;
        public TextMeshProUGUI nameText;
        public Slider tagTimeSlider;
    }


