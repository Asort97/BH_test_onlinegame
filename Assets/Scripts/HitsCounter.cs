using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class HitsCounter : MonoBehaviour
{
    [SerializeField] private Text countHit;
    private PhotonView photonView;
    private void Start()
    {
        photonView = GetComponent<PhotonView>();
    }
    public void UpdateCounter(int value)
    {
        if(photonView.IsMine)
        {
            countHit.text = value.ToString();
            Debug.Log(value);
        }
        
    }
        
}
