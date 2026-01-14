using Weapons;

public class EnemyComponent : ActorComponent
{
    private MovementComponent _movementComponent;
    public  EnemyData         enemyData;

    public  SimpleWeaponData      weaponData;
    private SimpleWeaponBehaviour _weaponBehaviour;

    private bool _isInitialized;

    public void Start()
    {
        _movementComponent = GetComponent<MovementComponent>();

        var enemySpawnAnimation = GetComponent<EnemySpawnAnimationComponent>();
        if (enemySpawnAnimation)
        {
            enemySpawnAnimation.OnSpawnComplete += () =>
            {
                _isInitialized             = true;
                _movementComponent.canMove = true;
            };
        }
        else
        {
            _isInitialized = true;
        }
        
        if (!weaponData || enemyData.Threat != EnemyData.ThreatLevel.Advanced) return;

        var behaviour = weaponData.AttachWeapon(gameObject, this);
        _weaponBehaviour = behaviour as SimpleWeaponBehaviour;
    }

    public void Update()
    {
        if (!_isInitialized || !Game.Player) return;

        var moveDirection = (Game.Player.transform.position - transform.position).normalized;

        _movementComponent.SetMovementDirection(moveDirection);
        _movementComponent.SetSpeed(enemyData.MoveSpeed);

        if (_weaponBehaviour)
            _weaponBehaviour.UpdateWeapon(transform.position, moveDirection);
    }

    protected override void OnDeath()
    {
        Game.AudioManager.PlaySoundAtPosition(enemyData.DeathSound, transform.position);
        Game.GetEnemyPool(enemyData).Release(this);
        Game.Enemies.Remove(this);
        _isInitialized = false;
    }
}