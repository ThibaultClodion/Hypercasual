using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //Datas use for Camera
    [SerializeField] private Camera mainCamera;
    [SerializeField] private FollowCharacter followCharacter;

    //Datas use for movement
    private RaycastHit hit;
    private PlayerInput playerInput;

    //Character Data
    [SerializeField] GameObject characterGO;
    [SerializeField] GameObject bulletGO;
    private List<Character> characters = new List<Character>();
    private float fireSpeed = 0.5f;
    private float fireDamage = 5f;
    private float bulletSpeed = 15f;
    private float gauge = 0f;

    // Start is called before the first frame update
    void Awake()
    {
        //get components
        playerInput = GetComponent<PlayerInput>();

        //First Character to spawn
        CreateNewCharacter();

        //Initialize the characters shoot
        StartCoroutine(CharactersShoot());
    }

    private void FixedUpdate()
    {
        //Avoid Bug if the List changes
        List<Character> currentcharacters = new List<Character>(characters);

        //Get the movePosition
        Vector3 movePosition = FindMovePosition(playerInput.actions["Move"].ReadValue<Vector2>());

        //Move each character on the troup
        foreach (Character character in currentcharacters) 
        {
            if(character != null)
            {
                character.Move(movePosition);
            }
        }
    }

    #region Movement
    private Vector3 FindMovePosition(Vector2 inputPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(inputPosition);

        Vector3 movePosition = new Vector3(0,1,0);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            movePosition = hit.point;
            movePosition.y = 1;
            movePosition.z = 0;
        }

        return movePosition;
    }
    #endregion

    #region Shoot
    IEnumerator CharactersShoot()
    {
        yield return new WaitForSeconds(fireSpeed);

        //Avoid Bug if the List changes
        List<Character> currentcharacters = new List<Character>(characters);

        //Make the character fire
        foreach (Character character in currentcharacters)
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
        //Find a spawn Position that don't collapse with other characters
        Vector3 spawnPosition = new Vector3(Random.Range(-5f,5f), 1, Random.Range(0, 3f));

        while(Physics.OverlapSphere(spawnPosition - new Vector3(0, 0.5f, 0), 0.5f, 3).Length > 1)
        {
            spawnPosition = new Vector3(Random.Range(-5f, 5f), 1, Random.Range(0, 3f));
        }

        GameObject newCharacter = Instantiate(characterGO,spawnPosition, Quaternion.identity, transform);
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
                character.Init(bulletSpeed, fireDamage);
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
