using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine
using UnityEngine.SceneManagement;

public class FirstPersonController : MonoBehaviour
{
    public bool CanMove { get; private set; } = true;
    public bool ShouldJump => Input.GetKeyDown(jumpKey) && controller.isGrounded;
    public bool IsSprinting => canSprint && Input.GetKey(sprintKey);//

    [Header("Funcionalidad")]
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canHeadBob = true;
    [SerializeField] private bool canSprint = true;//

    [Header("Controles")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;//

    [Header("Parametros de movimiento")]
    [SerializeField] private float walkSpeed = 6.0f;
    [SerializeField] private float sprintSpeed = 12.0f;
    [SerializeField] private float jumpForce = 10.0f;
    [SerializeField] private float gravity = 30.0f;

    [Header("Parametros de mirar")]
    [SerializeField, Range(1, 10)] private float lookSpeedX = 2.0f;
    [SerializeField, Range(1, 10)] private float lookSpeedY = 2.0f;
    [SerializeField, Range(1, 180)] private float upperLookLimit = 80.0f;
    [SerializeField, Range(1, 180)] private float lowerLookLimit = 80.0f;

    [Header("Paremetros de headbob")]
    [SerializeField] private float walkBobSpeed = 14f;
    [SerializeField] private float walkBobAmount = 0.05f;
    [SerializeField] private float sprintBobSpeed = 18f;
    [SerializeField] private float sprintBobAmount = 0.1f;
    private float defaultYPos = 0;
    private float timer;

    [Header("Parametros de stamina")]
    static public float maxStamina = 100f;
    [SerializeField] private float staminaUseMultiplier = 5f;
    [SerializeField] private float timeBeforeStaminaRegenStarts = 5f;
    [SerializeField] private float staminaValueIncrement = 2f;
    [SerializeField] private float staminaTimeIncrement = 0.1f;

    private float currentStamina;
    private Coroutine regerantingStamina;
    public static Action<float> OnStaminaChange;

    [Header("Parametros de vida")]
    static public float maxHealth = 100f;
    [SerializeField] private float timeBeforeHealthRegenStarts = 3f;
    [SerializeField] private float healthValueIncrement = 1f;
    [SerializeField] private float healthTimeIncrement = 0.1f;
    private float currentHealth;
    private Coroutine regeneratingHealth;
    public static Action<float> OnTakeDamage;
    public static Action<float> OnDamage;
    public static Action<float> OnHeal;

    private Camera playerCamera;
    private CharacterController controller;

    private Vector3 moveDirection;
    private Vector2 currentInput;

    private float rotationX = 0.0f;


void Awake()
    {
        //Caching!
        playerCamera = GetComponentInChildren<Camera>();
        controller = GetComponent<CharacterController>();
        defaultYPos = playerCamera.transform.localPosition.y;
        currentStamina = maxStamina;

        OnStaminaChange?.Invoke(currentStamina);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (CanMove)
        {
            //Controlar el input
            HandleMovementInput();
            //Controlar la camara
            HandleMouseLook();
            //Saltar? ...
            if (canJump)
                HandleJump();

            if (canHeadBob)
                HandleHeadbob();

            HandleStamina();
            //Aplicar los movimientos
            ApplyFinalMovements();
        }
    }
    void TakeDamage(float damage)
    {
        currentHealth -= damage;
        OnDamage?.Invoke(currentHealth);

        if (currentHealth <= 0)
            Debug.Log("Fracasado");
        else if (regeneratingHealth != null)
            StopCoroutine(regeneratingHealth);


        regeneratingHealth = StartCoroutine(RegenerateHealth());
    }
    IEnumerator RegenerateHealth()
    {
        yield return new WaitForSeconds(timeBeforeStaminaRegenStarts);
        WaitForSeconds timeToWait = new WaitForSeconds(staminaTimeIncrement);
        while (currentHealth < maxHealth)
        {

            currentHealth += healthValueIncrement;

            if (currentHealth > maxHealth)
                currentHealth = maxHealth;
            OnHeal?.Invoke(currentHealth);
            yield return timeToWait;
        }
        regeneratingHealth = null;
    }
    void HandleStamina()
    {
        if (IsSprinting && currentInput != Vector2.zero)
        {
            if (regerantingStamina != null)
            {
                StopCoroutine(regerantingStamina);
                regerantingStamina = null;
            }

            currentStamina -= staminaUseMultiplier * Time.deltaTime;

            if (currentStamina < 0)
                currentStamina = 0;

            OnStaminaChange?.Invoke(currentStamina);

            if (currentStamina <= 0)
                canSprint = false;
        }
        if (!IsSprinting && currentStamina < maxStamina && regerantingStamina == null)
        {
            regerantingStamina = StartCoroutine(RegenerateStamina());
        }
    }
    IEnumerator RegenerateStamina()
    {
        yield return new WaitForSeconds(timeBeforeStaminaRegenStarts);
        WaitForSeconds timeToWait = new WaitForSeconds(staminaTimeIncrement);
        while (currentStamina < maxStamina)
        {
            if (currentStamina > 0)
                canSprint = true;

            currentStamina += staminaValueIncrement;

            if (currentStamina > maxStamina)
                currentStamina = maxStamina;

            OnStaminaChange?.Invoke(currentStamina);

            yield return timeToWait;
        }
        regerantingStamina = null;
    }
    void HandleHeadbob()
    {
        if (!controller.isGrounded)
            return;

        if (Mathf.Abs(moveDirection.x) > 0.1f || Mathf.Abs(moveDirection.z) > 0.1f)
        {
            float bobSpeed = IsSprinting ? sprintBobSpeed : walkBobSpeed;
            float bobAmount = IsSprinting ? sprintBobAmount : walkBobAmount;

            timer += Time.deltaTime * bobSpeed;
            playerCamera.transform.localPosition = new Vector3(
                playerCamera.transform.localPosition.x,
                defaultYPos + Mathf.Sin(timer) * bobAmount,
                playerCamera.transform.localPosition.z);
        }
    }
    void HandleMovementInput()
    {
        //Turnary operator
        float speed = IsSprinting ? sprintSpeed : walkSpeed;

        currentInput = new Vector2(speed * Input.GetAxis("Vertical"),
            speed * Input.GetAxis("Horizontal"));

        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) +
            (transform.TransformDirection(Vector3.right) * currentInput.y);
        moveDirection.y = moveDirectionY;
    }
    void HandleMouseLook()
    {
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeedX, 0);
    }
    void HandleJump()
    {
        if (ShouldJump)
            moveDirection.y = jumpForce;
    }
    void ApplyFinalMovements()
    {
        if (!controller.isGrounded)
            moveDirection.y -= gravity * Time.deltaTime;

        controller.Move(moveDirection * Time.deltaTime);
    }
}