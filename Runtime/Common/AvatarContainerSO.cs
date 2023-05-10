using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MirrorGearSDK
{
    [CreateAssetMenu(fileName = "AvatarContainer", menuName = "ScriptableObjects/AvatarContainer", order = 1)]
    public class AvatarContainerSO : ScriptableObject
    {
        public AvatarData[] Avatars;
    }

    [Serializable]
    public class AvatarData
    {
        public GameObject Model;
        public Vector3 PositionOffset;
        public Vector3 RotationOffset;
        public GameObject[] Hairs;
        public GameObject[] Glasses;
        public GameObject[] Cloths;
    }
}
