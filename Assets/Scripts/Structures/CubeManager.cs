using System.Collections;
using UnityEngine;
using GeometryBattles.BoardManager;
using Photon.Pun;

namespace GeometryBattles.StructureManager
{
    public class CubeManager : MonoBehaviour
    {
        public BoardState boardState;
        public StructureStore structureStore;
        public ScoutManager scoutManager;
        public PhotonView photonView;

        void OnEnable()
        {
            EventManager.onCreateCube += AddCube;
        }

        public void AddCube(GameObject cube)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(SpawnScout(cube.GetComponent<Cube>()));
            }
        }
        
        IEnumerator SpawnScout(Cube cube)
        {
            yield return new WaitForSeconds(cube.GetSpawnRate());
            while (true)
            {
                photonView.RPC("RPC_SpawnScout", RpcTarget.AllViaServer, cube.Q, cube.R);
                yield return new WaitForSeconds(cube.GetSpawnRate());
            }
        }

        [PunRPC]
        void RPC_SpawnScout(int q, int r)
        {
            Cube cube = (Cube)(structureStore.GetStructure(q, r));
            cube.SpawnScout();
            scoutManager.SpawnScout(cube, boardState.GetNodeOwner(cube.Q, cube.R).GetColor());
        }
    }
}