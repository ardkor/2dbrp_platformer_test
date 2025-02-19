using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerController : MonoBehaviour, IPlayerController
{
    [SerializeField] private ScriptableStats _stats;
    private Rigidbody2D _rb;
    private CapsuleCollider2D _col;
    private FrameInput _frameInput;
    private Vector2 _frameVelocity;
    private bool _cachedQueryStartInColliders;

    public Vector2 FrameInput => _frameInput.Move;
    public event Action<bool, float> GroundedChanged;
    public event Action Jumped;

    private float _time;
    private bool _isAttacking;
    private PlayerAttacker _playerAttacker;
    private bool _flipX;
    private bool _inputEnabled;
    private bool _knockbackEnabled;

    private void Awake()
    {
        _playerAttacker = GetComponent<PlayerAttacker>();
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<CapsuleCollider2D>();

        _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
        _inputEnabled = true;
        _knockbackEnabled = true;
    }
    private void OnEnable()
    {
        _playerAttacker.AttackingChanged += ChangeAttacking;
        EventBus.Instance.playerDied += DisableInput;
        EventBus.Instance.gameFinished += DisableInput;
    }

    private void OnDisable()
    {
        _playerAttacker.AttackingChanged -= ChangeAttacking;
        EventBus.Instance.playerDied -= DisableInput;
        EventBus.Instance.gameFinished -= DisableInput;
    }

    private void Update()
    {
        _time += Time.deltaTime;
        if (_inputEnabled && _knockbackEnabled)
        {
            GatherInput();
            HandleSpriteFlip();
        }
        _playerAttacker.UpdateFlipped(_flipX);
    }
    private void DisableInput()
    {
        _inputEnabled = false;
    }
    public void DisableMovementForKnockback(float knockbackDuration)
    {
        StartCoroutine(DisableMovement(knockbackDuration/2));
    }

    private IEnumerator DisableMovement(float duration)
    {
        _knockbackEnabled = false;
        yield return new WaitForSeconds(duration);
        _knockbackEnabled = true;
    }

    private void GatherInput()
    {

        if (Input.GetButtonDown("Fire1") && !_isAttacking)
        {
            TryStartAttack();
        }
        _frameInput = new FrameInput
        {
            JumpDown = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.C),
            JumpHeld = Input.GetButton("Jump") || Input.GetKey(KeyCode.C),
            ShiftHeld = Input.GetKey(KeyCode.LeftShift),
            Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))
        };

        if (_stats.SnapInput)
        {
            _frameInput.Move.x = Mathf.Abs(_frameInput.Move.x) < _stats.HorizontalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.x);
            _frameInput.Move.y = Mathf.Abs(_frameInput.Move.y) < _stats.VerticalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.y);
        }

        if (_frameInput.JumpDown)
        {
            _jumpToConsume = true;
            _timeJumpWasPressed = _time;
        }

    }

    private void HandleSpriteFlip()
    {
        if (FrameInput.x != 0) _flipX = FrameInput.x < 0;
    }

    private void FixedUpdate()
    {
        CheckCollisions();
        HandleGravity();

        if (_inputEnabled && _knockbackEnabled)
        {
            HandleJump();
            HandleDirection();
            ApplyMovement();
        }
        else 
            ApplyGravity();
    }
    private void ChangeAttacking(bool isAttacking)
    {
        _isAttacking = isAttacking;
    }
    private void TryStartAttack()
    {
        if (_grounded)
        {
            _playerAttacker.PerformAttack();
        }
    }

    #region Collisions

    private float _frameLeftGrounded = float.MinValue;
    private bool _grounded;

    private void CheckCollisions()
    {
        Physics2D.queriesStartInColliders = false;

        // Ground and Ceiling
        bool groundHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.down, _stats.GrounderDistance, ~_stats.PlayerLayer);
        bool ceilingHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.up, _stats.GrounderDistance, ~_stats.PlayerLayer);
        //Debug.DrawLine(_col.bounds.center, _col.bounds.center - Vector3.down * _stats.GrounderDistance);

        // Hit a Ceiling
        if (ceilingHit) _frameVelocity.y = Mathf.Min(0, _frameVelocity.y);

        // Landed on the Ground
        if (!_grounded && groundHit)
        {
            SoundManager.Instance.PlaySound(SoundManager.landingSound);
            _grounded = true;
            _coyoteUsable = true;
            _bufferedJumpUsable = true;
            _endedJumpEarly = false;
            GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));
        }
        // Left the Ground
        else if (_grounded && !groundHit)
        {
            _grounded = false;
            _frameLeftGrounded = _time;
            GroundedChanged?.Invoke(false, 0);
        }

        Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
    }

    #endregion


    #region Jumping

    private bool _jumpToConsume;
    private bool _bufferedJumpUsable;
    private bool _endedJumpEarly;
    private bool _coyoteUsable;
    private float _timeJumpWasPressed;

    private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + _stats.JumpBuffer;
    private bool CanUseCoyote => _coyoteUsable && !_grounded && _time < _frameLeftGrounded + _stats.CoyoteTime;

    private void HandleJump()
    {
        if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rb.velocity.y > 0) _endedJumpEarly = true;

        if (!_jumpToConsume && !HasBufferedJump) return;

        if (_grounded || CanUseCoyote) ExecuteJump();

        _jumpToConsume = false;
    }

    private void ExecuteJump()
    {
        _endedJumpEarly = false;
        _timeJumpWasPressed = 0;
        _bufferedJumpUsable = false;
        _coyoteUsable = false;
        _frameVelocity.y = _stats.JumpPower;
        Jumped?.Invoke();
        SoundManager.Instance.PlaySound(SoundManager.jumpSound);
    }

    #endregion

    #region Horizontal

    private void HandleDirection()
    {
        if (_frameInput.Move.x == 0)
        {
            var deceleration = _grounded ? _stats.GroundDeceleration : _stats.AirDeceleration;
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
        }
        else
        {
            var accelerationMultiplayer = 1f;
            if (_frameInput.ShiftHeld)
            {
                SoundManager.Instance.PlaySound(SoundManager.accelerateSound);
                accelerationMultiplayer = _stats.AccelerationMultiplayer;
            }  
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, _frameInput.Move.x * _stats.MaxSpeed * accelerationMultiplayer, _stats.Acceleration * Time.fixedDeltaTime * accelerationMultiplayer);
        }
    }

    #endregion

    #region Gravity

    private void HandleGravity()
    {
        if (_grounded && _frameVelocity.y <= 0f)
        {
            _frameVelocity.y = _stats.GroundingForce;
        }
        else
        {
            var inAirGravity = _stats.FallAcceleration;
            if (_endedJumpEarly && _frameVelocity.y > 0) inAirGravity *= _stats.JumpEndEarlyGravityModifier;
            _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -_stats.MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
        }
    }

