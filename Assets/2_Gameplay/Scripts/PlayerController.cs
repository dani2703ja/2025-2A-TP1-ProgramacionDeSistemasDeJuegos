/*using Gameplay;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay
{
    [RequireComponent(typeof(Character))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private InputActionReference moveInput;
        [SerializeField] private InputActionReference jumpInput;
        [SerializeField] private float airborneSpeedMultiplier = .5f;
        //TODO: This booleans are not flexible enough. If we want to have a third jump or other things, it will become a hazzle.
        private bool _isJumping;
        private bool _isDoubleJumping;
        private Character _character;
        private Coroutine _jumpCoroutine;

        private void Awake()
            => _character = GetComponent<Character>();

        private void OnEnable()
        {
            if (moveInput?.action != null)
            {
                moveInput.action.started += HandleMoveInput;
                moveInput.action.performed += HandleMoveInput;
                moveInput.action.canceled += HandleMoveInput;
            }
            if (jumpInput?.action != null)
                jumpInput.action.performed += HandleJumpInput;
        }
        private void OnDisable()
        {
            if (moveInput?.action != null)
            {
                moveInput.action.performed -= HandleMoveInput;
                moveInput.action.canceled -= HandleMoveInput;
            }
            if (jumpInput?.action != null)
                jumpInput.action.performed -= HandleJumpInput;
        }

        private void HandleMoveInput(InputAction.CallbackContext ctx)
        {
            var direction = ctx.ReadValue<Vector2>().ToHorizontalPlane();
            if (_isJumping || _isDoubleJumping)
                direction *= airborneSpeedMultiplier;
            _character?.SetDirection(direction);
        }

        private void HandleJumpInput(InputAction.CallbackContext ctx)
        {
            //TODO: This function is barely readable. We need to refactor how we control the jumping
            if (_isJumping)
            {
                if (_isDoubleJumping)
                    return;
                RunJumpCoroutine();
                _isDoubleJumping = true;
                return;
            }
            RunJumpCoroutine();
            _isJumping = true;
        }

        private void RunJumpCoroutine()
        {
            if (_jumpCoroutine != null)
                StopCoroutine(_jumpCoroutine);
            _jumpCoroutine = StartCoroutine(_character.Jump());
        }

        private void OnCollisionEnter(Collision other)
        {
            foreach (var contact in other.contacts)
            {
                if (Vector3.Angle(contact.normal, Vector3.up) < 5)
                {
                    _isJumping = false;
                    _isDoubleJumping = false;
                }
            }
        }
    }
}*/


using Gameplay;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay
{
    [RequireComponent(typeof(Character))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private InputActionReference moveInput;
        [SerializeField] private InputActionReference jumpInput;
        [SerializeField] private float airborneSpeedMultiplier = .5f;
        [SerializeField] private int maxCount; // generamos la logica del maximo del contador


        //TODO: This booleans are not flexible enough. If we want to have a third jump or other things, it will become a hazzle.
        //Añado la logica de el contador de saltos
        //Añado la logica de los estados
        private Character _character;
        private Coroutine _jumpCoroutine;
        private int _jumpCount;
        private bool _isJumping;
        private PlayerStateMachine _stateMachine;
        private IPlayerState _groundedState;
        private IPlayerState _airborneState;



        //Inizialiso los estados del player y la maquina de estados
        private void Awake()
        {
            _character = GetComponent<Character>();
            _stateMachine = new PlayerStateMachine();
            _groundedState = new GroundedState(this);
            _airborneState = new AirborneState(this);

        }


        private void Update()
        {
            _stateMachine.Update();
        }

        private void OnEnable()
        {
            if (moveInput?.action != null)
            {
                moveInput.action.started += HandleMoveInput;
                moveInput.action.performed += HandleMoveInput;
                moveInput.action.canceled += HandleMoveInput;
            }
            if (jumpInput?.action != null)
                jumpInput.action.performed += HandleJumpInput;
        }
        private void OnDisable()
        {
            if (moveInput?.action != null)
            {
                moveInput.action.performed -= HandleMoveInput;
                moveInput.action.canceled -= HandleMoveInput;
            }
            if (jumpInput?.action != null)
                jumpInput.action.performed -= HandleJumpInput;
        }

        private void HandleMoveInput(InputAction.CallbackContext ctx)
        {
            var direction = ctx.ReadValue<Vector2>().ToHorizontalPlane();
            if (_isJumping)
                direction *= airborneSpeedMultiplier;
            _character?.SetDirection(direction);
        }

        private void HandleJumpInput(InputAction.CallbackContext ctx)
        {
            //TODO: This function is barely readable. We need to refactor how we control the jumping
            if (_isJumping)
            {
                //Esto genera que el salto tenga un tope maximo de saltos
                if (_jumpCount >= maxCount - 1)
                    return;

                RunJumpCoroutine();
                _jumpCount++; //Salto y aumento de variable

                // esto te dice cual es el primer salto
                if (!_isJumping)
                    _isJumping = true;
            }
            RunJumpCoroutine();
            _isJumping = true;
            _stateMachine.SetState(_airborneState);//Cambio a estado aire
        }

        //Cuando llegas al tope no te deja saltar mas
        public void ResetJump()
        {
            _jumpCount = 0;
            _isJumping = false;
        }

        private void RunJumpCoroutine()
        {
            if (_jumpCoroutine != null)
                StopCoroutine(_jumpCoroutine);
            _jumpCoroutine = StartCoroutine(_character.Jump());
        }

        private void OnCollisionEnter(Collision other)
        {
            foreach (var contact in other.contacts)
            {
                if (Vector3.Angle(contact.normal, Vector3.up) < 5)
                {
                    _isJumping = false;
                    _stateMachine.SetState(_groundedState);// cambio a estado tierra

                }
            }
        }
    }

}

