using System.Collections;
using UnityEngine;

namespace GeometryBattles.StructureManager
{
    public class Cube : Structure
    {
        public GameObject scouts;
        public Material cubeMat;
        public GameObject cubeScoutPrefab;

        public CubeData stats;
        bool spawn = false;

        void OnEnable()
        {
            stats = this.gameObject.GetComponent<CubeData>();
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
            StartCoroutine(RegenHP());
            cubeMat.SetColor("_BaseColor", boardState.GetNodeOwner(q, r).GetColor());
            cubeMat.SetFloat("_Level", 0.0f);
            gameObject.GetComponent<MeshRenderer>().material = cubeMat;
            EventManager.RaiseOnCreateCube(gameObject);
            StartCoroutine(Spawn());
        }

        public float GetSpawnRate()
        {
            return stats.currLevel.spawnRate;
        }

        public float GetMoveRate()
        {
            return stats.currLevel.moveRate;
        }

        public int GetNumMoves()
        {
            return stats.currLevel.numMoves;
        }

        IEnumerator Spawn()
        {
            Transform transform = gameObject.transform;
            float x = transform.position.x;
            float z = transform.position.z;
            Material mat = gameObject.GetComponent<MeshRenderer>().material;
            Color color = boardState.GetNodeOwner(q, r).GetColor();
            while (true)
            {
                float timer = stats.currLevel.spawnRate - 3.5f;
                while (timer > 0.0f)
                {
                    timer -= Time.deltaTime;
                    float intensity = 1.0f -  Mathf.Max(0.0f, timer) / (stats.currLevel.spawnRate - 3.5f);
                    float translateX = transform.position.x - x >= 0 ? Random.Range(-0.1f, 0.0f) : Random.Range(0.0f, 0.1f);
                    float translateZ = transform.position.z - z >= 0 ? Random.Range(-0.1f, 0.0f) : Random.Range(0.0f, 0.1f);    
                    mat.SetColor("_BaseColor", Color.Lerp(color, color * 18, intensity));
                    transform.Translate(intensity * translateX, 2.0f * Time.deltaTime / (stats.currLevel.spawnRate - 3.5f), intensity * translateZ);
                    yield return null;
                }
                spawn = false;
                while (!spawn)
                {
                    float translateX = transform.position.x - x >= 0 ? Random.Range(-0.1f, 0.0f) : Random.Range(0.0f, 0.1f);
                    float translateZ = transform.position.z - z >= 0 ? Random.Range(-0.1f, 0.0f) : Random.Range(0.0f, 0.1f);
                    transform.Translate(translateX, 0.0f, translateZ);
                    yield return null;
                }
                //GameObject scout = SpawnScout(color);
                //Material scoutMat = scout.GetComponent<MeshRenderer>().material;
                //bool scoutStart = false;
                timer = 1.0f;
                while (timer >= -0.5f)
                {
                    timer -= Time.deltaTime;
                    transform.Rotate(Vector3.up, Time.deltaTime * 180.0f * (Mathf.Max(0.0f, timer) + 0.1f));
                    mat.SetFloat("_Level", 1.0f - Mathf.Pow(Mathf.Max(0.0f, timer), 2.0f));
                    yield return null;
                /*  if (timer >= 0.0f)
                    {
                        float scoutY = scout.gameObject.transform.position.y;
                        scout.transform.position = new Vector3(x, (scoutY - 0.25f) * Mathf.Max(0.0f, timer / 1.0f) + 0.25f, z);
                        scoutMat.SetColor("_BaseColor", Color.Lerp(color * 5, color, 1.0f - Mathf.Max(0.0f, timer)));
                    }
                    else
                    {
                        if (!scoutStart)
                        {
                            EventManager.RaiseOnCreateScout(scout);
                            scoutStart = true;
                        }
                    }*/
                    
                }
                timer = 0.5f;
                while (timer > 0.0f)
                {
                    timer -= Time.deltaTime;
                    transform.Rotate(Vector3.up, Time.deltaTime * (18.0f + (1.0f - timer / 0.5f) * 180.0f));
                    mat.SetFloat("_Level", Mathf.Pow(Mathf.Max(0.0f, timer) / 0.5f, 0.5f));
                    mat.SetColor("_BaseColor", Color.Lerp(color, color * 15, Mathf.Max(0.0f, timer)));
                    yield return null;
                }
                timer = 0.35f;
                while (timer > 0.0f)
                {
                    timer -= Time.deltaTime;
                    Quaternion currRot = transform.rotation;
                    float yRot = transform.eulerAngles.y;
                    Quaternion targetRot = Quaternion.AngleAxis(225.0f - yRot, Vector3.up);
                    Quaternion newRot = targetRot * currRot;
                    transform.rotation = Quaternion.Lerp(currRot, newRot, 1.0f - Mathf.Max(timer, 0.0f) / 0.5f);
                    float yPos = transform.position.y;
                    transform.position = new Vector3(x, (yPos - 0.5f) * Mathf.Pow(Mathf.Max(0.0f, timer / 0.5f), 0.2f) + 0.5f, z);
                    yield return null;
                }
                transform.eulerAngles = new Vector3(0.0f, 45.0f, 0.0f);
                yield return new WaitForSeconds(0.5f);
            }
        }

        public void SpawnScout()
        {
            spawn = true;
        }

        public GameObject SpawnScout(Color color)
        {
            GameObject scout = Instantiate(cubeScoutPrefab, this.transform.position - new Vector3(0.0f, 0.25f, 0.0f), cubeScoutPrefab.transform.rotation, scouts.transform) as GameObject;
            scout.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", color * 5);
            CubeScout currScout = scout.GetComponent<CubeScout>();
            currScout.SetHome(this.q, this.r);
            currScout.SetCoords(this.q, this.r);
            currScout.SetPlayer(this.player);
            currScout.SetMoveRate(stats.currLevel.moveRate);
            currScout.SetMoves(stats.currLevel.numMoves);
            currScout.AddVisited(this.q, this.r);
            return scout;
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