using UnityEngine;

public class AmmoPickUp : MonoBehaviour
{
    [Header("Refrences")]

    [SerializeField] private AudioSource Sfx_Colect;



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Shoot.Singleton.Ammo += 5;
            UIManager.Instance.SetTextAmmo(Shoot.Singleton.Ammo);
            Sfx_Colect.pitch = Random.Range(0.95f, 1.05f);
            Sfx_Colect.Play();
            // Destroy(gameObject, Sfx_Colect.clip.length);
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