//Genero estado en tierra
public class GroundedState : IPlayerState
{
    private PlayerController _player;
    public GroundedState(PlayerController player) { _player = player; }
    public void Enter() { _player.ResetJump(); } // resetea el salto al tocar tierra 
    public void Update() { }
    public void Exit() { }
}

//Genero estado en aire
public class AirborneState : IPlayerState
{
    private PlayerController _player;
    public AirborneState(PlayerController player) { _player = player; }
    public void Enter() { }
    public void Update() { }
    public void Exit() { }
}


/*using Gameplay;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay
{
    [RequireComponent(typeof(Character))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private InputActionReference moveInput;
        [SerializeField] private InputActionReference jumpInput;
        [SerializeField] private float airborneSpeedMultiplier = .5f;
        //TODO: This booleans are not flexible enough. If we want to have a third jump or other things, it will become a hazzle.
        private bool _isJumping;
        private bool _isDoubleJumping;
        private Character _character;
        private Coroutine _jumpCoroutine;

        private void Awake()
            => _character = GetComponent<Character>();

        private void OnEnable()
        {
            if (moveInput?.action != null)
            {
                moveInput.action.started += HandleMoveInput;
                moveInput.action.performed += HandleMoveInput;
                moveInput.action.canceled += HandleMoveInput;
            }
            if (jumpInput?.action != null)
                jumpInput.action.performed += HandleJumpInput;
        }
        private void OnDisable()
        {
            if (moveInput?.action != null)
            {
                moveInput.action.performed -= HandleMoveInput;
                moveInput.action.canceled -= HandleMoveInput;
            }
            if (jumpInput?.action != null)
                jumpInput.action.performed -= HandleJumpInput;
        }

        private void HandleMoveInput(InputAction.CallbackContext ctx)
        {
            var direction = ctx.ReadValue<Vector2>().ToHorizontalPlane();
            if (_isJumping || _isDoubleJumping)
                direction *= airborneSpeedMultiplier;
            _character?.SetDirection(direction);
        }

        private void HandleJumpInput(InputAction.CallbackContext ctx)
        {
            //TODO: This function is barely readable. We need to refactor how we control the jumping
            if (_isJumping)
            {
                if (_isDoubleJumping)
                    return;
                RunJumpCoroutine();
                _isDoubleJumping = true;
                return;
            }
            RunJumpCoroutine();
            _isJumping = true;
        }

        private void RunJumpCoroutine()
        {
            if (_jumpCoroutine != null)
                StopCoroutine(_jumpCoroutine);
            _jumpCoroutine = StartCoroutine(_character.Jump());
        }

        private void OnCollisionEnter(Collision other)
        {
            foreach (var contact in other.contacts)
            {
                if (Vector3.Angle(contact.normal, Vector3.up) < 5)
                {
                    _isJumping = false;
                    _isDoubleJumping = false;
                }
            }
        }
    }
}*/