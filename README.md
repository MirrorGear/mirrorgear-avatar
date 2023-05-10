# MirrorGearSDK Avatar Unity SDK
## 프로젝트 소개
MirrorGearSDK Avatar는 Unity 엔진을 사용하여 아바타 생성을 위한 패키지를 제공합니다.

### 아바타 생성
이 패키지를 사용하면 Unity 엔진을 사용하여 사진을 기반으로 쉽게 아바타를 만들 수 있습니다.
아바타 생성 과정은 다음과 같습니다.

1. CMS 서버에 사진과 정보를 요청합니다.
2. 서버는 아바타 정보를 JSON 형태로 반환합니다.
3. Unity 엔진에서 해당 JSON 값을 파싱하여 아바타 생성에 필요한 정보로 변환합니다.
4. 아바타 정보와 일치하도록 적절한 에셋을 적용합니다.
5. 최종 아바타를 렌더링합니다.

위와 같은 과정으로 아바타를 만들 수 있습니다.

## 시작하기 전에
아바타 요청에 사용하는 cmsURL은 별도로 얻어야 합니다. 올바른 URL을 얻으려면 '시어스랩'에 문의하시기 바랍니다.

## 사용 방법
시작하려면 다음 단계를 따르세요.
``` c#
MirrorGearSDK.Avatar.AvatarCreation avatar;
avatar = new MirrorGearSDK.Avatar.AvatarCreation(cmsUrl);
```
`MirrorGearSDK.Avatar.AvatarCreation`은 아바타 생성 클래스입니다. `cmsUrl` 매개변수는 CMS 서버의 URL을 전달합니다. 이전에 설명한 것과 같이 CMS서버는 아바타 생성하기 위한 정보를 제공합니다.

``` c#
avatar.GenerateAvatar(avatarID, filePath, gender, AvatarCallback);
```
`avatar.GenerateAvatar`는 CMS 서버에서 아바타를 생성하는 함수입니다. 이 함수는 다음 매개 변수를 전달합니다.

- `avatarID` : 생성하려는 아바타의 ID입니다. `avatarID`는 `AvatarCallback`에서 생성된 아바타를 식별하는 용도로 사용됩니다.
- `filePath` : 아바타 생성하기 위해 사용되는 이미지 파일의 경로입니다.
- `gender` : 아바타의 성별을 나타내는 값입니다. 이 값은 "male"과 "female" 중 하나입니다.
- `AvatarCallback` : 아바타 생성 요청에 대한 응답 정보를 전달하는 콜백 함수입니다.

``` c#
public void AvatarCallback(string avatarID, MirrorGearSDK.AvatarResponse response)
{
    Debug.Log("response json : " + response.json);
    Debug.Log("response error_msg : " + response.error_msg);

    // json 정보를 파싱하여 필요한 정보를 추출하여 아바타를 생성.
}
```
`AvatarCallback` 함수는 `avatar.GenerateAvatar()` 함수가 완료되면 호출되는 콜백 함수입니다. 이 함수는 다음 매개변수를 전달합니다.

- `avatarID` : 생성한 아바타 ID입니다.
- `response` : 아바타 생성 결과를 나타내는 객체입니다. 이 객체에는 `json` 및 `error_msg` 프로퍼티가 있습니다.

`json` 프로퍼티는 생성된 아바타의 정보가 JSON 형태로 포함되어 있습니다. `error_msg` 프로퍼티는 서버에서 발생한 오류 메시지입니다. `AvatarCallback()` 함수에서는 이러한 결과를 처리하고 필요한 후속 작업을 수행할 수 있습니다.

### JSON 정보에 대한 추가 설명
아바타 생성 결과로 받아온 JSON 정보는 서버에서 생성된 아바타의 정보를 담고 있습니다. 이 정보를 파싱하여 필요한 정보를 추출하거나 사용할 수 있습니다.
``` json
{
  "message": "successfuly!!",
  "data": {
    "Avatar": {
      "BlendShape": {
        ...
      },
      "Accessories": {
        "hair": {
          "style": 0,
          "curl": 0,
          "bang": 0
        },
        "glasses": 0
      },
      "Color": {
        "skin": {
          "R": 255,
          "G": 237,
          "B": 194
        },
        "hair": {
          "R": 88,
          "G": 74,
          "B": 62
        }
      },
      "Gender": "male"
    }
  }
}
```
각각의 항목은 다음과 같은 의미를 가지고 있습니다.

- "message": 요청이 성공적으로 완료되었는지에 대한 메시지를 포함합니다.
- "data": 아바타 생성 결과 데이터를 포함하는 객체입니다.
- "Avatar": 생성된 아바타 정보를 포함하는 객체입니다.
- "BlendShape": 아바타의 블렌드 쉐이프 정보를 포함하는 객체입니다.
- "Accessories": 아바타의 액세서리 정보를 포함하는 객체입니다.
- "hair": 머리카락 스타일, 컬, 앞머리 정보를 포함하는 객체입니다.
- "glasses": 안경 유무를 나타내는 값입니다.
- "Color": 아바타의 색상 정보를 포함하는 객체입니다.
- "skin": 피부색 정보를 포함하는 객체입니다.
- "hair": 머리카락 색상 정보를 포함하는 객체입니다.
- "Gender": 아바타의 성별 정보를 포함합니다.

이 구조에 따라서 아바타 생성 결과를 처리하시면 됩니다. json 예제 파일은 [여기서](docs/avatar_api_sample.json) 확인하시면 됩니다.

## ScriptableObject로 아바타 에셋 관리
MirrorGearSDK는 CMS서버에 받은 JSON 값으로 아바타 에셋을 생성하고 관리하고 있습니다. 이때, 아바타 에셋은 ScriptableObject를 사용하여 관리합니다.

AvatarContainerSO의 구조는 다음과 같습니다.

![image01](/docs/images/image01.png)
`Avatars`에서 여성과 남성아바타 에셋을 각각 관리하고 있으며, 0번 인덱스는 여성아바타, 1번 인덱스는 남성아바타 에셋을 관리하고 있습니다. `Avatars`을 통해 아바타의 관련된 에셋을 가져올 수 있습니다.

- `Model` : 아바타의 모델(GameObject)을 저장합니다.
- `PositionOffset` : 아바타의 모델의 위치를 조정하기 위한 오프셋 벡터(Vector3)입니다.
- `RotationOffset` : 아바타의 모델의 회전을 조정하기 위한 오프셋 벡터(Vector3)입니다.
- `Hair` : 아바타 모델에 적용할 수 있는 머리카락(GameObject) 배열입니다. 이 배열의 인덱스 값은 json의 `Accessories/hair/style` 값과 일치합니다. 따라서 `AvatarData.Hairs`의 인덱스 값과 json의  `Accessories/hair/style` 값을 매핑하여 해당 머리카락 에셋을 가져올 수 있습니다.
- `Glasses` : 아바타 모델에 적용할 수 있는 안경(GameObject) 배열입니다. 이 배열의 인덱스 값은 json의 `Accessories/glasses` 값과 일치 합니다. 따라서 `AvatarData.Glasses`의 인덱스 값과 json의 `Accessories/glasses` 값을 매핑하여 해당 안경 에셋을 가져올 수 있습니다.
- `Cloths` : 아바타 모델에 적용할 수 있는 옷(GameObject) 배열입니다.

SDK를 사용해주셔서 감사합니다. 더 나은 서비스를 제공하기 위해 항상 노력하겠습니다.
