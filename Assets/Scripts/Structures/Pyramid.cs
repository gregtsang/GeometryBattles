using System.Collections;
using UnityEngine;

namespace GeometryBattles.StructureManager
{
    public class Pyramid : Structure
    {
        PyramidData stats;
        public GameObject pyramidBase;
        public GameObject pyramidTop;
        public MeshRenderer baseRenderer;
        public MeshRenderer topRenderer;
        public Material pyramidMat;

        void OnEnable()
        {
            stats = this.gameObject.GetComponent<PyramidData>();
            pyramidBase = gameObject.transform.GetChild(0).gameObject;
            pyramidTop = gameObject.transform.GetChild(1).gameObject;
            baseRenderer = pyramidBase.GetComponent<MeshRenderer>();
            topRenderer = pyramidTop.GetComponent<MeshRenderer>();
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
            // TODO RegenHP needs to be handled by a system calling a RPC
            //StartCoroutine(RegenHP());
            pyramidMat.SetColor("_BaseColor", boardState.GetNodeOwner(q, r).GetColor());
            topRenderer.material = pyramidMat;
            Material[] materials = baseRenderer.materials;
            materials[0] = pyramidMat;
            materials[1] = pyramidMat;
            baseRenderer.materials = materials;
            baseRenderer.materials[1].SetColor("_BaseColor", boardState.GetNodeOwner(q, r).GetColor() * 20);
            StartCoroutine(Animate());
            Buff(stats.currLevel.range, stats.currLevel.strength);
        }

        IEnumerator Animate()
        {
            Color color = boardState.GetNodeOwner(q, r).GetColor();
            Vector3 pos = pyramidTop.transform.position;
            float timer = 0.0f;
            while (true)
            {
                if (timer >= 2.0f * Mathf.PI)
                {
                    timer = 0.0f;
                }
                if (timer == 0.0f)
                {
                    //
                }
                float sintime = (Mathf.Sin(timer + 1.5f * Mathf.PI) + 1.0f) / 2.0f;
                timer += Time.deltaTime;
                pyramidTop.transform.position = pos + new Vector3(0.0f, sintime / 6.0f, 0.0f);
                topRenderer.material.SetColor("_BaseColor", Color.Lerp(color, color * 18, sintime));
                pyramidTop.transform.Rotate(Vector3.up, 45.0f * Time.deltaTime, Space.World);
                yield return null;
            }
        }

        public override void SetColor(Color color)
        {
            baseRenderer.materials[0].SetColor("_BaseColor", color);
            baseRenderer.materials[1].SetColor("_BaseColor", color);
            topRenderer.material.SetColor("_BaseColor", color);
        }

        public void Buff(int range, int strength)
        {
            for (int i = -range; i <= range; i++)
            {
                for (int j = Mathf.Max(-range, -range - i); j <= Mathf.Min(range, range - i); j++)
                {
                    if (boardState.IsValidTile(this.q + i, this.r + j))
                        boardState.SetNodeBuff(this.q + i, this.r + j, boardState.GetNodeOwner(this.q, this.r), strength);
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
            Buff(stats.currLevel.range, stats.currLevel.strength);
        }

        public override void Destroy()
        {
            Buff(stats.currLevel.range, 0);
            Destroy(gameObject);
        }
    }
}