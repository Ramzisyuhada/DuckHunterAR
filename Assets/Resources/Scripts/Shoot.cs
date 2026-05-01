using UnityEngine;

public class Shoot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputManager manager;
    [SerializeField] private GameObject PrefabNet;

    [Header("Shoot Settings")]
    [SerializeField] private float shootForce = 10f;
    [SerializeField] private float shootCooldown = 0.5f;

    [Header("Trajectory")]
    [SerializeField] private LineRenderer line;
    [SerializeField] private int resolution = 30;
    [SerializeField] private float timeStep = 0.1f;

    [Header("Aim Assist (Swipe)")]
    [SerializeField] private float aimAssistStrength = 0.6f; // 0 - 1
    [SerializeField] private float aimAssistRange = 20f;
    [SerializeField] private LayerMask aimLayer;

    [Header("Shoot Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] swingSounds;


    public int Ammo;

    private float lastShootTime;
    private Camera cam;

    void PlaySwingSound()
    {
        if (audioSource == null || swingSounds.Length == 0) return;

        int index = Random.Range(0, swingSounds.Length);

        audioSource.pitch = Random.Range(0.9f, 1.1f); // biar tidak monoton
        audioSource.PlayOneShot(swingSounds[index]);
    }

    private void Start()
    {
        cam = Camera.main;

        manager.OnHit += Hit;
        manager.OnSwipe += HandleSwipe;
        manager.OnSlingshot += HandleSlingshot;
        manager.OnSlingshotDrag += PreviewSlingshot;
    }

    private void OnDestroy()
    {
        manager.OnHit -= Hit;
        manager.OnSwipe -= HandleSwipe;
        manager.OnSlingshot -= HandleSlingshot;
        manager.OnSlingshotDrag -= PreviewSlingshot;
    }

    void DrawTrajectory(Vector3 startPos, Vector3 velocity)
    {
        if (line == null) return;

        line.positionCount = resolution;

        for (int i = 0; i < resolution; i++)
        {
            float t = i * timeStep;

            Vector3 point = startPos +
                            velocity * t +
                            0.5f * Physics.gravity * t * t;

            line.SetPosition(i, point);
        }
    }

    void ClearTrajectory()
    {
        if (line == null) return;
        line.positionCount = 0;
    }

    private void PreviewSlingshot(Vector2 start, Vector2 current)
    {
        Vector2 drag = start - current;

        Vector3 direction =
            cam.transform.forward +
            cam.transform.right * (drag.x / Screen.width) +
            cam.transform.up * (drag.y / Screen.height);

        float distance = drag.magnitude;
        float power = Mathf.Clamp01(distance / 300f);

        Vector3 velocity = direction.normalized * power * shootForce;

        DrawTrajectory(cam.transform.position, velocity);
    }
    public void AddAmmo(int amount)
    {
        Ammo += amount;
    }
    private bool CanShoot()
    {
        if (Ammo <= 0) {
            UIManager.Instance.PlayEmptyAnimation();
            return false;
        } 

        if (Time.time < lastShootTime + shootCooldown) return false;

        lastShootTime = Time.time;
        return true;
    }

    private void Hit(bool isTap)
    {
        if (!isTap || !CanShoot()) return;
        PlaySwingSound();

        Fire(cam.transform.forward, 1f);
    }

    private void HandleSwipe(Vector2 start, Vector2 end)
    {


        if (!CanShoot()) return;
        UIManager.Instance.PlayDecreaseAnimation();

        Vector2 swipe = end - start;

        if (swipe.magnitude < 50f) return;

        Vector3 direction =
            cam.transform.forward +
            cam.transform.right * (swipe.x / Screen.width) +
            cam.transform.up * (swipe.y / Screen.height);

        direction.Normalize();

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, aimAssistRange, aimLayer))
        {
            Vector3 targetDir = (hit.point - cam.transform.position).normalized;

            direction = Vector3.Lerp(direction, targetDir, aimAssistStrength).normalized;
        }

        float power = Mathf.Clamp(swipe.magnitude / 300f, 0.2f, 1f);
        PlaySwingSound();

        Fire(direction, power);
    }

    private void HandleSlingshot(Vector2 start, Vector2 end, float power)
    {
        if (!CanShoot()) return;

        Vector2 drag = start - end;

        Vector3 direction =
            cam.transform.forward +
            cam.transform.right * (drag.x / Screen.width) +
            cam.transform.up * (drag.y / Screen.height);

        direction.Normalize();
        PlaySwingSound();

        Fire(direction, power);

        ClearTrajectory();
    }

    private void Fire(Vector3 direction, float power)
    {
        GameObject net = Instantiate(PrefabNet, cam.transform.position, Quaternion.identity);
        Rigidbody rb = net.GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogWarning("PrefabNet belum ada Rigidbody!");
            return;
        }

        Ammo -= 1;
        UIManager.Instance.SetTextAmmo(Ammo);
        rb.linearVelocity = direction.normalized * power * shootForce;
    }
}