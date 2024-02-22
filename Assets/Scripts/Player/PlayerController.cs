using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Camera Data's")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private FollowCharacter followCharacter;

    [Header("Character Data's")]

    //Character Datas
    [SerializeField] GameObject characterGO;
    private List<Character> characters = new List<Character>();

    //Shoot Data's
    [SerializeField] GameObject bulletGO;
    private float fireSpeed = 0.4f;
    private float fireDamage = 5f;
    private float bulletSpeed = 75f;

    //Move Data's
    private float moveSpeed = 10f;
    private bool canMove = false;

    //Gauge Data's
    private int actualGauge = 0;
    private int[] gaugeCap = new int[] {0, 5, 10, 20, 30,
                                        50, 70, 80 ,90 ,100,
                                        100, 150, 150 ,200 ,250,
                                        300, 350, 400, 500, 500};

    [Header("Canvas Data's")]
    [SerializeField] Slider gaugeSlider;

    private void Start()
    {
        //First Character to spawn
        CreateNewCharacter();

        //Initialize the characters shoot
        ChangeCharactersShoot();
    }

    #region Movement

    public void CanMove(InputAction.CallbackContext context)
    {
        switch(context.phase) 
        {
            //The Hold begin
            case InputActionPhase.Started:
                canMove = true;
                break;
            //The Hold end
            case InputActionPhase.Canceled:
                canMove= false;
                break;
        }
    }
    public void Move(InputAction.CallbackContext context)
    {
        //Find the problem with the initial click that make back to the center of the map
        if (canMove)
        {
            //Get the movePosition
            Vector3 newMovement = new Vector3((context.ReadValue<Vector2>().x - Screen.width / 2) * 20 / Screen.width, 0, 0);

            //Avoid Bug if the List changes
            List<Character> currentcharacters = new List<Character>(characters);

            //Find if there is null character on the list
            foreach (Character character in currentcharacters)
            {
                if (character != null)
                {
                    character.ChangeMove(newMovement);
                }
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
        Vector3 spawnPosition = GetSpawnPosition();

        //Initialize the GameObject
        GameObject newCharacter = Instantiate(characterGO,spawnPosition, Quaternion.identity);
        newCharacter.transform.SetParent(transform, true);

        //Get the Character Component
        Character character = newCharacter.GetComponent<Character>();

        //Initialize the Shoot and MoveSpeed of the Character
        character.ChangeShoot(bulletGO, bulletSpeed, fireDamage, fireSpeed, new Vector3(0, 0, 0.80f));
        character.ChangeMoveSpeed(moveSpeed);

        //Add the newCharacters to the array of characters
        characters.Add(character);

        //Updates Camera Targets
        followCharacter.AddCharacter(character);
    }

    private Vector3 GetSpawnPosition()
    {
        Vector3 spawnPosition;

        if(characters.Count > 0)
        {
            spawnPosition = characters[Random.Range(0, characters.Count)].GetComponent<Transform>().position + new Vector3(Random.Range(0, 2) * 2 - 1, 0, Random.Range(0, 2));

            while (Mathf.Abs(spawnPosition.x) > 7f)
            {
                spawnPosition = characters[Random.Range(0, characters.Count)].GetComponent<Transform>().position + new Vector3(Random.Range(0, 2) * 2 - 1, 0, Random.Range(0, 2));
                //return new Vector3(Random.Range(-9, 9), 1, Random.Range(0, 5));
            }
        }
        else
        {
            spawnPosition = new Vector3(0,1,0);
        }

        return spawnPosition;
    }

    public void CharacterDestroy(Character character)
    {
        //Reset the actualGauge
        actualGauge = 0;
        gaugeSlider.value = 0f;

        //Remove the character from the characters list
        characters.Remove(character);

        //Remove the character from the group target
        followCharacter.RemoveCharacter(character);
    }
    #endregion

    #region Balance

    public void increaseGauge(int increment)
    {
        actualGauge += increment;
        int cap;

        if (characters.Count < gaugeCap.Length)
        {
            cap = gaugeCap[characters.Count];
        }
        else
        {
            cap = gaugeCap[characters.Count] + (characters.Count - gaugeCap.Length) * 100;
        }

        //Add a character if the gauge is greater than the cap
        if (actualGauge > cap)
        {
            CreateNewCharacter();
            increaseGauge(-cap);
        }
        else
        {
            //Update the Slider
            gaugeSlider.value = (float)actualGauge / (float)cap;
        }
    }

    public float getFirePower(float seconds)
    {
        //Return the FirePower of the troups during x seconds
        float nbFirePerSeconds = 1 / fireSpeed;
        return nbFirePerSeconds * seconds * fireDamage * characters.Count;
    }

    #endregion
}
