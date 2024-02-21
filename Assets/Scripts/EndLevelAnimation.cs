using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace BlastGame
{
    public class EndLevelAnimation : MonoBehaviour
    {
        public GameObject starParticles;
        public GameObject star;

        public void CelebrationAnimation()
        {
            Vector3 starCenter = new Vector3(0, 0, 0);

            for (int i = 0; i < 20; i++)
            {
                GameObject sparkle = Instantiate(starParticles, starCenter, Quaternion.identity);
                Vector2 spreadDirection = Random.insideUnitCircle.normalized;

                sparkle.transform.DOScale(0.1f, 0.3f).SetEase(Ease.OutQuad);

                float spreadDistance = 5.0f;
                sparkle.transform.DOMove(starCenter + new Vector3(spreadDirection.x, spreadDirection.y, 0) * spreadDistance, 2.0f).SetEase(Ease.OutQuad).OnComplete(() =>
                {
                    Destroy(sparkle);
                });
            }
        }
    }
}