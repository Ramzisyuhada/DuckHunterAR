using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Duck : MonoBehaviour
{
    private Animator anim;
    private Transform player;
    private AudioSource audioSource;

    [Header("Animation")]
    [SerializeField] private string[] animations = { "Idle", "Fly", "Swim", "Spin/Splash" };

    [Header("Look At Player")]
    [SerializeField] private float rotateSpeed = 5f;

    [Header("Audio")]
    [SerializeField] private AudioClip[] duckSounds;
    [SerializeField] private float minSoundDelay = 2f;
    [SerializeField] private float maxSoundDelay = 5f;

    private float nextSoundTime;

    // 🔥 AREA CONTROL
    [HideInInspector] public bool isPlayerInside;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        int index = Random.Range(0, animations.Length);
        anim.Play(animations[index], 0, Random.value);

        audioSource.pitch = Random.Range(0.9f, 1.2f);
    }

    private void Start()
    {
        player = Camera.main.transform;
        SetNextSoundTime();
    }

    private void Update()
    {
        LookAtPlayer();
        HandleSound();
    }

    void LookAtPlayer()
    {
        if (player == null) return;

        Vector3 direction = player.position - transform.position;
        direction.y = 0;

        if (direction == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRotation,
            rotateSpeed * Time.deltaTime
        );
    }

    void HandleSound()
    {
        // 🔥 kalau player tidak di area → jangan bunyi
        if (!isPlayerInside)
        {
            if (audioSource.isPlaying)
                audioSource.Stop();

            return;
        }

        if (duckSounds.Length == 0) return;

        if (Time.time >= nextSoundTime)
        {
            int index = Random.Range(0, duckSounds.Length);
            audioSource.PlayOneShot(duckSounds[index]);

            SetNextSoundTime();
        }
    }

    void SetNextSoundTime()
    {
        nextSoundTime = Time.time + Random.Range(minSoundDelay, maxSoundDelay);
    }
}