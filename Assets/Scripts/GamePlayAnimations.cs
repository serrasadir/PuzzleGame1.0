using UnityEngine;
using DG.Tweening;

namespace BlastGame
{
    public class GamePlayAnimations : MonoBehaviour
    {
        [SerializeField] private GameObject[] boxParticles;
        [SerializeField] private GameObject[] vaseParticles;
        [SerializeField] private GameObject[] stoneParticles;
        //[SerializeField] private GameObject[] tntParticles;
        public GameObject redParticles;
        public GameObject blueParticles;
        public GameObject yellowParticles;
        public GameObject greenParticles;
        public GameObject tntParticles;
        public GameObject tntCircle;

        public int numberOfparticles = 3;
        public float explosionForce = 1.0f;
        public float explosionRadius = 1.0f;
        public float upForce = 1.0f;
        public float particleLifetime = 2.0f;

        public void PopAnimation(CubeColor cubeColor)
        {
            if (cubeColor == CubeColor.TNT) { CircleAnimation(); }
            gameObject.SetActive(false);

            for (int i = 0; i < numberOfparticles; i++)
            {
                GameObject particleColor = GetParticle(cubeColor);
                Vector3 position = new Vector3(transform.position.x + Random.Range(-0.2f, 0.2f), transform.position.y + Random.Range(-0.2f, 0.2f), 0); //used Random to spread particles randomly
                GameObject particle = Instantiate(particleColor, position, Random.rotation);

                Rigidbody particleRb = particle.AddComponent<Rigidbody>();
                Vector3 explosionDir = (particle.transform.position - transform.position).normalized + Vector3.up * upForce;
                particleRb.AddForce(explosionDir * explosionForce, ForceMode.VelocityChange);
                particle.transform.DOScale(0, particleLifetime).SetEase(Ease.InQuad).OnComplete(() =>
                {
                    Destroy(particle);//destroy particles after the animation
                });

            }
            Destroy(gameObject, particleLifetime);
        }



        public void CircleAnimation()
        {
            gameObject.SetActive(false);

            GameObject circleInstance = Instantiate(tntCircle, transform.position, Quaternion.identity);
            circleInstance.transform.DOScale(0.4f, 0.2f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                circleInstance.transform.DOScale(0, 0.2f).SetEase(Ease.InQuad).OnComplete(() =>
                {
                    Destroy(circleInstance);//destroy circle after the animation
                });
            });
            Destroy(gameObject, 0.2f);
        }

        private GameObject GetParticle(CubeColor color)
        {
            int rand = Random.Range(0, 2);//for those have 3 particles, choose randomly
            switch (color)
            {
                case CubeColor.Red:
                    return redParticles;
                case CubeColor.Blue:
                    return blueParticles;
                case CubeColor.Yellow:
                    return yellowParticles;
                case CubeColor.Green:
                    return greenParticles;
                case CubeColor.Stone:
                    return stoneParticles[rand];
                case CubeColor.Box:
                    return boxParticles[rand];
                case CubeColor.BrokenVase:
                    return vaseParticles[rand];
                case CubeColor.TNT:
                    return tntParticles;
                default:
                    return null;
            }
        }
    }

}