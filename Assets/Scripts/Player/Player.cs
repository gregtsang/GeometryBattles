using System.Collections;
using UnityEngine;

namespace GeometryBattles.PlayerManager
{
    public class Player : MonoBehaviour
    {   
        [SerializeField] int resource = 0;
        Color color;

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

        public Color GetColor()
        {
            return color;
        }

        public void SetColor(Color color)
        {
            this.color = color;
        }
    }
}