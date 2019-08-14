using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GeometryBattles.HexAction;
using GeometryBattles.BoardManager;
using GeometryBattles.PlayerManager;
using UnityEngine.EventSystems;

namespace GeometryBattles.UI
{
    [RequireComponent(typeof(Tile))]
    public class HexActionMenu : MonoBehaviour, IPointerClickHandler, IPointerDownHandler
    {
        //Cached References
        Tile tilePrefab;
        UIManager uiManager;
        HexSelectionManager hexSelectionManager;
        
        // Start is called before the first frame update
        void Start()
        {
            uiManager = FindObjectOfType<UIManager>();
            hexSelectionManager = uiManager.HexSelectionManager;
            tilePrefab = GetComponent<Tile>();
        }

        private void HandleClick()
        {
            string errMsg = "";
            var actions = HexActionManager.getViableActions
            (uiManager.GetActivePlayer(), tilePrefab);

            Debug.Log($"I clicked as {uiManager.GetActivePlayer().Id}");

            if (actions.Count == 1)
            {
                if (actions[0].canDoAction(uiManager.GetActivePlayer(), tilePrefab, ref errMsg))
                {
                    actions[0].doAction(uiManager.GetActivePlayer(), tilePrefab);
                }
                else
                {
                    Debug.Log(errMsg);
                }
            }
            else if (actions.Count > 1)
            {
                hexSelectionManager.InitializeHexActionMenu();
                foreach (IHexAction hexAction in actions)
                {
                    Button button = hexSelectionManager.AddActionToHexActionMenu(hexAction.displayName + "  " + hexAction.GetTipText(uiManager.GetActivePlayer(), tilePrefab));
                    button.onClick.AddListener(delegate
                    {
                        hexAction.doAction(uiManager.GetActivePlayer(), tilePrefab);
                        hexSelectionManager.HideHexActionMenu();
                    });
                    if (!hexAction.canDoAction(uiManager.GetActivePlayer(), tilePrefab))
                    {
                        button.interactable = false;
                    }
                }
                hexSelectionManager.ShowHexActionMenu(tilePrefab);
            }
        }

        private void OnMouseOver()
        {
            hexSelectionManager.MouseEnteredTile(tilePrefab);
            
            var actions = HexActionManager.getViableActions(uiManager.GetActivePlayer(), tilePrefab);
            if (actions.Count == 1)
            {            
                GetComponentInChildren<TextMesh>().text = actions[0].GetTipText(uiManager.GetActivePlayer(), tilePrefab);
            }
            else
            {
                GetComponentInChildren<TextMesh>().text = "";
            }
        }

        private void OnMouseExit()
        {
            hexSelectionManager.MouseExitedTile(tilePrefab);
            
            GetComponentInChildren<TextMesh>().text = "";
        }

        private void DebugLogActions(List<IHexAction> actions)
        {
            string errMsg = "";
            foreach (IHexAction action in actions)
            {
                Debug.Log(action.displayName);
                if (!action.canDoAction(uiManager.GetActivePlayer(), tilePrefab, ref errMsg))
                {
                    Debug.Log(errMsg);
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                HandleClick();
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            hexSelectionManager.HideHexActionMenu();
        }
    }
}