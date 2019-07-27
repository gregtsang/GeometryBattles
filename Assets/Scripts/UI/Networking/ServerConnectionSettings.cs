using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace GeometryBattles.Networking
{
    static public class ServerConnectionSettings
    {
        static public void SetServerConnectionSettings()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.NickName = MasterManager.GameSettings.nickname;
            PhotonNetwork.GameVersion = MasterManager.GameSettings.GameVersion;
        }

        static public void UpdateNickName(string nickname)
        {
            MasterManager.GameSettings.nickname = nickname;
        }

        static public void UpdateRegion(string regionCode)
        {
            PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = regionCode;
        }
    }
}