using System.Collections;
using UnityEngine;

namespace GeometryBattles.PlayerManager
{
    public class Player : MonoBehaviour
    {
        public int miningAmount = 1;
        public float miningRate = 1.0f;
        
        [SerializeField] int resource = 0;

        float colorHue;

        void Start()
        {
            StartCoroutine("MineResource");
        }

        public int GetResource()
        {
            return resource;
        }

        public void SetResource(int amount)
        {
            resource = amount;
        }

        public void AddResource(int amount)
        {
            resource += amount;
        }

        public void SetMiningAmount(int amount)
        {
            miningAmount = amount;
        }

        public void AddMiningAmount(int amount)
        {
            miningAmount += amount;
        }

        IEnumerator MineResource()
        {
            while (true)
            {
                resource += miningAmount;
                yield return new WaitForSeconds(miningRate);
            }
        }

        public float GetColor()
        {
            return colorHue;
        }

        public void SetColor(float hue)
        {
            colorHue = hue;
        }
    }
}