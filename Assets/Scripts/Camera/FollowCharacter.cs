using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class FollowCharacter : MonoBehaviour
{
    [SerializeField] private Transform firstTarget;
    private CinemachineTargetGroup group;

    private void Awake()
    {
        group = GetComponent<CinemachineTargetGroup>();
        group.AddMember(firstTarget, 1, 0.5f);
    }

    public void AddCharacter(Character character)
    {
        group.AddMember(character.GetComponent<Transform>(), 0.5f, 0.5f);
    }

    public void RemoveCharacter(Character character) 
    {
        group.RemoveMember(character.GetComponent<Transform>());
    }
}