/*    private Vector2 _addedVelocity;
    public void AddVelocity(Vector2 velocity)
    {
        _addedVelocity = velocity;
    }*/
    #endregion

    private void ApplyMovement() => _rb.velocity = _frameVelocity;
    /*{
        if (_addedVelocity.magnitude > 0)
        {
            _rb.velocity = _addedVelocity + _frameVelocity; //new Vector2(_addedVelocity.x + _frameVelocity.x, _rb.velocity.y + _frameVelocity.y);
        }
        else
        {
            _rb.velocity = _frameVelocity;
            _addedVelocity = new Vector2(0, 0);
        }
    }*/
    private void ApplyGravity() => _rb.velocity = new Vector2(_rb.velocity.x, _frameVelocity.y);
    //private void ApplyAddedVelocity() => _rb.velocity = _addedVelocity + _frameVelocity;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_stats == null) Debug.LogWarning("Please assign a ScriptableStats asset to the Player Controller's Stats slot", this);
    }
#endif
}

public struct FrameInput
{
    public bool JumpDown;
    public bool JumpHeld;
    public bool ShiftHeld;
    public Vector2 Move;
}

public interface IPlayerController
{
    public event Action<bool, float> GroundedChanged;

    public event Action Jumped;
    public Vector2 FrameInput { get; }
}