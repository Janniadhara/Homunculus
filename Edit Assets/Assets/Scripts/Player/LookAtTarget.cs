using Cinemachine;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LookAtTarget : MonoBehaviour
{
    [Header("Interacting")]
    [SerializeField] private float interactRange;
    [SerializeField] private Text InteractText;
    private GameObject MainCamera;
    private GameObject currentTarget = null;
    private GameObject lastTarget = null;
    [SerializeField] private GameObject CameraTarget;
    [SerializeField] private GameObject RaycastOrigin;
    [SerializeField] private GameObject RaycastOriginOrigin;
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    private float camDistance;

    //player Inventory
    [Header("Player Inventory")]
    [SerializeField] private EquipmentObject PlayerEquipment;
    [SerializeField] private WeaponItem[] WeaponItems;
    public bool playerInvOpened;

    //loot inventory from chests etc
    [Header("Chest Inventory")]
    private Animator ChestAnimator;
    public bool chestInvOpened;

    //speaking with npc
    //[Header("Dialog")]
    //[SerializeField] private GameObject DialogScreen;

    //map
    [Header("Map")]
    [SerializeField] private GameObject MapScreen;

    [SerializeField] private GameObject PlayerInfoScreen;

    public bool isAttacking;

    public bool canMoveBody;
    public bool canMoveCamera;
    private bool canInteract;
    private bool inventoryOpen;
    private bool canOpenInventory;
    private bool mapOpen;
    private bool canOpenMap;
    private bool journalOpen;
    private bool canOpenJournal;
    private bool questBoardOpen;
    private bool canOpenQuestBoard; 
    private bool menuOpen;
    private bool isHarvesting;
    private bool containerOpen;
    private bool canOpenContainer;
    //weapon
    public bool hasDrawnWeapon;
    public bool canTakeWeapon;
    public bool canAttack;

    public int test = 0;
    private void OnEnable()
    {
        EventsManager.Instance.dialogueEvent.onFinishDialogue += DialogueFinished;
        EventsManager.Instance.pickUpItemEvent.onPlantHarvested += PlantHarvested;
        EventsManager.Instance.pickUpItemEvent.onMineralMined += MineralMined;

        EventsManager.Instance.playerStateEvent.onCanMove += CanPlayerMove;
        EventsManager.Instance.playerStateEvent.onCanTakeWeapon += CheckCanTakeWeapon;
        EventsManager.Instance.playerStateEvent.onCanAttack += CanPlayerAttack;
    }
    private void OnDisable()
    {
        EventsManager.Instance.dialogueEvent.onFinishDialogue -= DialogueFinished;
        EventsManager.Instance.pickUpItemEvent.onPlantHarvested -= PlantHarvested;
        EventsManager.Instance.pickUpItemEvent.onMineralMined -= MineralMined;
    }
    private void Start()
    {
        MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        camDistance = virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>().CameraDistance;
        MapScreen = GameObject.Find("Map Screen");
        InteractText.text = "";
        playerInvOpened = false;
        chestInvOpened = false;
        hasDrawnWeapon = false;
        canAttack = false;
        isAttacking = false;
        PlayerInfoScreen.SetActive(false);
        MapScreen.SetActive(false);
        canOpenInventory = true;
        canOpenJournal = true;
        canOpenMap = true;
        canOpenQuestBoard = true;
        canOpenContainer = true;
        EventsManager.Instance.playerStateEvent.CanMove(true);
        canMoveCamera = true;
        canInteract = true;
    }
    
    // Update is called once per frame
    void Update()
    {
        InteractText.gameObject.transform.parent.gameObject.SetActive(canInteract);
        //interaction ray
        if (canInteract)
        {
            Vector3 origin = RaycastOrigin.transform.position;
            Vector3 direction = (origin - MainCamera.transform.position);
            direction.Normalize();

            //when looking at something
            int layerMask = 1 << 4;
            layerMask = ~layerMask;
            if (Physics.Raycast(origin, direction, out RaycastHit hit, interactRange, layerMask))
            {
                Debug.DrawRay(origin, direction * 2, Color.yellow);

                currentTarget = hit.transform.gameObject;
                UpdateInteractText();
                OutlineObject();
                lastTarget = currentTarget;
            }
            //if "löcher in die luft gucken", remove potential outline from last target and set current target null
            else
            {
                if (currentTarget != null && (lastTarget.CompareTag("Container") || lastTarget.CompareTag("Item")))
                {
                    lastTarget.transform.gameObject.GetComponent<Outline>().enabled = false;
                    currentTarget = null;
                    InteractText.text = "";
                }
                else
                {
                    currentTarget = null;
                    InteractText.text = "";
                }
            }
        }
        //camerazoom
        if (canMoveCamera)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                camDistance -= Time.deltaTime * 16;
                if (camDistance < 1.5f)
                {
                    camDistance = 1.5f;
                }
                virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>().CameraDistance = camDistance;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                camDistance += Time.deltaTime * 16;
                if (camDistance > 6f)
                {
                    camDistance = 6f;
                }
                virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>().CameraDistance = camDistance;
            }
        }
        //open player inventory
        if (Input.GetKeyDown(KeyCode.I) && canOpenInventory)
        {
            inventoryOpen = !inventoryOpen;
            if (inventoryOpen)
            {
                canOpenMap = false;
                canOpenJournal = false;
                canOpenQuestBoard = false;
                canOpenContainer = false;
                canMoveBody = false;
                canMoveCamera = false;
                canInteract = false;
                InventoryManager.Instance.OpenPlayerInventory();
            }
            else
            {
                canOpenMap = true;
                canOpenJournal = true;
                canOpenQuestBoard = true;
                canOpenContainer = true;
                canMoveBody = true;
                canMoveCamera = true;
                canInteract = true;
                InventoryManager.Instance.ClosePlayerInventory();
            }
            CheckInteractions();
        }
        //quest journal
        if (Input.GetKeyDown(KeyCode.J) && canOpenJournal)
        {
            journalOpen = !journalOpen;
            if (journalOpen)
            {
                canOpenMap = false;
                canOpenInventory = false;
                canOpenQuestBoard = false;
                canOpenContainer = false;
                canMoveBody = false;
                canMoveCamera = false;
                canInteract = false;
                QuestManager.Instance.OpenJournalScreen();
            }
            else
            {
                canOpenMap = true;
                canOpenInventory = true;
                canOpenQuestBoard = true;
                canOpenContainer = true;
                canMoveBody = true;
                canMoveCamera = true;
                canInteract = true;
                QuestManager.Instance.CloseJournalScreen();
            }
            CheckInteractions();
        }
        //map
        if (Input.GetKeyDown(KeyCode.M) && canOpenMap)
        {
            mapOpen = !mapOpen;
            if (mapOpen)
            {
                canOpenInventory = false;
                canOpenJournal = false;
                canOpenQuestBoard = false;
                canOpenContainer = false;
                canMoveBody = false;
                canMoveCamera = false;
                canInteract = false;
                MapScreen.SetActive(true);
            }
            else
            {
                canOpenInventory = true;
                canOpenJournal = true;
                canOpenQuestBoard = true;
                canOpenContainer = true;
                canMoveBody = true;
                canMoveCamera = true;
                canInteract = true;
                MapScreen.SetActive(false);
            }
            CheckInteractions();
        }
        //pause game and menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            menuOpen = !menuOpen;
            if (menuOpen)
            {
                canOpenInventory = false;
                canOpenJournal = false;
                canOpenMap = false;
                canOpenQuestBoard = false;
                canOpenContainer = false;
                canMoveBody = false;
                canMoveCamera = false;
                canInteract = false;
            }
            else
            {
                canMoveBody = true;
                canMoveCamera = true;
                canInteract = true;
                ResetScreens();
            }
            if (isHarvesting)
            {
                canMoveBody = false;
                canInteract = false;
            }
            CheckInteractions();
            TimeManager.Instance.PauseGame();
        }
        //tab closes all
        if (Input.GetKeyDown(KeyCode.Tab) && !menuOpen)
        {
            //close screens (but not unpause)
            canMoveBody = true;
            canMoveCamera = true;
            canInteract = true;
            ResetScreens();
            if (isHarvesting)
            {
                canMoveBody = false;
                canInteract = false;
            }
            CheckInteractions();
        }
        /*
        //take weapon or put away
        if (Input.GetKeyDown(KeyCode.Y) && canMoveBody && canTakeWeapon)
        {
            hasDrawnWeapon = !hasDrawnWeapon;
            if (hasDrawnWeapon)
            {
                canMoveBody = false;
                canTakeWeapon = false;
                canAttack = false;
                AnimatorOverrideController weaponAnimations = 
                    (PlayerEquipment.Weapon as WeaponItem).weaponAnimations;
                EventsManager.Instance.playerStateEvent.DrawWeapon(weaponAnimations);
            }
            else
            {
                canMoveBody = false;
                canTakeWeapon = false;
                canAttack = false;
                EventsManager.Instance.playerStateEvent.SheathWeapon();
            }
        }
        //attacking
        if (Input.GetKeyDown(KeyCode.Mouse0) && canMoveBody && canAttack)
        {
            canMoveBody = false;
            canTakeWeapon = false;
            canAttack = false;
            EventsManager.Instance.playerStateEvent.AttackEnemy();
            //InventoryManager.Instance.AttackWithEquipedWeapon();
        }
        //items swapping for now
        if (Input.GetKeyDown(KeyCode.Alpha1) && !hasDrawnWeapon)
        {
            PlayerEquipment.Weapon = WeaponItems[0];
            InventoryManager.Instance.EquipWeapon();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && !hasDrawnWeapon)
        {
            PlayerEquipment.Weapon = WeaponItems[1];
            InventoryManager.Instance.EquipWeapon();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && !hasDrawnWeapon)
        {
            PlayerEquipment.Weapon = WeaponItems[2];
            InventoryManager.Instance.EquipWeapon();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) && !hasDrawnWeapon)
        {
            PlayerEquipment.Weapon = WeaponItems[3];
            InventoryManager.Instance.EquipWeapon();
        }
        */
        //interactions with "E"
        if (Input.GetKeyDown(KeyCode.E) && currentTarget != null && canInteract)
        {
            if (currentTarget.tag == "Item")
            {
                var Item = currentTarget.GetComponent<AttachedItem>();
                if (Item.item.type == ItemType.Money)
                {
                    CoinItem Coin = Item.item as CoinItem;
                    EventsManager.Instance.moneyGainedEvent.GainMoney(
                        Coin.valueBronze * Item.amount, 
                        Coin.valueSilver * Item.amount, 
                        Coin.valueGold * Item.amount
                        );
                }
                else
                {
                    //PlayerInventory.AddItem(Item.item, Item.amount);
                    EventsManager.Instance.pickUpItemEvent.ItemPickUp(Item.item, Item.amount);
                }
                Destroy(currentTarget.gameObject);
            }
            else if (currentTarget.CompareTag("Harvestable"))
            {
                canOpenInventory = false;
                canOpenJournal = false;
                canOpenMap = false;
                canOpenQuestBoard = false;
                canOpenContainer = false;
                canMoveBody = false;
                isHarvesting = true;
                canInteract = false;
                RotateTowardsRessource(currentTarget.transform.position);
                var Item = currentTarget.GetComponent<AttachedItem>();
                EventsManager.Instance.pickUpItemEvent.HarvestPlant(Item);
            }
            else if (currentTarget.CompareTag("Mineral"))
            {
                canOpenInventory = false;
                canOpenJournal = false;
                canOpenMap = false;
                canOpenQuestBoard = false;
                canOpenContainer = false;
                canMoveBody = false;
                isHarvesting = true;
                canInteract = false;
                RotateTowardsRessource(currentTarget.transform.position);
                var item = currentTarget.GetComponent<AttachedItem>();
                EventsManager.Instance.pickUpItemEvent.MineMineral(item);
            }
            else if (currentTarget.CompareTag("Container") && canOpenContainer)
            {
                containerOpen = !containerOpen;
                if (containerOpen)
                {
                    canOpenInventory = false;
                    canOpenJournal = false;
                    canOpenMap = false;
                    canMoveBody = false;
                    canMoveCamera = false;
                    List<InventorySlot> chestInv = currentTarget.GetComponent<ChestInventoryManager>().InventoryList;
                    InventoryManager.Instance.OpenChestInventory(chestInv);
                    InventoryManager.Instance.OpenPlayerInventory();
                    if (currentTarget.GetComponent<Animator>() != null)
                    {
                        currentTarget.GetComponent<Animator>().SetBool("b_open", true);
                        currentTarget.GetComponent<Animator>().SetBool("b_close", false);
                    }
                }
                else
                {
                    canOpenInventory = true;
                    canOpenJournal = true;
                    canOpenMap = true;
                    canMoveBody = true;
                    canMoveCamera = true;
                    InventoryManager.Instance.ClosePlayerInventory();
                    InventoryManager.Instance.CloseChestInventory();
                    if (currentTarget.GetComponent<Animator>() != null)
                    {
                        currentTarget.GetComponent<Animator>().SetBool("b_open", false);
                        currentTarget.GetComponent<Animator>().SetBool("b_close", true);
                    }
                }
                CheckInteractions();
            }
            else if (currentTarget.CompareTag("Door"))
            {
                var animator = currentTarget.GetComponent<Animator>();
                animator.SetTrigger("openclose");
            }
            else if (currentTarget.CompareTag("NPC"))
            {
                canMoveBody = false;
                canMoveCamera = false;
                TextAsset npcDialogue = currentTarget.GetComponent<NpcDialogue>().npcJson;
                string npcName = currentTarget.name;
                DialogueManager.Instance.StartDialogue(npcDialogue, npcName);
                var DialogScript = currentTarget.GetComponent<BasicDialog>();
                //DialogScript.StartDialog(npcDialogue);
                CheckInteractions();
            }
            else if (currentTarget.CompareTag("QuestBoard") && canOpenQuestBoard)
            {
                questBoardOpen = !questBoardOpen;
                if (questBoardOpen)
                {
                    canOpenInventory = false;
                    canOpenJournal = false;
                    canOpenMap = false;
                    canOpenContainer = false;
                    canMoveBody = false;
                    canMoveCamera = false;
                    canInteract = false;
                    QuestManager.Instance.OpenQuestBoardScreen();
                }
                else
                {
                    canOpenInventory = true;
                    canOpenJournal = true;
                    canOpenMap = true;
                    canOpenContainer = true;
                    canMoveBody = true;
                    canMoveCamera = true;
                    canInteract = true;
                    QuestManager.Instance.CloseQuestBoardScreen();
                }
                CheckInteractions();
            }
            else if (currentTarget.CompareTag("Dungeon"))
            {
                currentTarget.GetComponent<EnterExitDungeon>().UsePortal();
            }
        }
    }
    private void ResetScreens()
    {
        inventoryOpen = false;
        canOpenInventory = true;
        journalOpen = false;
        canOpenJournal = true;
        mapOpen = false;
        canOpenMap = true;
        questBoardOpen = false;
        canOpenQuestBoard = true;
        containerOpen = false;
        canOpenContainer = true;

        InventoryManager.Instance.ClosePlayerInventory();
        InventoryManager.Instance.CloseChestInventory();
        QuestManager.Instance.CloseJournalScreen();
        MapScreen.SetActive(false);
        QuestManager.Instance.CloseQuestBoardScreen();
    }
    private void CheckInteractions()
    {
        if (canMoveBody && canMoveCamera)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
    }
    private void PlantHarvested(AttachedItem item)
    {
        canOpenInventory = true;
        canOpenJournal = true;
        canOpenMap = true;
        canOpenQuestBoard = true;
        canOpenContainer = true;
        canMoveBody = true;
        isHarvesting = false;
        canInteract = true;
        //PlayerInventory.AddItem(item.item, item.amount);
        currentTarget.GetComponent<HarvestResource>().Harvest();
    }
    private void MineralMined(AttachedItem item)
    {
        canOpenInventory = true;
        canOpenJournal = true;
        canOpenMap = true;
        canOpenQuestBoard = true;
        canOpenContainer = true;
        canMoveBody = true;
        isHarvesting = false;
        canInteract = true;
        //PlayerInventory.AddItem(item.item, item.amount);
        currentTarget.GetComponent<HarvestResource>().Harvest();
    }
    private void RotateTowardsRessource(Vector3 resPosition)
    {
        Vector3 playerPos = transform.position;
        Vector3 rotateXZ = new Vector3(resPosition.x - playerPos.x, 0, resPosition.z - playerPos.z);
        Quaternion rotation = Quaternion.LookRotation(rotateXZ, Vector3.up);

        float camEulerY = CameraTarget.transform.eulerAngles.y;
        float rayEulerY = RaycastOriginOrigin.transform.eulerAngles.y;

        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 400f);
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 500 * Time.deltaTime);

        CameraTarget.transform.localRotation = Quaternion.Euler(
                CameraTarget.transform.localEulerAngles.x, -transform.eulerAngles.y + camEulerY, 0);
        RaycastOriginOrigin.transform.localRotation = Quaternion.Euler(
            RaycastOriginOrigin.transform.localEulerAngles.x, -transform.eulerAngles.y + rayEulerY, 0);
    }
    private void UpdateInteractText()
    {
        if (currentTarget.tag == "Container")
        {
            if (!currentTarget.GetComponent<ChestInventoryManager>().looted)
            {
                InteractText.text = "Open (E)";
            }
        }
        else if (currentTarget.tag == "NPC")
        {
            InteractText.text = "Speak (E)";
        }
        else if (currentTarget.tag == "Item")
        {
            InteractText.text = "Collect (E)";
        }
        else if (currentTarget.tag == "Door")
        {
            InteractText.text = "Open/Close (E)";
        }
        else if (currentTarget.tag == "Harvestable")
        {
            InteractText.text = "Harvest (E)";
        }
        else if (currentTarget.tag == "SpawnPoint")
        {
            InteractText.text = "Upgrade (E)";
        }
        else if (currentTarget.tag == "QuestBoard")
        {
            InteractText.text = "Look at Quests (E)";
        }
        else if (currentTarget.tag == "Mineral")
        {
            InteractText.text = "Mine (E)";
        }
        else if (currentTarget.tag == "Dungeon")
        {
            InteractText.text = "Use Portal (E)";
        }
        else 
        {
            InteractText.text = "";
        }
    }
    private void OutlineObject()
    {
        if (currentTarget.tag == "Container")
        {
            if (!currentTarget.GetComponent<ChestInventoryManager>().looted)
            {
                currentTarget.GetComponent<Outline>().enabled = true;
            }
        }
        else if (currentTarget.CompareTag("Item"))
        {
            currentTarget.GetComponent<Outline>().enabled = true;
        }
        //change last target to new and remove outline Component
        if (lastTarget != null && lastTarget != currentTarget)
        {
            if (lastTarget.tag == "Container" || lastTarget.tag == "Item")
            {
                lastTarget.transform.gameObject.GetComponent<Outline>().enabled = false;
            }
        }
    }
    private void DialogueFinished(string npcName)
    {
        canMoveBody = true;
        canMoveCamera = true;
        CheckInteractions();
    }
    private void CanPlayerMove(bool canMove)
    {
        this.canMoveBody = canMove;
    }
    private void CheckCanTakeWeapon(bool canTakeWeapon)
    {
        this.canTakeWeapon = canTakeWeapon;
    }
    private void CanPlayerAttack(bool canAttack)
    {
        this.canAttack = canAttack;
    }
}
