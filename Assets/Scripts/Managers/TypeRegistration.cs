using ExitGames.Client.Photon;
using UnityEngine;

public class TypeRegistration : MonoBehaviour
{
   public static byte SERIALIZABLE_VECTOR2INT_CODE = 0;

   public TypeRegistration()
   {
      PhotonPeer.RegisterType(
         typeof(SerializableVector2Int),
         SERIALIZABLE_VECTOR2INT_CODE,
         SerializableVector2Int.Serialize,
         SerializableVector2Int.Deserialize);
   }
}
