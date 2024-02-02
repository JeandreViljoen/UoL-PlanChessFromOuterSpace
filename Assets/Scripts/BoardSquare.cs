using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace PlanChess
{
    public class BoardSquare : MonoBehaviour
    {

        // --------------- Member variables and data --------------- //
        public int IndexX;
        public int IndexZ;
        public GameObject ChessPieceAssigned;

        private Tween _bounceAnimateTween;

        void Start()
        {
            _bounceAnimateTween = transform.DOMove(transform.localPosition, 0.2f).SetEase(Ease.InOutSine);
        }
    
        void Update()
        {
        
        }
    
        // --------------- Public Functions and Methods ---------------
    
        // Returns the chess piece assigned to this board square
        public ChessPiece GetChessPiece()
        {
            return ChessPieceAssigned.GetComponent<ChessPiece>();
        }

        // Returns true if there is no chess piece on this board square
        public bool IsEmpty()
        {
            return (ChessPieceAssigned == null);
        }
    
        // Destroys the chess piece assigned to this board square, use cautiously
        public void DestroyChessPiece()
        {
            Debug.Log("Destroying chess piece assigned to board square with index [" + IndexX.ToString() + "," + IndexZ.ToString() + "]");
            Destroy(ChessPieceAssigned);
        }
    
        //TEMP
        public void BounceAnimate()
        {
            if (_bounceAnimateTween.IsPlaying())
            {
                return;
            }
        
        
            Sequence s = DOTween.Sequence();
            s.Append(transform.DOMove( transform.position + Vector3.up *3 , 1f).SetEase(Ease.InOutSine));
            s.Append(transform.DOMove( transform.position , 1f).SetEase(Ease.InOutSine));

            _bounceAnimateTween = s;
        }
    
    }
}

