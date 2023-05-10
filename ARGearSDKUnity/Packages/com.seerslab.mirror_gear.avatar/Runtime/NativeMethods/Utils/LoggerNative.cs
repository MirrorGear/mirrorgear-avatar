using UnityEngine;
using System;
using System.Runtime.InteropServices;

namespace MirrorGearSDK
{
    internal static partial class NativeMethods
    {
#if UNITY_IOS || UNITY_TVOS || UNITY_WEBGL
            public const string Import = "__Internal";
#else
        public const string Import = "MirrorGearSDKAvatar_unity";
#endif

        [DllImport(Import)]
        public static extern void SetLogger(Utils.Logger.LogHandler handler);
    }
}