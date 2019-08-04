using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof (Chat))]
public class NamePicker : MonoBehaviour
{
    private string         photon_username = "photon_username";
    private Chat           chat            = null;
    public  TMP_InputField username_input  = null;
    
    private void Start()
    {
            // Cache Chat component 
        chat = GetComponent<Chat>();

            /* If the client has previously saved the username, load it into the 
               TMP_InputField's text field
            */
        string username = PlayerPrefs.GetString(photon_username);
        if (!(username is null) && username.Length != 0)
        {
            username_input.text = username;
        }
    }

    public void StartChat()
    {
            // Remove username whitespace padding and connect to chat
        chat.username = username_input.text.Trim();
        chat.Connect();

        Debug.Log($"You picked {chat.username} for your username. Wise choice.");

            // Stop calling updates on NamePicker
        enabled = false;

            // Update saved username to match what was entered in the GUI
        PlayerPrefs.SetString(photon_username, chat.username);
    }
}
