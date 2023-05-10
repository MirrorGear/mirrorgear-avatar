using System.Runtime.InteropServices;
using AOT;
using System;
using UnityEngine;

namespace MirrorGearSDK
{
    internal static partial class AvatarCreationNative
    {
#if UNITY_IOS || UNITY_TVOS || UNITY_WEBGL
    private const string Import = "__Internal";
#else
        private const string Import = "MirrorGearSDKAvatar_unity";
#endif
        [DllImport(Import)]
        public static extern IntPtr avatarCreation(string cms_url_);

        [DllImport(Import)]
        public static extern void generateAvatar(IntPtr obj, int avatar_id, string image_file_path, string gender, [MarshalAs(UnmanagedType.FunctionPtr)] Avatar.AvatarCreation.GenerateAvatarHandler func);

        [DllImport(Import)]
        public static extern void deleteAvatarCreation(IntPtr obj);
    }
}
