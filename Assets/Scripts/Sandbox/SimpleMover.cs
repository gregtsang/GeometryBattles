using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class SimpleMover : MonoBehaviourPun
{
   [SerializeField]
   private float _moveSpeed = 0.15f;

   private Animator _animator;

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
