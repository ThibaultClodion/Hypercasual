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
    [SerializeField] GameObject bulletGO;
    private List<Character> characters;
    private float fireSpeed = 0.5f;
    private float fireDamage = 5f;
    private float bulletSpeed = 15f;
    private float gauge = 0f;

    // Start is called before the first frame update
    void Start()
    {
        //First Character to spawn
        CreateNewCharacter();


        //Initialize the characters shoot
        StartCoroutine(CharactersShoot());
    }

    private void FixedUpdate()
    {
        //Move each character on the troup
        foreach (Character character in characters) 
        {
            if(character != null)
            {
                character.Move(movePosition);
            }
        }
    }

    #region Movement
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

        //This avoid bugs (going through wall)
        if (movePosition.x > 10f)
        {
            movePosition.x = 10f;
        }
        else if(movePosition.x < -10f)
        {
            movePosition.x = -10f;
        }
    }
    #endregion

    #region Shoot
    IEnumerator CharactersShoot()
    {
        yield return new WaitForSeconds(fireSpeed);

        //Make the character fire
        foreach (Character character in characters)
        {
            if (character != null)
            {
                character.Fire(bulletGO, new Vector3(0, 0, 0.80f));
            }
        }

        StartCoroutine(CharactersShoot());
    }
    #endregion

    #region CharactersManagement

    private void CreateNewCharacter()
    {
        //Random position of spawn
        Vector3 spawnPosition = new Vector3(Random.Range(-3f,3f), 1, Random.Range(-1f, 3f));

        GameObject newCharacter = Instantiate(characterGO,spawnPosition, Quaternion.identity, transform);
        characters.Add(newCharacter.GetComponent<Character>());

        //Update to don't have too much null object on the characters list
        UpdateCharacters();
    }

    private void UpdateCharacters()
    {
        //Find if there is null character on the list
        foreach (Character character in characters)
        {
            if (character == null)
            {
                characters.Remove(character);
            }
            else
            {
                character.Init(bulletSpeed, fireDamage);
            }
        }
    }
    #endregion

    #region Balance

    public void increaseGauge(float increment)
    {
        gauge += increment;

        int cap = 10; //For test purpose

        //Add a character if the gauge is greater than the cap
        if (gauge > cap)
        {
            CreateNewCharacter();
            gauge = 0;
        }
    }

    public float getFirePower(float seconds)
    {
        //To avoid bug, update the Characters
        UpdateCharacters();

        //Return the FirePower of the troups during x seconds
        float nbFirePerSeconds = 1 / fireSpeed;
        return nbFirePerSeconds * seconds * fireDamage * characters.Count;
    }

    #endregion
}
