using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.Events;

using MiniJSON;

public class AvatarController : MonoBehaviour
{
    [SerializeField]
    private int avatarID;
    [SerializeField]
    private string cmsUrl;
    [SerializeField]
    private Texture2D image;
    [SerializeField]
    private string imageFileName;
    [SerializeField]
    private string gender;
    [SerializeField]
    private MirrorGearSDK.AvatarContainerSO avatarContainerSO;

    private GameObject[] avatarPool;
    private SkinnedMeshRenderer[] faceBlendShapeMeshes;
    private Dictionary<string, int>[] blendshapeInfo;
    private Dictionary<string, Gender> genderInfo;

    public GameObject Target;

    MirrorGearSDK.Avatar.AvatarCreation avatar;

    private Queue<UnityAction> actionBuffer = new Queue<UnityAction>();

    Gender currentGender;

    // 아바타 오브젝트 관리
    private GameObject currentHair;
    private GameObject currentGlasses;

    // 아바타 애니메이션 관리
   private AvatarAnimation movement;

    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponent<AvatarAnimation>();
        InstantiateAvatars();

        // 아바타 객체 만들기
        avatar = new MirrorGearSDK.Avatar.AvatarCreation(cmsUrl);

        string path = Path.Combine(Application.persistentDataPath, imageFileName);
        byte[] bytes = image.EncodeToPNG();
        File.WriteAllBytes(path, bytes);

        genderInfo = new Dictionary<string, Gender>();
        genderInfo["female"] = Gender.female;
        genderInfo["male"] = Gender.male;

        currentGender = genderInfo[gender];
        
