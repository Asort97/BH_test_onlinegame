using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerSpawner playerSpawner;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private Text winnerText;
    [SerializeField] private float timeRespawn;
    public static GameManager gameManager;
    private void Awake()
    {
        GameManager.gameManager = this;        
    }
    
    public void EndGame(string nickWinner)
    {
        winPanel.SetActive(true);
        winnerText.text = nickWinner;

        StartCoroutine(RestartGame(timeRespawn));
    }

    private IEnumerator RestartGame(float time)
    {
        yield return new WaitForSeconds(time);
        RestartGame();
    }
    private void RestartGame()
    {
        PhotonNetwork.LoadLevel("Gameplay");
    }
}
