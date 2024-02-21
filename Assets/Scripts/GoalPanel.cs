using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace BlastGame
{
    public class GoalPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI boxCountText;
        [SerializeField] private TextMeshProUGUI stoneCountText;
        [SerializeField] private TextMeshProUGUI vaseCountText;
        [SerializeField] private TextMeshProUGUI moveCountText;
        [SerializeField] private GameObject boxCheck;
        [SerializeField] private GameObject vaseCheck;
        [SerializeField] private GameObject stoneCheck;



        public void UpdateMoveCount(int moveCount)
        {
            moveCountText.text = moveCount > 9 ? moveCount.ToString() : " " + moveCount.ToString();
        }
        public void UpdateBoxCount(int boxCount)
        {
            if (boxCount > 0)
            {
                boxCountText.gameObject.SetActive(true);
                boxCheck.SetActive(false);
                boxCountText.text = boxCount.ToString();
            }
            else
            {
                boxCountText.gameObject.SetActive(false); //hide the text
                boxCheck.SetActive(true); //show check icon
            }
        }
        public void UpdateStoneCount(int stoneCount)
        {
            if (stoneCount > 0)
            {
                stoneCountText.gameObject.SetActive(true);
                stoneCheck.SetActive(false);
                stoneCountText.text = stoneCount.ToString();
            }
            else
            {
                stoneCountText.gameObject.SetActive(false);
                stoneCheck.SetActive(true); 
            }
        }
        public void UpdateVaseCount(int vaseCount)
        {

            if (vaseCount > 0)
            {
                vaseCountText.gameObject.SetActive(true);
                vaseCheck.SetActive(false);
                vaseCountText.text = vaseCount.ToString();
            }
            else
            {
                vaseCountText.gameObject.SetActive(false); 
                vaseCheck.SetActive(true); 
            }
        }

    }
}