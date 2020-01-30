using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour, IDamageable
{
    #region Private Members

    private Animator _animator;

    private InventoryItemBase mCurrentItem = null;

    [SerializeField]
    private TestInjuredEffect mHealthBar;

    public FloatReference Hitpoints;

    [SerializeField]
    private FloatReference HungerValue;

    private HealthBar mFoodBar;

    private int startHealth;

    private int startFood;

    #endregion

    #region Public Members

    public Inventory Inventory;

    public GameObject Hand;

    public HUD Hud;

    public event EventHandler PlayerDied;

    #endregion

    // Use this for initialization
    void Start()
    {
        _animator = GetComponent<Animator>();
        Inventory.ItemUsed += Inventory_ItemUsed;
        Inventory.ItemRemoved += Inventory_ItemRemoved;

        //mHealthBar = Hud.transform.Find("Bars_Panel/HealthBar").GetComponent<HealthBar>();
        //mHealthBar.Min = 0;
        //mHealthBar.Max = Health;
        //startHealth = Health;
        //mHealthBar.SetValue(Health);

        //mFoodBar = Hud.transform.Find("Bars_Panel/FoodBar").GetComponent<HealthBar>();
        //mFoodBar.Min = 0;
        //mFoodBar.Max = Food;
        //startFood = Food;
        //mFoodBar.SetValue(Food);

        /// <summary>
        /// 2/27
        /// </summary>

        CameraR = Camera.main.transform;
        forwardInput = turnInput = 0;
        rigidbody = GetComponent<Rigidbody>();

        /// <summary>
        /// 2/27
        /// </summary>
        InvokeRepeating("IncreaseHunger", 0, HungerRate);
    }
      

    #region Inventory

    private void Inventory_ItemRemoved(object sender, InventoryEventArgs e)
    {
        InventoryItemBase item = e.Item;
        GameObject goItem = (item as MonoBehaviour).gameObject;
        goItem.SetActive(true);
        goItem.transform.parent = null;
    }

    private void SetItemActive(InventoryItemBase item, bool active)
    {
        GameObject currentItem = (item as MonoBehaviour).gameObject;
        currentItem.SetActive(active);
        currentItem.transform.parent = active ? Hand.transform : null;
    }

    private void Inventory_ItemUsed(object sender, InventoryEventArgs e)
    {
        if (e.Item.ItemType != EItemType.Consumable)
        {
            // If the player carries an item, un-use it (remove from player's hand)
            if (mCurrentItem != null)
            {
                SetItemActive(mCurrentItem, false);
            }

            InventoryItemBase item = e.Item;

            // Use item (put it to hand of the player)
            SetItemActive(item, true);

            mCurrentItem = e.Item;
        }

    }

    private int Attack_1_Hash = Animator.StringToHash("Base Layer.Attack_1");

    public bool IsAttacking
    {
        get
        {
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.fullPathHash == Attack_1_Hash)
            {
                return true;
            }
            return false;
        }
    }

    public void DropCurrentItem()
    {
        _animator.SetTrigger("tr_drop");

        GameObject goItem = (mCurrentItem as MonoBehaviour).gameObject;

        Inventory.RemoveItem(mCurrentItem);

        // Throw animation
        Rigidbody rbItem = goItem.AddComponent<Rigidbody>();
        if (rbItem != null)
        {
            rbItem.AddForce(transform.forward * 2.0f, ForceMode.Impulse);

            Invoke("DoDropItem", 0.25f);
        }
    }

    public void DropAndDestroyCurrentItem()
    {
        Inventory.RemoveItem(mCurrentItem);

        GameObject goItem = (mCurrentItem as MonoBehaviour).gameObject;

        Destroy(goItem);

        mCurrentItem = null;
    }

    public void DoDropItem()
    {

        // Remove Rigidbody
        Destroy((mCurrentItem as MonoBehaviour).GetComponent<Rigidbody>());

        mCurrentItem = null;
    }

    #endregion

    #region Health & Hunger

    //[Tooltip("Amount of health")]
    //public int Health = 100;

    //[Tooltip("Amount of food")]
    //public int Food = 100;

    [Tooltip("Rate in seconds in which the hunger increases")]
    public float HungerRate = 0.5f;

    public void IncreaseHunger()
    {
        HungerValue.Value--;

        if (HungerValue.Value == 0)
        {
            CancelInvoke();
            Die();
        }
    }

    public bool IsDead
    {
        get
        {
            return Hitpoints.Value == 0 || HungerValue.Value == 0;
        }
    }

    public bool CarriesItem(string itemName)
    {
        if (mCurrentItem == null)
            return false;

        return (mCurrentItem.Name == itemName);
    }

    public bool IsArmed
    {
        get
        {
            if (mCurrentItem == null)
                return false;

            return mCurrentItem.ItemType == EItemType.Weapon;
        }
    }


    public void Eat(int amount)
    {
        HungerValue.Value += amount;
    }

    public void Rehab(int amount)
    {
        Hitpoints.Value += amount;
    }

    public void TakeDamage(int amount)
    {
        Hitpoints.Value -= amount;
        mHealthBar.damaged = true;

        if (Hitpoints.Value == 0)
        {
            Die();
        }
    }

    private void Die()
    {
        _animator.SetTrigger("death");

        if(PlayerDied != null)
        {
            PlayerDied(this, EventArgs.Empty);
        }
    }

    #endregion


    public void Talk()
    {
        _animator.SetTrigger("tr_talk");
    }

    private bool mIsControlEnabled = true;

    public void EnableControl()
    {
        mIsControlEnabled = true;
    }

    public void DisableControl()
    {
        mIsControlEnabled = false;
    }

    /// <summary>
    /// 2/27
    /// </summary>
    #region move
    public float mouseSensitivityX = 250f;
    public float mouseSensitivityY = 250f;
    public float inputDelay = 0.1f;

    private Transform CameraR;
    private float VerticalLookR;
    private float mouseXInput;
    private float mouseYInput;

    public float moveSpeed = 5.0f;
    private Vector3 moveDir;
    private Vector3 targetAmount;
    private Vector3 moveAmount;
    private float forwardInput;
    private float turnInput;
    private Vector3 smoothMoveVel;
    public float jumpForce = 220;

    private bool isGrounded;
    public LayerMask groundMask;

    private new Rigidbody rigidbody;
    #endregion
    /// <summary>
    /// 2/27
    /// </summary>


    

    // Update is called once per frame
    void Update()
    {
        if (!IsDead && mIsControlEnabled)
        {
            
            // Interact with the item
            if (mInteractItem != null && Input.GetKeyDown(KeyCode.F))
            {
                // Interact animation
                mInteractItem.OnInteractAnimation(_animator);
            }

            // Execute action with item
            if (mCurrentItem != null && Input.GetMouseButtonDown(0))
            {
                // Dont execute click if mouse pointer is over uGUI element
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    // TODO: Logic which action to execute has to come from the particular item
                    _animator.SetTrigger("attack_1");
                }
            }

            /// <summary>
            /// 2/27
            /// </summary>
            GetInput();
            LookControl();
            /// <summary>
            /// 2/27
            /// </summary>
            /// 
        }
        if (IsDead)
        {
            Die();
        }
    }

    void FixedUpdate()
    {
        /// <summary>
        /// 2/27
        /// </summary>

        Move();
        
        /// <summary>
        /// 2/27
        /// </summary>
        
        if (!IsDead)
        {
            // Drop item
            if (mCurrentItem != null && Input.GetKeyDown(KeyCode.R))
            {
                DropCurrentItem();
            }
        }
    }

    private void LookControl()
    {
        VerticalLookR = Mathf.Clamp(VerticalLookR, -60, 60);
        CameraR.localEulerAngles = Vector3.left * VerticalLookR;
    }

    private void GetInput()
    {
        mouseXInput = Input.GetAxis("Mouse X");
        mouseYInput = Input.GetAxis("Mouse Y");
        forwardInput = Input.GetAxisRaw("Vertical");
        turnInput = Input.GetAxisRaw("Horizontal");

        transform.Rotate(Vector3.up * mouseXInput * Time.deltaTime * mouseSensitivityX);
        VerticalLookR += mouseYInput * Time.deltaTime * mouseSensitivityY;

        moveDir = new Vector3(turnInput * 0.4f, 0, forwardInput).normalized;
        targetAmount = moveDir * moveSpeed;
        moveAmount = Vector3.SmoothDamp(moveAmount, targetAmount, ref smoothMoveVel, 0.15f);

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
        else
        {
            _animator.SetBool("is_in_air", false);
        }
        isGrounded = false;
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 0.1f, groundMask))
        {           
            isGrounded = true;
        }
        // keyboard input;
    }

    private void Jump()
    {
        if (isGrounded)
        {
            _animator.SetBool("is_in_air", true);
            rigidbody.AddForce(transform.up * jumpForce);
        }
    }

    private void Move()
    {
        rigidbody.MovePosition(rigidbody.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
        _animator.SetBool("run", targetAmount.magnitude>0);
    }

    public void InteractWithItem()
    {
        if (mInteractItem != null)
        {
            mInteractItem.OnInteract();

            if (mInteractItem is InventoryItemBase)
            {
                InventoryItemBase inventoryItem = mInteractItem as InventoryItemBase;
                Inventory.AddItem(inventoryItem);
                inventoryItem.OnPickup();

                if (inventoryItem.UseItemAfterPickup)
                {
                    Inventory.UseItem(inventoryItem);
                }
            }
        }

        Hud.CloseMessagePanel();

        mInteractItem = null;
    }

    private InteractableItemBase mInteractItem = null;

    /// <summary>
    /// Finding the Plane
    /// </summary>
    private PlayerFlightControl mPlayerFlight = null;
    public bool isOnPlane = false;

    private void OnTriggerEnter(Collider other)
    {
        InteractableItemBase item = other.GetComponent<InteractableItemBase>();

        PlayerFlightControl plane = other.GetComponent<PlayerFlightControl>();

        if (item != null)
        {
            if (item.CanInteract(other))
            {

                mInteractItem = item;

                Hud.OpenMessagePanel(mInteractItem);
            }
        }

        if(plane != null)
        {
            mPlayerFlight = plane;
        }
       
    }

    /// <summary>
    /// 切换控制将主角换为飞船
    /// </summary>
    /// <param name="other"> </param>
    /// 
    private void DEBUG(string a)
    {
        Debug.Log(a);
    }

    private void OnTriggerStay(Collider other)
    {
        
        if (!isOnPlane && mPlayerFlight)
        {
            DEBUG("player is near flight");
            InteractableItemBase item = other.GetComponent<InteractableItemBase>();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                GameManager.Instance.GetOnPlane();
                CameraManager.Instance.CameraState = CameraManager.CameraPosi.Flight;

                Hud.CloseMessagePanel();
                mInteractItem = null;  ///关掉消息面板 并且清空消息队列

                isOnPlane = true;
                GameManager.Instance.Flight.GetComponent<PlayerFlightControl>().canLand = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        InteractableItemBase item = other.GetComponent<InteractableItemBase>();
        PlayerFlightControl plane = other.GetComponent<PlayerFlightControl>();

        if (item != null)
        {
            Hud.CloseMessagePanel();
            mInteractItem = null;
        }

        if(plane != null)
        {
            mPlayerFlight = null;
        }
    }
}
