using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private float speedMove;
    [SerializeField] private float forceDash;
    [SerializeField] private Transform mainCamera;
    [SerializeField] private Transform playerRig;
    [SerializeField] private float dashCd;
    [SerializeField] private float timeOfDash;
    [SerializeField] private float radiusDash;
    [SerializeField] private float timeImmortal;
    [SerializeField] private Animator anim;
    [SerializeField] private LayerMask layerEnemy;
    [SerializeField] private Material redMat;
    [SerializeField] private Material blackMat;
    [SerializeField] private GameObject bodyPlayer;
    private GameManager gameManager;
    private PhotonView photonView;
    private HitsCounter hitsCounter;
    private Rigidbody rb;
    private bool canDash = true;
    private bool IsDash;
    
    private float timerDash;
    private float moveHorizontal;
    private float moveVertical;

    public int countHit;
    public bool isImmortal;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(moveHorizontal);
            stream.SendNext(moveVertical);
            stream.SendNext(countHit);

        }
        else
        {
            moveHorizontal = (float) stream.ReceiveNext();
            moveVertical = (float) stream.ReceiveNext();
            countHit = (int) stream.ReceiveNext();
        }
    }

    private void Start()
    {
        gameManager = GameManager.gameManager;
        hitsCounter = GetComponent<HitsCounter>();
        photonView = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();
        

        if(!photonView.IsMine)
        {
            mainCamera.gameObject.SetActive(false);
        }
    }
    private void Update()
    {
        if(photonView.IsMine)
        {
            Movement();
            RotateDir();
            SuperDash();
            Attack();

            if(Input.GetKeyDown(KeyCode.E))
            {
                GetHit();
            }

        }

    }
    private void Attack()
    {
        if(IsDash)
        {
            Collider[] players = Physics.OverlapSphere(transform.position, radiusDash, layerEnemy);
            if(players != null)
            {
                foreach(Collider player in players)
                {
                    PlayerController playerController = player.GetComponent<PlayerController>();
                    if(!player.GetComponent<PhotonView>().IsMine && !playerController.isImmortal)
                    {
                        playerController.GetHit();

                        if(playerController.countHit >= 3)
                        {
                            photonView.RPC("PlayerWin", RpcTarget.All, PhotonNetwork.NickName);
                        }
                    }
                }
            }
        }        
    }
    private void RotateDir()
    {
        if(photonView.IsMine)
        {
            Vector3 rot = new Vector3(0, mainCamera.eulerAngles.y, 0);
            playerRig.rotation = Quaternion.Euler(rot);            
        }
    }
    private void SuperDash()
    {            
        if(Input.GetMouseButtonDown(0))
        {
            if(canDash)
            {        
                rb.AddForce(mainCamera.forward * forceDash, ForceMode.Impulse);
                StartCoroutine(PlayerDashing(timeOfDash));
                canDash = false;
            }
        }
        if(!canDash)
        {
            if(timerDash >= 0)
            {
                timerDash -= Time.deltaTime;
            }
            else
            {
                canDash = true;
                timerDash = dashCd;
            }
        }     

    }

    private void Movement()
    {
        if(photonView.IsMine)
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            Vector3 movementZ = new Vector3(mainCamera.forward.x, 0,  mainCamera.forward.z);
            Vector3 movementX = new Vector3(mainCamera.right.x, 0f, mainCamera.right.z);
            
            if(moveVertical > 0)
            {
                anim.SetBool("isRun", true);
                transform.Translate(movementZ * speedMove * Time.deltaTime); 
            }
            else if(moveVertical < 0)
            {
                transform.Translate(-movementZ * speedMove * Time.deltaTime); 
            }

            if(moveHorizontal > 0)
            {
                transform.Translate(movementX * speedMove * Time.deltaTime); 
            }
            else if(moveHorizontal < 0)
            {
                
                transform.Translate(-movementX * speedMove * Time.deltaTime); 
            }        
            anim.SetFloat("Run", moveVertical);
            anim.SetFloat("Turn", moveHorizontal);            
        }
    }

    public void GetHit()
    {
        photonView.RPC("GettingHit", RpcTarget.All);
        
    }

    [PunRPC]
    private void GettingHit()
    {
        StartCoroutine(ChangeColor(timeImmortal));
        countHit++;
        hitsCounter.UpdateCounter(countHit);
    }

    [PunRPC]
    private void PlayerWin(string nick)
    {
        gameManager.EndGame(nick);
    }    
    private IEnumerator ChangeColor(float time)
    {
        isImmortal = true;
        ChangeMaterial(redMat);
        yield return new WaitForSeconds(time);
        ChangeMaterial(blackMat); 
        isImmortal = false;
    }
    private void ChangeMaterial(Material mat)
    {
        Material[] materials = bodyPlayer.GetComponent<Renderer>().materials;
    
        materials[0] = mat;
        materials[1] = mat; 

        bodyPlayer.GetComponent<Renderer>().materials = materials;
    }
    private IEnumerator PlayerDashing(float time)
    {
        IsDash = true;
        yield return new WaitForSeconds(time);
        IsDash = false;
    }
}
