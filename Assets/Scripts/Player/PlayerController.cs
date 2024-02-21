using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static Unity.Burst.Intrinsics.X86.Avx;

public class PlayerController : MonoBehaviour
{
    [Header("Camera Data's")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private FollowCharacter followCharacter;

    [Header("Character Data's")]
    [SerializeField] GameObject characterGO;
    [SerializeField] GameObject bulletGO;
    private List<Character> characters = new List<Character>();
    private float fireSpeed = 0.4f;
    private float fireDamage = 5f;
    private float bulletSpeed = 75f;
    private float moveSpeed = 7.5f;
    private int actualGauge = 0;
    private bool canMove = false;
    private int[] gaugeCap = new int[] {0, 5, 10, 20, 30,
                                        50, 70, 80 ,90 ,100,
                                        100, 150, 150 ,200 ,250,
                                        300, 350, 400, 500, 500};

    [Header("Canvas Data's")]
    [SerializeField] Slider gaugeSlider;

    [Header("Map Data's")]
    [SerializeField] private LayerMask ignoreMoveLayer;

    //Components Data
    private PlayerInput playerInput;
    private RaycastHit hit;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        //First Character to spawn
        CreateNewCharacter();

        //Initialize the characters shoot
        ChangeCharactersShoot();
    }

    private void Update()
    {
        Move(playerInput.actions["Move"].ReadValue<Vector2>());
    }


    #region Movement

    private void Move(Vector2 inputPosition)
    {
        if(canMove) 
        {
            //Convert the inputPosition to WorldPosition
            Ray ray = mainCamera.ScreenPointToRay(inputPosition);
            Vector3 movePosition = Vector3.zero;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~ignoreMoveLayer))
            {
                movePosition = hit.point;
                movePosition.y = 1;
            }

            //This avoid player from clicking too far away
            if (movePosition.z < 15)
            {

                movePosition.z = 0;

                //Avoid Bug if the List changes
                List<Character> currentcharacters = new List<Character>(characters);

                //Change the movePosition of each character
                foreach (Character character in currentcharacters)
                {
                    if (character != null)
                    {
                        character.ChangeMove(movePosition);
                    }
                }
            }
        }
        else
        {
            //Avoid Bug if the List changes
            List<Character> currentcharacters = new List<Character>(characters);

            //Change the movePosition of each character
            foreach (Character character in currentcharacters)
            {
                if (character != null)
                {
                    character.DontMove();
                }
            }
        }
    }

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

    /*private void OnMove(InputValue inputPosition)
    {
        //Player Input give inputPosition of the player
        //movement = new Vector3((inputPosition.Get<Vector2>().x - Screen.width / 2) * 20 / Screen.width / 10, 0, 0);

        Ray ray = mainCamera.ScreenPointToRay(inputPosition.Get<Vector2>());
        Vector3 movePosition = Vector3.zero;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            movePosition = hit.point;
            movePosition.y = 1;
            movePosition.z = 0;
        }

        //Avoid Bug if the List changes
        List<Character> currentcharacters = new List<Character>(characters);

        //Find if there is null character on the list
        foreach (Character character in currentcharacters)
        {
            if (character != null)
            {
                character.ChangeMove(movePosition);
            }
        }
    }*/


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

        //Initialize the Shoot and MoveSpeed of the Character
        newCharacter.GetComponent<Character>().ChangeShoot(bulletGO, bulletSpeed, fireDamage, fireSpeed, new Vector3(0, 0, 0.80f));
        newCharacter.GetComponent<Character>().ChangeMoveSpeed(moveSpeed);

        //Add the newCharacters to the array of characters
        characters.Add(newCharacter.GetComponent<Character>());

        //Updates Camera Targets
        followCharacter.UpdateTargets(characters);
    }

    private Vector3 GetSpawnPosition()
    {
        Vector3 spawnPosition;

        if(characters.Count > 0)
        {
            spawnPosition = characters[Random.Range(0, characters.Count)].GetComponent<Transform>().position + new Vector3(Random.Range(0, 2) * 2 - 1, 0, Random.Range(0,2));
            //return new Vector3(Random.Range(-9, 9), 1, Random.Range(0, 5));

            if(Mathf.Abs(spawnPosition.x) > 4.5f)
            {
                spawnPosition = GetSpawnPosition();
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
