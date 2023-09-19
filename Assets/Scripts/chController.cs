using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using System.IO;

public class chController : MonoBehaviourPunCallbacks, Photon.Pun.IPunObservable
{
    public float jumpForce;
    public float rotationSpeed = 90f;
    public Rigidbody rig;
    private Animator animator;
    public Player photonPlayer;
    public GameObject tagObject;

    [HideInInspector]
    public int id;
    [HideInInspector]
    public float tagTime;
    [HideInInspector]
    public float curTagTime;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (tagTime >= GameManager.instance.timeToWin && !GameManager.instance.gameEnded)
            {
                GameManager.instance.gameEnded = true;
                GameManager.instance.photonView.RPC("WinGame", RpcTarget.All, id);
            }
        }

        if (photonView.IsMine)
        {


            if (Input.GetKey(KeyCode.W))
            {
                animator.SetBool("isRun", true);
                transform.Translate(new Vector3(0, 0, 4f) * Time.deltaTime);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    animator.SetBool("isRunToT", true);
                }
                else
                    animator.SetBool("isRunToT", false);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                animator.SetBool("isRun", true);
                transform.Translate(new Vector3(0, 0, -4f) * Time.deltaTime);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    animator.SetBool("isRunToT", true);
                }
                else
                    animator.SetBool("isRunToT", false);
            }
            else
                animator.SetBool("isRun", false);

            if (photonView.IsMine)
            {
                if (tagObject.activeInHierarchy)
                {
                    tagTime += Time.deltaTime;
                }
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                animator.SetBool("isTouch", true);
            }
            else
                animator.SetBool("isTouch", false);

            if (Input.GetKeyDown(KeyCode.Space))
            {

                Ray ray = new Ray(transform.position, Vector3.down);


                rig.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);


            }

            if (Input.GetKey(KeyCode.D))
            {
                transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.A))
            {

                transform.Rotate(Vector3.down * rotationSpeed * Time.deltaTime);
            }

        }
    }
    public void SetTag(bool hasTag)
    {
        tagObject.SetActive(hasTag);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (!photonView.IsMine)
            return;
        if (collision.gameObject.CompareTag("Player"))
        {
            if (GameManager.instance.GetPlayer(collision.gameObject).id == GameManager.instance.playerWithTag)
            {
                if (GameManager.instance.CanGetTag())
                {
                    GameManager.instance.photonView.RPC("GiveTag", RpcTarget.All, id, false);

                }
            }
        }

    }
    [PunRPC]
    public void Initialize(Player player)
    {
        photonPlayer = player;
        id = player.ActorNumber;
        GameManager.instance.players[id - 1] = this;

        if (id == 1)
            GameManager.instance.GiveTag(id, true);

        if (!photonView.IsMine)
        {
            rig.isKinematic = true;
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(curTagTime);
        }
        else if (stream.IsReading)
        {
            curTagTime = (float)stream.ReceiveNext();
        }
    }
}
