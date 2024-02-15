using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class FollowCharacter : MonoBehaviour
{
    private CinemachineTargetGroup group;

    private void Awake()
    {
        group = GetComponent<CinemachineTargetGroup>();
    }

    public void UpdateTargets(List<Character> characters)
    {
        //For the Initialization
        if(characters.Count == 1) 
        {
            StartCoroutine(UpdateWaitTarget(characters, 0));
        }
        else
        {
            StartCoroutine(UpdateWaitTarget(characters, 0.5f));
        }
    }

    IEnumerator UpdateWaitTarget(List<Character> characters, float seconds)
    {
        yield return new WaitForSeconds(seconds);

        foreach (Character character in characters)
        {
            if(character != null)
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
