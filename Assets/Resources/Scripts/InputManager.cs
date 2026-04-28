using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum MekanikGame
{
    Tap,
    Swipe,
    SlingShoot,
}

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }


    [Header("References")]
    [SerializeField] private InputActionAsset inputActions;

    [Header("Mode")]
    public MekanikGame mode;

    private InputActionMap playerInputAction;
    private InputAction hit;

    // EVENTS
    public Action<bool> OnHit;
    public Action<Vector2, Vector2> OnSwipe;
    public Action<Vector2, Vector2, float> OnSlingshot;
    public Action<Vector2, Vector2> OnSlingshotDrag;

    [Header("Slingshot Settings")]
    [SerializeField] private float maxDragDistance = 300f;
    [SerializeField] private float powerMultiplier = 1.5f;

    private Vector2 startPos;
    private bool isTouching;
    private bool isDragging;
    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // optional (biar tidak hilang saat pindah scene)
    }
    private void OnEnable()
    {
        playerInputAction = inputActions.FindActionMap("Player");
        hit = playerInputAction.FindAction("Attack");

        playerInputAction.Enable();
    }

    private void OnDisable()
    {
        playerInputAction.Disable();
    }

    private void Update()
    {
        switch (mode)
        {
            case MekanikGame.Tap:
                HandleTap();
                break;

            case MekanikGame.Swipe:
                HandleSwipe();
                break;

            case MekanikGame.SlingShoot:
                HandleSlingshot();
                break;
        }
    }

    // ================= TAP =================
    void HandleTap()
    {
        if (hit.triggered)
        {
            OnHit?.Invoke(true);
        }
    }

    // ================= SWIPE =================
    void HandleSwipe()
    {
        if (Touchscreen.current == null) return;

        var touch = Touchscreen.current.primaryTouch;

        if (touch.press.wasPressedThisFrame)
        {
            startPos = touch.position.ReadValue();
            isTouching = true;
        }

        if (touch.press.wasReleasedThisFrame && isTouching)
        {
            Vector2 endPos = touch.position.ReadValue();

            if (Vector2.Distance(startPos, endPos) > 50f)
            {
                OnSwipe?.Invoke(startPos, endPos);
            }

            isTouching = false;
        }
    }

    // ================= SLINGSHOT =================
    void HandleSlingshot()
    {
        if (Touchscreen.current == null) return;

        var touch = Touchscreen.current.primaryTouch;

        // mulai tarik
        if (touch.press.wasPressedThisFrame)
        {
            startPos = touch.position.ReadValue();
            isDragging = true;
        }

        // 🔥 DRAG (buat preview trajectory)
        if (isDragging && touch.press.isPressed)
        {
            Vector2 current = touch.position.ReadValue();
            OnSlingshotDrag?.Invoke(startPos, current);
        }

        // lepas
        if (touch.press.wasReleasedThisFrame && isDragging)
        {
            Vector2 endPos = touch.position.ReadValue();

            float distance = Vector2.Distance(startPos, endPos);
            float clamped = Mathf.Clamp(distance, 0, maxDragDistance);

            float power = (clamped / maxDragDistance) * powerMultiplier;

            OnSlingshot?.Invoke(startPos, endPos, power);

            isDragging = false;
        }
    }


}