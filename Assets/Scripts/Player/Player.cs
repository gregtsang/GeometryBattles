using System.Collections.Generic;
using UnityEngine;

namespace GeometryBattles.PlayerManager
{
    public class Player : MonoBehaviour
    {   
        [SerializeField] int resource = 0;
        HashSet<Vector2Int> tiles = new HashSet<Vector2Int>();
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

        public int GetNumTiles()
        {
            return tiles.Count;
        }

        public bool OwnsTile(int q, int r)
        {
            return tiles.Contains(new Vector2Int(q, r));
        }

        public void AddTile(int q, int r)
        {
            tiles.Add(new Vector2Int(q, r));
        }

        public void RemoveTile(int q, int r)
        {
            tiles.Remove(new Vector2Int(q, r));
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