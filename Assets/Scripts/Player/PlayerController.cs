using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class PlayerController : MonoBehaviour
{
    [Header("Camera Data's")]
    [SerializeField] private Camera mainCamera;

    [Header("Character Data's")]
    [SerializeField] GameObject characterGO;
    [SerializeField] GameObject bulletGO;
    private List<Character> characters = new List<Character>();
    private float fireSpeed = 0.5f;
    private float fireDamage = 5f;
    private float bulletSpeed = 15f;
    private float moveSpeed = 5f;
    private float gauge = 0f;

    //Components Data
    private RaycastHit hit;
    private Vector3 movement;
    private Rigidbody rb;

    void Awake()
    {
        //Initialize components
        rb = GetComponent<Rigidbody>();

        //First Character to spawn
        CreateNewCharacter();

        //Initialize the characters shoot
        ChangeCharactersShoot();
    }

    private void Start()
    {
        StartCoroutine(TestSpawn());
    }

    IEnumerator TestSpawn()
    {
        yield return new WaitForSeconds(3f);
        CreateNewCharacter() ;
        StartCoroutine(TestSpawn());
    }


    #region Movement

    private void Move()
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
        }
    }

    private void OnMove(InputValue inputPosition)
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

        //Initialize the Shoot and MoveSpeed of the Character
        newCharacter.GetComponent<Character>().ChangeShoot(bulletGO, bulletSpeed, fireDamage, fireSpeed, new Vector3(0, 0, 0.80f));
        newCharacter.GetComponent<Character>().ChangeMoveSpeed(moveSpeed);

        //Add the newCharacters to the array of characters
        characters.Add(newCharacter.GetComponent<Character>());

        //Update to don't have too much null object on the characters list
        UpdateCharacters();
    }

    private Vector3 GetSpawnPosition()
    {
        if(characters.Count > 0)
        {
            return new Vector3(Random.Range(-9, 9), 1, Random.Range(0, 5));
        }
        else
        {
            return new Vector3(0,1,0);
        }
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
        }
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
