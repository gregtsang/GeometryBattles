using System.Collections;
using UnityEngine;
using GeometryBattles.BoardManager;

namespace GeometryBattles.StructureManager
{
    public class CubeManager : MonoBehaviour
    {
        public BoardState boardState;
        public ScoutManager scoutManager;

        void OnEnable()
        {
            boardState = GameObject.FindObjectOfType<BoardState>();
            EventManager.onCreateCube += AddCube;
        }

        public void AddCube(GameObject cube)
        {
            StartCoroutine(SpawnScout(cube.GetComponent<Cube>()));
        }
        
        IEnumerator SpawnScout(Cube cube)
        {
            yield return new WaitForSeconds(cube.GetSpawnRate());
            while (true)
            {
                RPC_SpawnScout(cube);
                yield return new WaitForSeconds(cube.GetSpawnRate());
            }
        }

        void RPC_SpawnScout(Cube cube)
        {
            cube.SpawnScout();
            scoutManager.SpawnScout(cube, boardState.GetNodeOwner(cube.Q, cube.R).GetColor());
        }
    }
}