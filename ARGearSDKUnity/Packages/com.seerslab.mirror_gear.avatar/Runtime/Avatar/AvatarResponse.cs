using UnityEngine;
using System.Runtime.InteropServices;

namespace MirrorGearSDK
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct AvatarResponse
    {
        [MarshalAs(UnmanagedType.LPStr)] public string json;
        [MarshalAs(UnmanagedType.LPStr)] public string error_msg;
    }
}
