using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static PlayerInput;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : NetworkBehaviour, IPlayerActions
{
    private PlayerInput _playerInput;
    private Vector2 _moveInput = new();
    private Vector2 _cursorLocation;

    private Transform _shipTransform;
    private Rigidbody2D _rb;

    private Transform turretPivotTransform;

    private float _SpeedBuffAccumulator;

    public UnityAction<bool> onFireEvent;

    [HideInInspector] public NetworkVariable<int> _RespawnLeft = new NetworkVariable<int>(5);

    public UnityAction<bool> OnRespawnEvent;

    [Header("Settings")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float shipRotationSpeed = 100f;
    [SerializeField] private float turretRotationSpeed = 4f;


    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        if (_playerInput == null)
        {
            _playerInput = new();
            _playerInput.Player.SetCallbacks(this);
        }
        _playerInput.Player.Enable();

        _rb = GetComponent<Rigidbody2D>();
        _shipTransform = transform;
        turretPivotTransform = transform.Find("PivotTurret");
        if (turretPivotTransform == null) Debug.LogError("PivotTurret is not found", gameObject);
    }





    public void OnFire(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onFireEvent.Invoke(true);
        }
        else if (context.canceled)
        {
            onFireEvent.Invoke(false);
        }
    }

    public void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        if (IsServer && _SpeedBuffAccumulator > float.Epsilon)
        {
            _SpeedBuffAccumulator -= Time.deltaTime;
            if (_SpeedBuffAccumulator <= float.Epsilon)
            {
                _SpeedBuffAccumulator = 0f;
                ResetMovementSpeed();
            }
        }
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        _rb.velocity = transform.up * _moveInput.y * movementSpeed;
        _rb.MoveRotation(_rb.rotation + _moveInput.x * -shipRotationSpeed * Time.fixedDeltaTime);
    }
    private void LateUpdate()
    {
        if (!IsOwner) return;
        Vector2 screenToWorldPosition = Camera.main.ScreenToWorldPoint(_cursorLocation);
        Vector2 targetDirection = new Vector2(screenToWorldPosition.x - turretPivotTransform.position.x, screenToWorldPosition.y - turretPivotTransform.position.y).normalized;
        Vector2 currentDirection = Vector2.Lerp(turretPivotTransform.up, targetDirection, Time.deltaTime * turretRotationSpeed);
        turretPivotTransform.up = currentDirection;
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        _cursorLocation = context.ReadValue<Vector2>();
    }

    public void _SpeedBuff()
    {
        movementSpeed = 6.5f;
        _SpeedBuffAccumulator = 2.5f;
    }

    private void ResetMovementSpeed()
    {
        movementSpeed = 5f;
    }

    public void HandleDie()
    {
        if (IsServer)
        {
            if (_RespawnLeft.Value <= 0)
            {
                NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
                networkObject.Despawn();
            }
            else
            {
                _RespawnLeft.Value--;
                gameObject.transform.position = new Vector3(0, 0, 0);
                gameObject.transform.rotation = Quaternion.identity;
                OnRespawnEvent.Invoke(true);
            }
        }
    }

}
