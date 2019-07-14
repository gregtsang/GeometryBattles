using UnityEngine;

namespace GeometryBattles.PlayerManager
{
    public class PlayerPrefab : MonoBehaviour
    {
        public int miningAmount = 1;
        public float miningRate = 1.0f;
        float miningTimer = 1.0f;
        
        public int startResource = 0;
        [SerializeField] int resource = 0;

        float colorHue;

        void Start()
        {
            resource = startResource;
            miningTimer = miningRate;
        }

        void Update()
        {
            miningTimer -= Time.deltaTime;
            if (miningTimer <= 0.0f)
            {
                resource += miningAmount;
                miningTimer = miningRate;
            }
        }

        public int GetResource()
        {
            return resource;
        }

        public void AddResource(int amount)
        {
            resource += amount;
        }

        public void AddMiningAmount(int amount)
        {
            miningAmount += amount;
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