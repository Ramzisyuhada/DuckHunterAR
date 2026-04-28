using Unity.VisualScripting;
using UnityEngine;

public class PokemonBalls : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject PregabParticle;

    [SerializeField] private AudioSource Audio;   

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Duck"))
        {
            // 🎯 Ambil posisi tengah bebek
            Vector3 hitPos = other.bounds.center;

            // ✨ Spawn particle
            if (PregabParticle != null)
            {
              Destroy(  Instantiate(PregabParticle, hitPos, Quaternion.identity),2f);
               Audio.Play();
            }
            GameManager.Singleton.SetScore(1);
            DuckCounter.Singleton.OnCountDuck(GameManager.Singleton.jumlahMenangkap);
            other.gameObject.SetActive(false);


            Destroy(gameObject);

            switch (GameManager.Singleton.modegame)
            {
                case ModeGame.Tutorial:
                    GameManager.Singleton.TutorialGame();
                    break;
                case ModeGame.PlayGame:
                    FireBase.instance.SavePlayerData(PlayerPrefs.GetString("Name"), GameManager.Singleton.jumlahMenangkap);
                    break;

            }
        }
    }
}