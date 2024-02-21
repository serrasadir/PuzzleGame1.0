using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlastGame;

namespace BlastGame { 
    public class Cube : MonoBehaviour
    {
        public CubeColor cubeColor;
        public CubeType cubeType;
        public int xPos;
        public int yPos;

        public bool isObstacle;

        [SerializeField] private Sprite tntSpriteRed;
        [SerializeField] private Sprite tntSpriteBlue;
        [SerializeField] private Sprite tntSpriteGreen;
        [SerializeField] private Sprite tntSpriteYellow;

        [SerializeField] private Sprite normalSpriteRed;
        [SerializeField] private Sprite normalSpriteBlue;
        [SerializeField] private Sprite normalSpriteGreen;
        [SerializeField] private Sprite normalSpriteYellow;

        [SerializeField] private Sprite TNT;
        [SerializeField] private Sprite Box;
        [SerializeField] private Sprite Stone;
        [SerializeField] private Sprite Vase;
        [SerializeField] private Sprite BrokenVase;

        public void SetIndices(int _x, int _y)
        {
            xPos = _x;
            yPos = _y;
        }
        private void OnMouseDown()
        {
            
            BlastGame.GameManager.Instance.OnCubeClicked(this);

        }

        public void MoveToTargetPos(Vector3 targetPos)
        {
            StartCoroutine(MoveToPosition(targetPos, 0.5f));
        }

        public IEnumerator MoveToPosition(Vector3 targetPos, float timeToMove) //move the cubes with lerp to provide a smooth and nice transition
        {
            Vector3 startPosition = transform.position;
            float elapsedTime = 0;

            while (elapsedTime < timeToMove)
            {
                transform.position = Vector3.Lerp(startPosition, targetPos, (elapsedTime / timeToMove));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = targetPos; //just to ensure the pos
        }
        public void ChangeToTntHint()//change the sprites to tnt hint versions
        {
            this.cubeType = CubeType.Tnt_hint;
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            switch (this.cubeColor)
            {
                case CubeColor.Yellow:
                    spriteRenderer.sprite = tntSpriteYellow; 
                    break;
                case CubeColor.Blue:
                    spriteRenderer.sprite = tntSpriteBlue;
                    break;
                case CubeColor.Red:
                    spriteRenderer.sprite = tntSpriteRed;
                    break;
                case CubeColor.Green:
                    spriteRenderer.sprite = tntSpriteGreen;
                    break;
            }
        }
        public void ChangeToNormal() //change the sprites from tnt hint version to normal version 
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            this.cubeType = CubeType.Normal;
            switch (this.cubeColor)
            {
                case CubeColor.Yellow:
                    spriteRenderer.sprite = normalSpriteYellow;
                    break;
                case CubeColor.Blue:
                    spriteRenderer.sprite = normalSpriteBlue;
                    break;
                case CubeColor.Red:
                    spriteRenderer.sprite = normalSpriteRed;
                    break;
                case CubeColor.Green:
                    spriteRenderer.sprite = normalSpriteGreen;
                    break;
            }
        }

        public void ChangeToTNT() //change the cube to a tnt
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            this.cubeType = CubeType.TNT;
            this.cubeColor = CubeColor.TNT;
            spriteRenderer.sprite = TNT;
        }
        public void ChangeToTNTAbout()
        {
            this.cubeType = CubeType.TNT_about_to_convert;

        }
        public void ChangeToBrokenVase()
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = BrokenVase;
            this.cubeColor = CubeColor.BrokenVase;
        }


    }


    public enum CubeType
    {
        Normal,
        Tnt_hint,
        TNT_about_to_convert,
        TNT,
        Obstacle,//stone, box, vase
    }

    public enum CubeColor
    {
        Red,
        Blue,
        Yellow,
        Green,
        TNT,
        Vase,
        BrokenVase,
        Box,
        Stone
    }
}