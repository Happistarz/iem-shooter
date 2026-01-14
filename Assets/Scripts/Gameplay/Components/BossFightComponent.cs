using UnityEngine;
using Weapons;

public class BossFightComponent : ActorComponent
{
    private MovementComponent _movementComponent;

    // weapon
    public  WeaponData          bossWeapon;
    private BossWeaponBehaviour _bossWeaponBehaviour;

    public  float spawnInterval            = 0.7f;
    public  float spawnIntervalSecondPhase = 1.5f;
    private float _spawnTimer;

    [Header("Audio")]
    public AudioSource audioSource;

    public AudioClip spawnClip;
    public AudioClip phaseChangeClip;

    private enum FightPhase
    {
        PhaseOne, // Immobile, Spawner
        PhaseTwo  // Mobile, Shooter
    }

    private bool _isInitialized;

    private FightPhase _currentPhase = FightPhase.PhaseOne;

    private void OnEnable()
    {
        audioSource.PlayOneShot(spawnClip);
    }

    protected override void Awake()
    {
        base.Awake();

        _movementComponent = GetComponent<MovementComponent>();

        var spawnAnimation = GetComponent<EnemySpawnAnimationComponent>();

        if (spawnAnimation) spawnAnimation.OnSpawnComplete += () => { _isInitialized = true; };

        // Initialize and set up the boss weapon
        if (!bossWeapon) return;
        _bossWeaponBehaviour = bossWeapon.AttachWeapon(gameObject, this) as BossWeaponBehaviour;

        if (!_bossWeaponBehaviour) return;
        _bossWeaponBehaviour.OnShoot += () =>
        {
            // You can add shooting sound effects or other effects here
        };
    }

    private void Update()
    {
        if (!Game.Player)
        {
            _movementComponent.SetMovementDirection(Vector3.zero);
            return;
        }

        var playerPosition    = Game.Player.transform.position;
        var directionToPlayer = (playerPosition - transform.position).normalized;

        _movementComponent.SetMovementDirection(directionToPlayer);

        if (directionToPlayer != Vector3.zero)
        {
            var rotation = Quaternion.LookRotation(directionToPlayer, Vector3.up);
            transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
        }

        if (!_isInitialized) return;

        switch (_currentPhase)
        {
            case FightPhase.PhaseOne:
                UpdatePhaseOne();
                break;
            case FightPhase.PhaseTwo:
                UpdatePhaseTwo(directionToPlayer);
                break;
        }
    }

    private void UpdatePhaseOne()
    {
        _movementComponent.canMove = false;

        _spawnTimer += Time.deltaTime;
        if (_spawnTimer >= spawnInterval)
        {
            _spawnTimer = 0.0f;
            GameLoop.Instance.SpawnBossEnemy();
        }

        if (health > maxHealth / 2) return;

        // Transition to Phase Two
        _currentPhase              = FightPhase.PhaseTwo;
        _movementComponent.canMove = true;
        audioSource.PlayOneShot(phaseChangeClip);
    }

    private void UpdatePhaseTwo(Vector3 directionToPlayer)
    {
        _spawnTimer += Time.deltaTime;
        if (_spawnTimer >= spawnIntervalSecondPhase)
        {
            _spawnTimer = 0.0f;
            GameLoop.Instance.SpawnBossEnemy();
        }
        
        // The Boss moves towards the player and shoots
        _bossWeaponBehaviour.UpdateWeapon(transform.position, directionToPlayer);
    }

    protected override void OnDeath()
    {
        // Game.AudioManager.PlaySoundAtPosition(Game.Data.BossDeathSound, transform.position);
        GameLoop.Instance.OnBossDefeated();

        // Disable self component
        enabled = false;
    }
}