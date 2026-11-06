using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAvatar : NetworkBehaviour
{
    [Networked]
    public NetworkString<_16> NickName { get; set; }
    private NetworkCharacterController characterController;
    private InputAction moveAction;
    private InputAction jumpAction;

    public override void Spawned()
    {
        characterController = GetComponent<NetworkCharacterController>();

        var view = GetComponent<PlayerAvatarView>();
        // プレイヤー名をテキストに反映する
        view.SetNickName(NickName.Value);

        // 自分のアバターでなければ、ここで終了
        if (!HasStateAuthority) return;

        // 自身がアバターの権限を持っているなら、カメラの追従対象にする
        view.MakeCameraTarget();

        // 移動(WASD / 矢印キー / ゲームパッド左スティック)
        moveAction = new InputAction("Move", InputActionType.Value);
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");
        moveAction.AddBinding("<Gamepad>/leftStick");

        // ジャンプ(スペース / ゲームパッド下ボタン)
        jumpAction = new InputAction("Jump", InputActionType.Button);
        jumpAction.AddBinding("<Keyboard>/space");
        jumpAction.AddBinding("<Gamepad>/buttonSouth");

        moveAction.Enable();
        jumpAction.Enable();
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        moveAction?.Dispose();
        jumpAction?.Dispose();
    }

    public override void FixedUpdateNetwork()
    {
        // 自分のアバター以外は入力を読まない
        if (!HasStateAuthority) return;

        // 移動(カメラの向きを基準にする)
        var cameraRotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);
        Vector2 move = moveAction.ReadValue<Vector2>();
        var inputDirection = new Vector3(move.x, 0f, move.y);
        characterController.Move(cameraRotation * inputDirection);

        // ジャンプ
        if (jumpAction.IsPressed())
        {
            characterController.Jump();
        }
    }
}