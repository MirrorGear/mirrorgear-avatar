using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AvatarAnimation : MonoBehaviour
{
    public RuntimeAnimatorController animController;

    private GameObject character;
    private Rigidbody charRigidbody;
    private Animator parentAnimator;

    // 하위 오브젝트들의 Animator 컴포넌트
    Animator[] childAnimators;

    public void InitCharacter(GameObject obj)
    {
        // Capsule Collider 추가
        CapsuleCollider capsuleCollider = obj.AddComponent<CapsuleCollider>();
        capsuleCollider.center = new Vector3(0, 0.9f, 0);
        capsuleCollider.radius = 0.18f;
        capsuleCollider.height = 1.75f;

        // Rigidbody 추가
        charRigidbody = obj.AddComponent<Rigidbody>();
        charRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

        // 캐릭터 설정함.
        SetCharacter(obj);
    }

    public void SetCharacter(GameObject obj)
    {
        character = obj;
        charRigidbody = character.GetComponent<Rigidbody>();
        parentAnimator = character.GetComponent<Animator>();
        childAnimators = character.GetComponentsInChildren<Animator>();

        SettingAnimatorController();
    }

    void SettingAnimatorController()
    {
        parentAnimator.runtimeAnimatorController = animController;

        // 하위 오브젝트들의 Animator 컴포넌트의 animator 추가
        foreach (Animator childAnimator in childAnimators)
        {
            childAnimator.runtimeAnimatorController = animController;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
