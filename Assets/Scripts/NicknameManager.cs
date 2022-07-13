using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class NicknameManager : MonoBehaviour
{
    [SerializeField] private InputField nameInputField;

    public void PlacePlayerName()
    {
        string PlayerNickname = nameInputField.text;
        PhotonNetwork.NickName = PlayerNickname;
        PlayerPrefs.SetString("PlayerName", PlayerNickname);
    }

}
