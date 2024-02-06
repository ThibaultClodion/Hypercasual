using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //Datas use for Movement
    [SerializeField] Camera mainCamera;
    private Vector3 touchPosition;
    private Vector3 movePosition;

    //Character Data
    [SerializeField] GameObject characterGO;
    private List<Character> characters;

    // Start is called before the first frame update
    void Start()
    {
        //For Test
        CreateNewCharacter();
        CreateNewCharacter();
        CreateNewCharacter();
        CreateNewCharacter();
        CreateNewCharacter();
    }

    private void FixedUpdate()
    {
        //Move each characters
        foreach (Character character in characters) 
        {
            character.Move(movePosition);
        }
    }

    private void CreateNewCharacter()
    {
        //Random position of spawn
        Vector3 spawnPosition = new Vector3(Random.Range(-3f,3f), 1, Random.Range(-1f, 3f));

        GameObject newCharacter = Instantiate(characterGO,spawnPosition, Quaternion.identity, transform);
        characters.Add(newCharacter.GetComponent<Character>());
    }

    private void OnMove(InputValue inputPosition)
    {
        //Get the position of the touch (-1 is necessary)
        touchPosition = new Vector3(inputPosition.Get<Vector2>().x, inputPosition.Get<Vector2>().y, mainCamera.transform.position.z);
        touchPosition = mainCamera.ScreenToWorldPoint(touchPosition) * -1;

        //Put x and z to 0 to only move horizontaly
        touchPosition.y = 1;
        touchPosition.z = 0;

        //Update the movePosition to the corrected touchPosition
        movePosition = touchPosition;
    }
}
