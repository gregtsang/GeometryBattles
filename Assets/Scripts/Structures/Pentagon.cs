using System.Collections;
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

        public Material pentagonMat;
        MeshRenderer pentagonRenderer;
        float rotateSpeed = 1.0f;
        Coroutine rotate = null;

        void OnEnable()
        {
            stats = this.gameObject.GetComponent<PentagonData>();
            pentagonRenderer = gameObject.GetComponent<MeshRenderer>();
        }

        void Start()
        {
            hp = stats.currLevel.maxHP;
            maxhp = stats.currLevel.maxHP;
            regen = stats.currLevel.regen;
            armor = stats.currLevel.armor;
        }

        public override void StartEffect()
        {
            //StartCoroutine(RegenHP());
            EventManager.RaiseOnCreatePentagon(gameObject);
            pentagonMat.SetColor("_BaseColor", boardState.GetNodeOwner(q, r).GetColor());
            Material[] materials = pentagonRenderer.materials;
            for (int i = 0; i < 5; i++)
            {
                materials[i] = pentagonMat;
            }
            pentagonRenderer.materials = materials;
        }

        public bool CheckSpace()
        {
            return this.CheckSpace(this.q, this.r, this.player);
        }

        public override bool CheckSpace(int q, int r, Player player)
        {
            List<Vector2Int> tiles = new List<Vector2Int>();
            tiles.Add(new Vector2Int(q, r));
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

        public override void SetColor(Color color)
        {
            foreach (Material m in gameObject.GetComponent<MeshRenderer>().materials)
            {
                m.SetColor("_BaseColor", color);
            }
        }

        public int GetBombRadius()
        {
            return stats.currLevel.bombRadius;
        }

        public float GetBombRate()
        {
            return stats.currLevel.bombRate;
        }

        public int GetBombStrength()
        {
            return stats.currLevel.bombStrength;
        }

        public Vector2Int GetTarget()
        {
            return new Vector2Int(targetQ, targetR);
        }

        public bool HasTarget()
        {
            return target;
        }

        public void SetTarget(int q, int r)
        {
            targetQ = q;
            targetR = r;
            target = true;
        }

        public bool IsGlowing()
        {
            return rotate != null;
        }

        public void StartGlow()
        {
            rotate = StartCoroutine(Glow());
        }

        public void StopGlow()
        {
            StopCoroutine(rotate);
            StartCoroutine(ShutDown());
            rotate = null;
        }

        IEnumerator Glow()
        {
            int i = 0;
            Color color = boardState.GetNodeOwner(q, r).GetColor();
            while (true)
            {
                float lerp = 0.0f;
                while (lerp <= 1.0f)
                {
                    lerp += Time.deltaTime * rotateSpeed;
                    pentagonRenderer.materials[i].SetColor("_BaseColor", Color.Lerp(color * Mathf.Clamp(rotateSpeed, 1.0f, 3.0f), color * 3 * Mathf.Clamp(rotateSpeed, 1.0f, 5.0f), Mathf.Min(1.0f, lerp)));
                    yield return null;
                }
                lerp = 1.0f;
                while (lerp >= 0.0f)
                {
                    lerp -= Time.deltaTime * rotateSpeed;
                    pentagonRenderer.materials[i].SetColor("_BaseColor", Color.Lerp(color * Mathf.Clamp(rotateSpeed, 1.0f, 3.0f), color * 3 * Mathf.Clamp(rotateSpeed, 1.0f, 5.0f), Mathf.Max(0.0f, lerp)));
                    yield return null;
                }
                i = (i + 1) % 5;
                rotateSpeed = Mathf.Min(15.0f, rotateSpeed + 1.0f);
            }
        }

        IEnumerator ShutDown()
        {
            float lerp = 0.0f;
            Color[] colors = new Color[5];
            Color color = boardState.GetNodeOwner(q, r).GetColor();
            for (int i = 0; i < 5; i++)
            {
                colors[i] = pentagonRenderer.materials[i].GetColor("_BaseColor");
            }
            while (lerp < 1.0f)
            {
                lerp += Time.deltaTime * 2.0f;
                for (int i = 0; i < 5; i++)
                {
                    pentagonRenderer.materials[i].SetColor("_BaseColor", Color.Lerp(colors[i], color, lerp));
                }
                yield return null;
            }
        }

        public void Reset()
        {
            rotateSpeed = 1.0f;
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