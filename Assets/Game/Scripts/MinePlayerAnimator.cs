using UnityEngine;

public class MinePlayerAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Animator _anim;

    [SerializeField] private SpriteRenderer _sprite;

    [Header("Settings")]
    [SerializeField, Range(1f, 3f)]
    private float _maxIdleSpeed = 2;

    [SerializeField] private float _maxTilt = 5;
    [SerializeField] private float _tiltSpeed = 20;

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip[] _footsteps;

    private AudioSource _source;
    private IPlayerController _player;
    private PlayerAttacker _playerAttacker;
    private bool _grounded;
    private ParticleSystem.MinMaxGradient _currentGradient;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        _player = GetComponentInParent<IPlayerController>();
        _playerAttacker = GetComponentInParent<PlayerAttacker>();
    }

    private void OnEnable()
    {
        _player.Jumped += OnJumped;
        _player.GroundedChanged += OnGroundedChanged;
        _playerAttacker.AttackingChanged += OnMeleeAttackChanged;
        EventBus.Instance.playerDied += OnDied;
    }

    private void OnDisable()
    {
        _player.Jumped -= OnJumped;
        _player.GroundedChanged -= OnGroundedChanged;
        _playerAttacker.AttackingChanged -= OnMeleeAttackChanged;
        EventBus.Instance.playerDied -= OnDied;
    }

    private void Update()
    {
        if (_player == null) return;

        HandleSpriteFlip();

        HandleIdleSpeed();

        HandleCharacterTilt();
    }

    private void HandleSpriteFlip()
    {
        if (_player.FrameInput.x != 0) _sprite.flipX = _player.FrameInput.x < 0;
    }

    private void HandleIdleSpeed()
    {
        var inputStrength = Mathf.Abs(_player.FrameInput.x);
        _anim.SetFloat(IdleSpeedKey, Mathf.Lerp(1, _maxIdleSpeed, inputStrength));
    }

    private void HandleCharacterTilt()
    {
        var runningTilt = _grounded ? Quaternion.Euler(0, 0, _maxTilt * _player.FrameInput.x) : Quaternion.identity;
        _anim.transform.up = Vector3.RotateTowards(_anim.transform.up, runningTilt * Vector2.up, _tiltSpeed * Time.deltaTime, 0f);
    }

    private void OnJumped()
    {
        _anim.SetTrigger(JumpKey);
        _anim.ResetTrigger(GroundedKey);

    }

    private void OnGroundedChanged(bool grounded, float impact)
    {
        _grounded = grounded;

        if (grounded)
        {

            _anim.SetTrigger(GroundedKey);
            //_source.PlayOneShot(_footsteps[Random.Range(0, _footsteps.Length)]);
        }
        else
        {
        }
    }
    private void OnDied()
    {
        _anim.SetTrigger(Died);
    }
    private void OnMeleeAttackChanged(bool isAttacking)
    {
        _anim.SetBool(MeleeAttackKey, isAttacking);
    }

    private static readonly int Died = Animator.StringToHash("Died");
    private static readonly int MeleeAttackKey = Animator.StringToHash("MeleeAttack1");
    private static readonly int GroundedKey = Animator.StringToHash("Grounded");
    private static readonly int IdleSpeedKey = Animator.StringToHash("IdleSpeed");
    private static readonly int JumpKey = Animator.StringToHash("Jump");
}