using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField]
    private Canvas gameOverCanvas;
    public Player player;
    public Vector2 worldLimits = new Vector2(-20, 20);

    //inventory
    [SerializeField]
    private List<InventoryObject> inventoryObjectList;
    public Color selectedSlotColor = new Color(69f, 241f, 47f);
    public Transform inventoryUIParent;
    public GameObject inventoryObject;

    // texts
    public TextMeshProUGUI textScore;
    public TextMeshProUGUI finalTextScore;
    public TextMeshProUGUI textTime;

    //audios
    [SerializeField]
    private AudioClip popSound;
    public AudioClip scoreSound;
    public AudioClip powerUpSound;

    //objects
    [SerializeField] private GameObject scoreObjectType1;
    [SerializeField] private GameObject scoreObjectType2;
    [SerializeField] private List<ObjectSpawner> objectSpawnersList;

    //time
    [HideInInspector]
    public int remainingSeconds;
    private int maxTime = 60;

    #region UnityFunctions
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        StartCoroutine(SpawnInventoryObject());
        StartCoroutine(SpawnScoreObjects());
        StartCoroutine(UpdateGameTime());
    }
    #endregion


    #region Coroutines
    private IEnumerator SpawnInventoryObject()
    {
        yield return new WaitForSeconds(1f);

        while (true)
        {
            // seulement les spawners vides
            List<ObjectSpawner> emptySpawners = objectSpawnersList.FindAll(s => s.IsEmpty());

            if (emptySpawners.Count > 0)
            {
                ObjectSpawner chosenSpawner = emptySpawners[Random.Range(0, emptySpawners.Count)];

                InventoryObject inventoryObject = inventoryObjectList[Random.Range(0, inventoryObjectList.Count)];

                GameObject spawned = Instantiate(inventoryObject.gameObject, chosenSpawner.transform.position, Quaternion.identity);
                chosenSpawner.audioSource.clip = popSound;
                chosenSpawner.audioSource.Play();
                chosenSpawner.CurrentObject = spawned;

                StartCoroutine(chosenSpawner.DestroyAfterDelay(spawned, 10f));
            }

            yield return new WaitForSeconds(5f);
        }
    }

    private IEnumerator UpdateGameTime()
    {
        remainingSeconds = maxTime;

        while (remainingSeconds > 0)
        {
            int minutes = remainingSeconds / 60;
            int seconds = remainingSeconds % 60;

            textTime.text = $"{minutes:00}:{seconds:00}";
            yield return new WaitForSeconds(1f);
            remainingSeconds--;
        }

        textTime.text = "00:00";
        finalTextScore.text = "Score: "+player.PlayerScore.ToString();
        gameOverCanvas.gameObject.SetActive(true);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    private IEnumerator SpawnScoreObjects()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);

            SpawnRandomScoreObjects();
        }
    }
    #endregion

    public void SpawnRandomScoreObjects()
    {
        // proba 75% / 25%
        GameObject prefabToSpawn = Random.value < 0.75f ? scoreObjectType1 : scoreObjectType2;

        float x = Random.Range(worldLimits.x, worldLimits.y);
        float z = Random.Range(worldLimits.x, worldLimits.y);
        Vector3 spawnPosition = new Vector3(x, 0.5f, z);

        GameObject instance = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        ScoreObject scoreObj = instance.GetComponent<ScoreObject>();
        scoreObj.StartDestroyCoroutine(10f);
    }

}

public static class GM
{
    public static GameManager Instance => GameManager.Instance;
}
