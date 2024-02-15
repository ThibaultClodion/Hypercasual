using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //Datas use for Camera
    [SerializeField] private Camera mainCamera;
    [SerializeField] private FollowCharacter followCharacter;

    //Character Data
    [SerializeField] GameObject characterGO;
    [SerializeField] GameObject bulletGO;
    private List<Character> characters = new List<Character>();
    private float fireSpeed = 0.5f;
    private float fireDamage = 5f;
    private float bulletSpeed = 15f;
    private float moveSpeed = 5f;
    private float gauge = 0f;

    // Start is called before the first frame update
    void Awake()
    {
        //First Character to spawn
        CreateNewCharacter();

        //Initialize the characters shoot
        ChangeCharactersShoot();
    }


    #region Movement
    private void OnMove(InputValue inputPosition)
    {
        //Avoid Bug if the List changes
        List<Character> currentcharacters = new List<Character>(characters);

        Vector3 movement = new Vector3((inputPosition.Get<Vector2>().x - Screen.width / 2) * 20 / Screen.width / 10, 0, 0);

        //Change Move for each characters
        foreach (Character character in currentcharacters)
        {
            if (character != null)
            {
                character.ChangeMove(movement);
            }
        }


    }
    #endregion

    #region Shoot
    private void ChangeCharactersShoot()
    {
        //Avoid Bug if the List changes
        List<Character> currentcharacters = new List<Character>(characters);

        //Make the character fire
        foreach (Character character in currentcharacters)
        {
            if (character != null)
            {
                character.ChangeShoot(bulletGO, bulletSpeed, fireDamage, fireSpeed, new Vector3(0, 0, 0.80f));
            }
        }
    }
    #endregion

    #region CharactersManagement

    private void CreateNewCharacter()
    {    
        //Find a spawn Position that don't collapse with other characters
        Vector3 spawnPosition = new Vector3(Random.Range(-5f,5f), 1, Random.Range(0, 3f));

        while(Physics.OverlapSphere(spawnPosition - new Vector3(0, 0.5f, 0), 0.5f, 3).Length > 1)
        {
            spawnPosition = new Vector3(Random.Range(-5f, 5f), 1, Random.Range(0, 3f));
        }

        GameObject newCharacter = Instantiate(characterGO,spawnPosition, Quaternion.identity, transform);
        newCharacter.GetComponent<Character>().Init(moveSpeed);
        characters.Add(newCharacter.GetComponent<Character>());

        //Update to don't have too much null object on the characters list
        UpdateCharacters();
    }

    private void UpdateCharacters()
    {
        //Avoid Bug if the List changes
        List<Character> currentcharacters = new List<Character>(characters);

        //Find if there is null character on the list
        foreach (Character character in currentcharacters)
        {
            if (character == null)
            {
                characters.Remove(character);
            }
            else
            {
                character.ChangeShoot(bulletGO, bulletSpeed, fireDamage, fireSpeed, new Vector3(0, 0, 0.80f));
            }
        }

        //Update the camera
        followCharacter.UpdateTargets(currentcharacters);
    }
    #endregion

    #region Balance

    public void increaseGauge(float increment)
    {
        gauge += increment;

        int cap = 1; //For test purpose

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
