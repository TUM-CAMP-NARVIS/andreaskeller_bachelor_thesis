﻿using UnityEngine.VR;

public class Utils
{
    private static PlayerType playerType = PlayerType.Undetermined;

    public enum PlayerType
    {
        Undetermined,
        Unknown,
        HoloLens,
        VR,
        IOS,
    }

    public static PlayerType CurrentPlayerType
    {
        get
        {
            if (playerType == PlayerType.Undetermined)
            {
                switch (UnityEngine.XR.XRSettings.loadedDeviceName)
                {
                    case "HoloLens":
                        playerType = PlayerType.HoloLens;
                        break;
                    case "OpenVR":
                        playerType = PlayerType.VR;
                        break;
#if UNITY_IOS
                    default:
                        playerType = PlayerType.IOS;
                        break;
#elif UNITY_WSA
                    default:
                        playerType = PlayerType.HoloLens;
                        break;
#else
                    default:
                        playerType = PlayerType.VR;
                        break;
#endif

                }
            }

            return playerType;
        }
    }

    public static bool IsHoloLens
    {
        get
        {
            return CurrentPlayerType == PlayerType.HoloLens;
        }
    }

    public static bool IsVR
    {
        get
        {
            return CurrentPlayerType == PlayerType.VR;
        }
    }

    public static bool IsIOS
    {
        get
        {
            return CurrentPlayerType == PlayerType.IOS;
        }
    }
}
