using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class FollowCharacter : MonoBehaviour
{
    [SerializeField] private Transform firstTarget;
    private CinemachineTargetGroup group;

    private void Awake()
    {
        group = GetComponent<CinemachineTargetGroup>();
    }

    public void UpdateTargets(List<Character> characters)
    {
        //Remove the useless Member
        if (characters.Count == 1)
        {
            group.RemoveMember(firstTarget);
        }

        foreach (Character character in characters)
        {
            if (character != null)
            {
                Transform characterTransform = character.GetComponent<Transform>();

                if (group.FindMember(characterTransform) == -1)
                {
                    group.AddMember(characterTransform, 1, 0.5f);
                }
            }
        }
    }
}
