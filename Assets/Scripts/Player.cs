using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    //movements
    public float moveSpeed = 5f;
    [HideInInspector]
    public float currentMoveSpeed;
    private IA_Main inputActions;
    private Vector2 moveInput;
    private Rigidbody rb;

    //cam
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float mouseSensitivity = 100f;
    private float xRotation = 0f;

    //score
    private int playerScore = 0;
    [HideInInspector]
    public int PlayerScore => playerScore;
    private TextMeshProUGUI textScore;
    private AudioClip scoreClip;
    [HideInInspector]
    public int currentScoreMultipliyer = 1;

    //inventory
    [SerializeField]
    private int maxInventoryObjects = 4;
    private int selectedInventoryIndex = 0;
    private List<InventoryObject> inventory = new List<InventoryObject>();

    //audio
    private AudioSource playerAudio;

    #region UnityFunctions
    private void Awake()
    {
        inputActions = new IA_Main();

        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        inputActions.Player.UsePower.performed += _ => UsePowerCurrentSlot();
        inputActions.Player.UsePower.canceled += _ => UsePowerCurrentSlot();

        inputActions.Player.SelectSlot.performed += ctx =>
        {
            var key = ctx.control.name;

            //Debug.Log("selected key: " + key);

            if (int.TryParse(key, out int number))
            {
                if (number - 1 < maxInventoryObjects)
                {
                    SetInventorySlot(number - 1);
                }
                
            }
        };

        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        textScore = GM.Instance.textScore;
        scoreClip = GM.Instance.scoreSound;

        playerAudio = GetComponent<AudioSource>();
        //playerAudio.clip = scoreClip;

        UpdateScoreUI();
        InitInventoryUI();
        UpdateSelectedSlotUI(selectedInventoryIndex);

        currentMoveSpeed = moveSpeed;

        for (int i = 0; i < maxInventoryObjects; i++)
        {
            inventory.Add(null);
        }
    }

    private void FixedUpdate()
    {
        if (moveInput != null)
        {
            Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
            Vector3 newPosition = rb.position + move * currentMoveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(newPosition);
        }
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        //Debug.Log(mouseY);
        xRotation -= mouseY;
        //Debug.Log(xRotation);
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void OnTriggerEnter(Collider other)
    {
        InventoryObject inventoryObj = other.GetComponent<InventoryObject>();

        // si inventaire object
        if (inventoryObj != null)
        {
            inventoryObj.isInInventory = true;
            AddToInventory(inventoryObj);
        }
        // si score object
        else
        {
            ScoreObject scoreObj = other.GetComponent<ScoreObject>();

            if (scoreObj != null)
            {
                AddScore(scoreObj.score);
                playerAudio.clip = scoreClip;
                playerAudio.Play();
                scoreObj.destroyCoroutine = null;
                Destroy(scoreObj.gameObject);
            }
        }
    }

    #endregion


    #region Inventory
    private void InitInventoryUI()
    {
        for (int i = 0; i < maxInventoryObjects; i++)
        {
            Instantiate(GM.Instance.inventoryObject, GM.Instance.inventoryUIParent);
        }
    }

    private void AddToInventory(InventoryObject item)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i] == null)
            {
                inventory[i] = item;
                UpdateInventoryUI(i);

                item.gameObject.SetActive(false);
                return;
            }
        }
    }

    private void RemoveFromInventory(int index)
    {
        if (inventory[index] != null)
        {
            Destroy(inventory[index].gameObject);
            inventory[index] = null;
            UpdateInventoryUI(index);
        }
    }

    private void UpdateInventoryUI(int index)
    {
        if (GM.Instance.inventoryUIParent)
        {
            Image img = GM.Instance.inventoryUIParent.GetChild(index).GetComponent<Image>();

            if (inventory[index])
            {
                img.sprite = inventory[index].sprite;
                img.enabled = true;
            }
            else
            {
                img.enabled = false;
            }
        } 
    }

    private void UpdateSelectedSlotUI(int index, bool selected = true)
    {
        //Update la couleur du cadre
        if (GM.Instance.inventoryUIParent)
        {
            Image img = GM.Instance.inventoryUIParent.GetChild(index).GetChild(1).GetComponent<Image>();
            if (selected) img.color = GM.Instance.selectedSlotColor;
            else img.color = Color.black;
        }
    }

    private void SetInventorySlot(int index)
    {
        UpdateSelectedSlotUI(selectedInventoryIndex, false);
        //Debug.Log("New Inventory Slot Selected: " + index);
        selectedInventoryIndex = index;
        UpdateSelectedSlotUI(index);
    }

    private void UsePowerCurrentSlot()
    {
        if (inventory[selectedInventoryIndex])
        {
            inventory[selectedInventoryIndex].UsePower();
            playerAudio.clip = GM.Instance.powerUpSound;
            playerAudio.Play();
            RemoveFromInventory(selectedInventoryIndex);
        }
    }
    #endregion


    #region Score
    private void AddScore(int score)
    {
        playerScore += (score * currentScoreMultipliyer);
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        textScore.text = playerScore.ToString();
    }
    #endregion
}
