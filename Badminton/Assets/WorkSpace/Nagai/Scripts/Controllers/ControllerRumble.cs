using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// コントローラーの振動を管理するクラス
/// </summary>
public class ControllerRumble : MonoBehaviour
{
    private const float MotorSpeedMin = 0.0f; // モーターの最小速度
    private const float DefaultVibrationDuration = 0.3f; // デフォルトのコントローラーの振動時間
    private const float DefaultVibrationIntensity = 0.8f; // デフォルトのコントローラーの振動強度
    [Tooltip("プレイヤーの取得")]
    [SerializeField] private GameObject[] players = null;

    [Header("コントローラーの振動")]
    [Tooltip("振動時間")]
    [SerializeField] private float vibrationDuration = DefaultVibrationDuration;
    [Tooltip("振動の強さ")]
    [SerializeField] private float vibrationIntensity = DefaultVibrationIntensity;

    private Gamepad targetGamepad = null;
    private Coroutine rumbleCoroutine = null;

    private Gamepad Pad => targetGamepad ??= Gamepad.current;

    //振動させるコントローラーの指定
    public void SetGamepad(Gamepad gamepad)
    {
        targetGamepad = gamepad;
    }
    public void Rumble()
    {
        Rumble(vibrationIntensity, vibrationIntensity, vibrationDuration);
    }

    //コントローラーを振動させる
    public void Rumble(float low, float high, float duration)
    {
        if (Pad == null)
        {
            return;
        }

        if (rumbleCoroutine != null)
        {
            StopCoroutine(rumbleCoroutine);
        }
        rumbleCoroutine = StartCoroutine(RumbleRoutine(duration, low, high));
    }

    //コントローラーの振動を止める
    public void Stop()
    {
        if (rumbleCoroutine != null)
        {
            StopCoroutine(rumbleCoroutine);
            rumbleCoroutine = null;
        }
        Pad?.SetMotorSpeeds(MotorSpeedMin, MotorSpeedMin);
    }

    //コントローラーの振動を行うコルーチン
    private IEnumerator RumbleRoutine(float duration, float low, float high)
    {
        Pad?.SetMotorSpeeds(low, high);
        yield return new WaitForSeconds(duration);
        Pad?.SetMotorSpeeds(MotorSpeedMin, MotorSpeedMin);
        rumbleCoroutine = null;
    }

    //無効時にコントローラーの振動を止める
    private void OnDisable()
    {
        Stop();
    }

    //確認用
    private void Update()
    {
        if (Pad == null)
        {
            return;
        }
        if (Pad.buttonSouth.wasPressedThisFrame)
        {
            Rumble();
        }
    }
}