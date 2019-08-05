using System.Collections.Generic;
using UnityEngine;
using GeometryBattles.PlayerManager;

namespace GeometryBattles.StructureManager
{
    public class Pentagon : Structure
    {
        public PentagonData stats;

        int targetQ, targetR;
        bool target = false;
        bool bombard = false;
        float bombTimer;

        void OnEnable()
        {
            stats = this.gameObject.GetComponent<PentagonData>();
        }

        void Start()
        {
            hp = stats.currLevel.maxHP;
            maxhp = stats.currLevel.maxHP;
            regen = stats.currLevel.regen;
            armor = stats.currLevel.armor;
        }

        void Update()
        {
            if (bombard && target && CheckSpace(q, r, player))
            {
                bombTimer -= Time.deltaTime;
                if (bombTimer <= 0.0f)
                {
                    Bombard();
                    bombTimer = stats.currLevel.bombRate;
                }
            }
        }

        public override void StartEffect()
        {
            StartCoroutine(RegenHP());
            bombTimer = 0.0f;
            bombard = true;
        }

        public override bool CheckSpace(int q, int r, Player player)
        {
            List<Vector2Int> tiles = new List<Vector2Int>();
            tiles.Add(new Vector2Int(q - 1, r));
            tiles.Add(new Vector2Int(q + 1, r));
            tiles.Add(new Vector2Int(q, r - 1));
            tiles.Add(new Vector2Int(q, r + 1));
            tiles.Add(new Vector2Int(q - 1, r + 1));
            tiles.Add(new Vector2Int(q + 1, r - 1));
            foreach (Vector2Int t in tiles)
            {
                if (!boardState.IsValidTile(t[0], t[1]) || !boardState.IsOwned(t[0], t[1]) || boardState.GetNodeOwner(t[0], t[1]) != player)
                    return false;
            }
            return true;
        }

        public void SetTarget(int q, int r)
        {
            targetQ = q;
            targetR = r;
            target = true;
        }

        void Bombard()
        {
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
            visited.Add(new Vector2Int(targetQ, targetR));
            Queue<Vector3Int> queue = new Queue<Vector3Int>();
            queue.Enqueue(new Vector3Int(targetQ, targetR, 0));
            while (queue.Count > 0)
            {
                Vector3Int curr = queue.Dequeue();
                boardState.AddNode(curr[0], curr[1], this.player, stats.currLevel.bombStrength / (1 + curr[2]));
                if (curr[2] < stats.currLevel.bombRadius)
                {
                    List<Vector2Int> neighbors = boardState.GetNeighbors(curr[0], curr[1]);
                    foreach (var n in neighbors)
                    {
                        if (!visited.Contains(n))
                        {
                            visited.Add(n);
                            queue.Enqueue(new Vector3Int(n[0], n[1], curr[2] + 1));
                        }
                    }
                }
            }
        }

        public override void Upgrade()
        {
            stats.Upgrade();
            hp = stats.currLevel.maxHP;
            maxhp = stats.currLevel.maxHP;
            regen = stats.currLevel.regen;
            armor = stats.currLevel.armor;
        }

        public override void Destroy()
        {
            Destroy(gameObject);
        }
    }
}