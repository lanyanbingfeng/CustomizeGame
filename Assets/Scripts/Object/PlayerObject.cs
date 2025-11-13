
using UnityEngine;

public class PlayerObject : MonoBehaviour
{
    public float moveSpeed;
    public float rotateSpeed;
    public float maxJumpHeight;
    
    private bool _isRun;
    private float _runValue;
    private float _moveSpeedValue;
    
    private CharacterController _characterController;
    private Animator _animator;
    private static readonly int Move = Animator.StringToHash("Move");
    private float _animatorValue; //动画偏移值
    
    private Vector3 velocity;
    private bool isJumping;
    private bool isAtking;
    
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        
        EventCenterManager.Instance.AddEventListener(E_EventType.PlayerDownCtrl, () => { SetMoveMode(!_isRun); });
        EventCenterManager.Instance.AddEventListener(E_EventType.PlayerJump, () =>
        {
            if (isAtking || isJumping) return;
            _animator.SetTrigger("Jump");
            isJumping = true;
            velocity.y = Mathf.Sqrt(2 * Mathf.Abs(Physics.gravity.y) * maxJumpHeight);
        });
        EventCenterManager.Instance.AddEventListener(E_EventType.PlayerMouse0Atk, () =>
        {
            if (isAtking || isJumping) return;
            _animator.SetTrigger("Atk1");
            isAtking = true;
        });
        EventCenterManager.Instance.AddEventListener(E_EventType.PlayerMouse1Atk, () =>
        {
            if (isAtking || isJumping) return;
            _animator.SetTrigger("Atk2");
            isAtking = true;
        });
        EventCenterManager.Instance.AddEventListener(E_EventType.PlayerFAtk, () =>
        {
            if (isAtking || isJumping) return;
            _animator.SetTrigger("FAtk");
            isAtking = true;
        });
        
        InputManager.Instance.AddKeyboardInput(E_EventType.PlayerDownCtrl,KeyCode.LeftControl,InputInfo.E_InputMode.Down);
        InputManager.Instance.AddKeyboardInput(E_EventType.PlayerJump,KeyCode.Space,InputInfo.E_InputMode.Down);
        InputManager.Instance.AddMouseInput(E_EventType.PlayerMouse0Atk,0,InputInfo.E_InputMode.Down);
        InputManager.Instance.AddMouseInput(E_EventType.PlayerMouse1Atk,1,InputInfo.E_InputMode.Down);
        InputManager.Instance.AddKeyboardInput(E_EventType.PlayerFAtk,KeyCode.F,InputInfo.E_InputMode.Down);
        
        MonoManager.Instance.AddUpdateEvent(MyUpdate);
    }

    private void SetMoveMode(bool flag)
    {
        _isRun = flag;
        moveSpeed = _isRun ? moveSpeed * 2 : moveSpeed / 2;
    }

    private void JumpAnimationEndEvent() { isJumping = false; }
    private void AtkAnimationEndEvent() { isAtking = false; }

    private void MyUpdate()
    {
        //得到移动方向
        Vector3 moveDir = new Vector3(Input.GetAxis("Horizontal"),0, Input.GetAxis("Vertical"));
        
        if (moveDir != Vector3.zero)
        {
            Vector3 cameraDir = Camera.main.transform.forward;
            cameraDir.y = 0;
            cameraDir.Normalize();
            
            Quaternion worldOffset = Quaternion.FromToRotation(Vector3.forward,cameraDir); // 计算世界偏移量

            Vector3 targetDir = worldOffset * moveDir;
            Quaternion targetRotation = Quaternion.LookRotation(targetDir);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotateSpeed);
        }

        if (isAtking) _moveSpeedValue = Mathf.Clamp(_moveSpeedValue - Time.deltaTime * 5, 0, moveSpeed);
        else _moveSpeedValue = moveSpeed;
        
        _characterController.Move(transform.forward * (_moveSpeedValue * Time.deltaTime * Mathf.Max(Mathf.Abs(Input.GetAxis("Horizontal")), Mathf.Abs(Input.GetAxis("Vertical")))));
        _characterController.Move(velocity * Time.deltaTime);

        if (_characterController.isGrounded) velocity.y = -2;
        else velocity.y += Physics.gravity.y * Time.deltaTime;
        
        if (_isRun)
        {
            _runValue = Mathf.Clamp(_runValue + Time.deltaTime, 0.5f, 1f);
            _animator.SetFloat(Move,_runValue * Mathf.Max(Mathf.Abs(Input.GetAxis("Horizontal")), Mathf.Abs(Input.GetAxis("Vertical"))));
        }
        else
        {
            _runValue = Mathf.Clamp(_runValue - Time.deltaTime, 0.5f, 1f);
            _animator.SetFloat(Move,_runValue * Mathf.Max(Mathf.Abs(Input.GetAxis("Horizontal")), Mathf.Abs(Input.GetAxis("Vertical"))));
        }
        
    }
}
