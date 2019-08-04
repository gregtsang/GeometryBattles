using UnityEngine;

public class RoomsGUI : MonoBehaviour {

   [SerializeField]
   private CreateOrJoinRoomCanvas _createOrJoinRoomCanvas = null;
   public CreateOrJoinRoomCanvas CreateOrJoinRoomCanvas { get { return _createOrJoinRoomCanvas; } }

   [SerializeField]
   private CurrentRoomCanvas _currentRoomCanvas = null;
   public CurrentRoomCanvas CurrentRoomCanvas { get { return _currentRoomCanvas; } }

   private void Awake() {
      FirstInitialize();
   }

   private void FirstInitialize() {
      CreateOrJoinRoomCanvas.FirstInitialize(this);
      CurrentRoomCanvas.FirstInitialize(this);
   }
}