        Target = avatarPool[(int)currentGender];
        Target.SetActive(true);
        movement.SetCharacter(Target);
    }

    public void GenerateAvatar()
    {
        string filePath = Path.Combine(Application.persistentDataPath, imageFileName);
        avatar.GenerateAvatar(avatarID, filePath, gender, AvatarCallback);
    }

    public void AvatarCallback(string avatarID, MirrorGearSDK.AvatarResponse response)
    {
        Debug.Log("response json : " + response.json);
        Debug.Log("response error_msg : " + response.error_msg);
        if (String.IsNullOrEmpty(response.json))
            return;

        actionBuffer.Enqueue(delegate { SettingAvatar(response.json); });
    }

    private void SettingAvatar(string json)
    {
        var dict = Json.Deserialize(json) as Dictionary<string, object>;
        var dataJson = Json.Deserialize(Json.Serialize((object)dict["data"])) as Dictionary<string, object>;
        var avatarJson = Json.Deserialize(Json.Serialize((object)dataJson["Avatar"])) as Dictionary<string, object>;
        var accessoriesJson = Json.Deserialize(Json.Serialize((object)avatarJson["Accessories"])) as Dictionary<string, object>;
        var colorJson = Json.Deserialize(Json.Serialize((object)avatarJson["Color"])) as Dictionary<string, object>;

        SetAvatarModel(avatarJson);
        SetHair(accessoriesJson, colorJson);
        SetSkin(colorJson);
        SetGlasses(accessoriesJson);
        SetBlendShape(avatarJson);

        // 아바타 아바타 움직이게 설정하기
        movement.SetCharacter(Target);
    }

    public void SetAvatarModel(Dictionary<string, object> avatarJson)
    {
        // 성별 파싱
        string gender = (string)avatarJson["Gender"];

        // 성별에 따른 아바타 설정.
        currentGender = genderInfo[gender];
        Target.SetActive(false); //기존 활성화된 것 비활성화
        Target = avatarPool[(int)currentGender];
        Target.SetActive(true);
    }

    private void SetHair(Dictionary<string, object> accessoriesJson, Dictionary<string, object> colorJson)
    {
        // 헤어 스타일 파싱
        var hairJson = Json.Deserialize(Json.Serialize((object)accessoriesJson["hair"])) as Dictionary<string, object>;
        var hair_style = (long)hairJson["style"];

        // 헤어 생성
        if (currentHair != null) //헤어가 존재할 경우 헤어 제거
        {
            Destroy(currentHair);
        }
        currentHair = Instantiate(avatarContainerSO.Avatars[(int)currentGender].Hairs[hair_style], avatarPool[(int)currentGender].transform);
        currentHair.transform.localPosition = Vector3.zero; // 자식 오브젝트의 위치를 상위 오브젝트와 일치시킵니다.
        currentHair.transform.localRotation = Quaternion.identity; // 자식 오브젝트의 회전을 상위 오브젝트와 일치시킵니다.
        currentHair.transform.localScale = Vector3.one; // 자식 오브젝트의 스케일을 상위 오브젝트와 일치시킵니다.
        
        // 헤어 색상 파싱
        var hairColor = JsonUtility.FromJson<CustomColor>(Json.Serialize((object)colorJson["hair"]));

        // 헤어 색상 변경
        var hairMeshes = currentHair.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var h in hairMeshes)
        {
            if (h.name.Contains("Hair") || h.name.Contains("hair") || h.name.Contains("Hiar"))
            {
                h.material.color = new Color32((byte)hairColor.R, (byte)hairColor.G, (byte)hairColor.B, 255);
            }
        }
    }

    private void SetSkin(Dictionary<string, object> colorJson)
    {
        // 피부색 파싱
        var skinColor = JsonUtility.FromJson<CustomColor>(Json.Serialize((object)colorJson["skin"]));

        // 피부색 변경
        var skinColor32 = new Color32((byte)skinColor.R, (byte)skinColor.G, (byte)skinColor.B, 255);
        var skinMeshes = Target.GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (var h in skinMeshes)
        {
            if (currentGender == Gender.female)
            {
                if (h.name.Contains("Base") || h.name.Contains("Face") || h.name.Contains("body"))
                {
                    h.material.color = skinColor32;
                    if (h.materials.Length != 1)
                    {
                        h.materials[2].color = skinColor32;
                    }
                }
            }
            else
            {
                if (h.name.Contains("body") || h.name.Contains("Face"))
                {
                    h.material.color = skinColor32;
                }
            }
        }
    }

    private void SetGlasses(Dictionary<string, object> accessoriesJson)
    {
        // 안경 착용 여부 파싱
        var glasses = (long)accessoriesJson["glasses"];
        
        // 안경 착용
        if (currentGlasses != null) Destroy(currentGlasses);

        GameObject glassesItem = avatarContainerSO.Avatars[(int)currentGender].Glasses[glasses];
        if (glassesItem != null)
        {
            currentGlasses = Instantiate(glassesItem, avatarPool[(int)currentGender].transform);
            if (currentGender == Gender.female)
            {
                currentGlasses.transform.localPosition = new Vector3(0f, -0.04f, 0.01f);
            }
        }
    }

    // 아바타 전부 생성해서 비활성화 시켜둠
    private void InstantiateAvatars()
    {
        var length = avatarContainerSO.Avatars.Length;
        avatarPool = new GameObject[length];
        faceBlendShapeMeshes = new SkinnedMeshRenderer[length];
        blendshapeInfo = new Dictionary<string, int>[length];


        var parentObj = new GameObject("---- Avatar ----");
        for (int i = 0; i < avatarContainerSO.Avatars.Length; i++)
        {
            blendshapeInfo[i] = new Dictionary<string, int>();

            avatarPool[i] = Instantiate(avatarContainerSO.Avatars[i].Model, parentObj.transform);
            avatarPool[i].transform.localPosition = avatarContainerSO.Avatars[i].PositionOffset;
            avatarPool[i].transform.localRotation = Quaternion.Euler(avatarContainerSO.Avatars[i].RotationOffset);
            avatarPool[i].SetActive(false);

            Target = avatarPool[i];

            var skinnedMeshes = avatarPool[i].GetComponentsInChildren<SkinnedMeshRenderer>();

            foreach (var mesh in skinnedMeshes)
            {
                if (mesh.name.Contains("Face"))
                {
                    faceBlendShapeMeshes[i] = mesh;
                    for (int j = 0; j < mesh.sharedMesh.blendShapeCount; j++)
                    {
                        string name = mesh.sharedMesh.GetBlendShapeName(j);
                        blendshapeInfo[i].Add(name, j);
                    }
                }
            }
            movement.InitCharacter(Target);
        }
    }

    private void SetBlendShape(Dictionary<string, object> avatarJson)
    {
        var BlendShapeJson = Json.Deserialize(Json.Serialize((object)avatarJson["BlendShape"])) as Dictionary<string, object>;
        foreach (KeyValuePair<string, object> item in BlendShapeJson)
        {
            float value = float.Parse((item.Value).ToString());
            SetBlendShape(item.Key, value * 100);
        }
    }

    void SetBlendShape(string name, float value)
    {
        if (!blendshapeInfo[(int)currentGender].ContainsKey(name))
            return;

        int index = blendshapeInfo[(int)currentGender][name];
        faceBlendShapeMeshes[(int)currentGender].SetBlendShapeWeight(index, value);
    }

    void OnDestory()
    {
        avatar.DeleteAvatarCreation();
    }

    void Update()
    {
        if (actionBuffer.Count > 0)
        {
            actionBuffer.Dequeue().Invoke();
        }
    }
}
