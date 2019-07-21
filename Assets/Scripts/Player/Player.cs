using UnityEngine;

namespace GeometryBattles.PlayerManager
{
    public class Player : MonoBehaviour
    {   
        [SerializeField] int resource = 0;
        private int _id;
        Color color;

        public int Id { get => _id; set => _id = value; }

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