using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class SimpleMover : MonoBehaviourPun, IPunObservable
{
   [SerializeField]
   private float _moveSpeed = 0.15f;

   private Animator _animator;

   public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
   {
         // If owner of object (change state)
      if (stream.IsWriting) {
         stream.SendNext(transform.position);
         stream.SendNext(transform.rotation);
      }
         // If client (update state)
      else if (stream.IsReading) {
         transform.position = (Vector3) stream.ReceiveNext();
         transform.rotation = (Quaternion) stream.ReceiveNext();
      }
   }

   private void Awake()
   {
      _animator = GetComponent<Animator>();
   }

   private void Update()
   {
      if (base.photonView.IsMine)
      {
         float x = Input.GetAxisRaw("Horizontal");
         float y = Input.GetAxisRaw("Vertical");
         transform.position += (new Vector3(x, y, 0f)) * _moveSpeed;

         UpdatedMoving(x != 0f || y != 0f);
      }
   }

   private void UpdatedMoving(bool isMoving)
   {
      _animator.SetBool("Moving", isMoving);
   }
}
