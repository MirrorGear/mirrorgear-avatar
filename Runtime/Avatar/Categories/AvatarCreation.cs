using System.Collections;
using System.Collections.Generic;
using System;
using AOT;
using UnityEngine;

namespace MirrorGearSDK
{
    namespace Avatar
    {
        public class AvatarCreation
        {
            // intptr 객체를 여기서 관리
            private IntPtr creation_handler = IntPtr.Zero;
            
            public delegate void GenerateAvatarHandler(string avatar_id, AvatarResponse response);
            private static Dictionary<int, GenerateAvatarHandler> eventDict = new Dictionary<int, GenerateAvatarHandler>();

            [MonoPInvokeCallback(typeof(GenerateAvatarHandler))]
            private static void OnReceiveGenerateAvatar(string avatar_id, AvatarResponse response)
            {
                int id = int.Parse(avatar_id);

                if(eventDict.ContainsKey(id) && eventDict[id] != null)
                {
                    eventDict[id](avatar_id, response);
                    RemoveGenerateAvatarEvent(id);
                }
            }

            // 생성자 생성
            public AvatarCreation(string cms_url_)
            {
                MirrorGearSDK.Utils.Logger.SetLogger(MirrorGearSDK.Utils.Logger.DefaultLogHandler);

                if(creation_handler != IntPtr.Zero)
                {
                    DeleteAvatarCreation();
                }
                creation_handler =  AvatarCreationNative.avatarCreation(cms_url_);
            }

            public void GenerateAvatar(int avatar_id, string image_file_path, string gender, GenerateAvatarHandler callback)
            {
                AddGenerateAvatarEvent(avatar_id, callback);
                AvatarCreationNative.generateAvatar(creation_handler, avatar_id, image_file_path, gender, OnReceiveGenerateAvatar);
            }

            public void DeleteAvatarCreation()
            {
                AvatarCreationNative.deleteAvatarCreation(creation_handler);
            }

            private static void AddGenerateAvatarEvent(int avatar_id, GenerateAvatarHandler callback)
            {
                if(eventDict.ContainsKey(avatar_id)){
                    eventDict[avatar_id] += callback;
                }else{
                    eventDict[avatar_id] = null;
                    eventDict[avatar_id] += callback;
                }
            }

            private static void RemoveGenerateAvatarEvent(int avatar_id)
            {
                if(eventDict.ContainsKey(avatar_id))
                {
                    eventDict[avatar_id] = null;
                }
            }
        }
    }
}