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
    [SerializeField] private WeaponData actualWeapon;
    private int weaponIndex;

    //Move Data's
    private PlayerInput playerInput;
    private Vector3 initialPosition = Vector3.zero;
    private float moveSpeed = 250f;
    private bool canMove = false;

    private void Start()
    {   
        //Get components
        playerInput = GetComponent<PlayerInput>();

        //Initialize the weapon index
        weaponIndex = PlayerPrefs.GetInt("Upgrade_StartWeaponIndex");

        //Make the initial characters spawn
        for(int i = 0; i < PlayerPrefs.GetInt("Upgrade_nbStartCharacter"); i++)
        {
            CreateNewCharacter();
        }

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

                initialPosition = new Vector3((playerInput.actions["Move"].ReadValue<Vector2>().x - Screen.width / 2) / Screen.width * 20, 1, 0);

                //Avoid Bug if the List changes
                List<Character> currentcharacters = new List<Character>(characters);

                //Find if there is null character on the list
                foreach (Character character in currentcharacters)
                {
                    if (character != null)
                    {
                        character.StartMove();
                    }
                }

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
            Vector3 newMovement = new Vector3((context.ReadValue<Vector2>().x - Screen.width / 2) / Screen.width * 20, 1, 0) - initialPosition;

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
                character.ChangeShoot(actualWeapon.bulletsGo[weaponIndex], actualWeapon.fireRate[weaponIndex] * PlayerPrefs.GetFloat("Upgrade_fireRateMultiply")
                                                                         , actualWeapon.bulletSpeed[weaponIndex] * PlayerPrefs.GetFloat("Upgrade_bulletSpeedMultiply")
                                                                         , actualWeapon.bulletDamage[weaponIndex] * PlayerPrefs.GetFloat("Upgrade_bulletDommageMultiply")
                                                                         , actualWeapon.bulletRange[weaponIndex] * PlayerPrefs.GetFloat("Upgrade_bulletRangeMultiply"));
            }
        }
    }

    public void UpgradeWeapon()
    {
        if(weaponIndex < actualWeapon.bulletsGo.Length - 1)
        {
            weaponIndex++;
        }

        ChangeCharactersShoot();
    }

    #endregion

    #region CharactersManagement

    public void CreateNewCharacter()
    {
        //Find a spawn Position that don't collapse with other characters
        Vector3 spawnPosition = GetSpawnPosition();

        //Initialize the GameObject
        GameObject newCharacter = Instantiate(characterGO,spawnPosition, Quaternion.identity);
        newCharacter.transform.SetParent(transform, true);

        //Get the Character Component
        Character character = newCharacter.GetComponent<Character>();

        //Add the newCharacters to the array of characters
        characters.Add(character);

        //Initialize the Shoot and MoveSpeed of the Character
        character.StartMove(characters[0].GetStartPosition(), characters[0].GetDesirePosition());
        character.ChangeMoveSpeed(moveSpeed * PlayerPrefs.GetFloat("Upgrade_moveSpeedMultiply"));
        character.ChangeShoot(actualWeapon.bulletsGo[weaponIndex], actualWeapon.fireRate[weaponIndex] * PlayerPrefs.GetFloat("Upgrade_fireRateMultiply")
                                                         , actualWeapon.bulletSpeed[weaponIndex] * PlayerPrefs.GetFloat("Upgrade_bulletSpeedMultiply")
                                                         , actualWeapon.bulletDamage[weaponIndex] * PlayerPrefs.GetFloat("Upgrade_bulletDommageMultiply")
                                                         , actualWeapon.bulletRange[weaponIndex] * PlayerPrefs.GetFloat("Upgrade_bulletRangeMultiply"));

        //Updates Camera Targets
        followCharacter.AddCharacter(character);
    }

    private Vector3 GetSpawnPosition()
    {
        Vector3 spawnPosition;

        if(characters.Count > 0)
        {
            spawnPosition = characters[Random.Range(0, characters.Count)].GetComponent<Transform>().position + new Vector3(Random.Range(0, 2) * 2 - 1, 0, Random.Range(0, 2));

            while (Mathf.Abs(spawnPosition.x) > 7f && Physics.CheckSphere(spawnPosition - new Vector3(0, 0.5f, 0), 0.4f, 3))
            {
                spawnPosition = characters[Random.Range(0, characters.Count)].GetComponent<Transform>().position + new Vector3(Random.Range(0, 2) * 2 - 1, 0, Random.Range(0, 2));
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
        //Remove the character from the characters list
        characters.Remove(character);

        //Remove the character from the group target
        followCharacter.RemoveCharacter(character);

        //Game Over
        if(characters.Count == 0)
        {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().GameOver();
        }
    }
    #endregion

    #region Balance

    public float getFirePower(float seconds)
    {
        //Return the FirePower of the troups during x seconds
        float nbFirePerSeconds = 1 / actualWeapon.fireRate[weaponIndex];

        // Get the dps (nbFirePerSeconds * damage of a bullet * nb character * nb bullets per shoot) and multiply it by seconds
        return nbFirePerSeconds * actualWeapon.bulletDamage[weaponIndex] * characters.Count * actualWeapon.bulletsGo[weaponIndex].transform.childCount * seconds;
    }

    #endregion
}

