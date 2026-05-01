using UnityEngine;

public class PokemonBalls : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject PregabParticle;
    [SerializeField] private AudioSource Audio;
    bool isListening = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Duck")) return;

        Vector3 hitPos = other.transform.position;

        // Particle
        if (PregabParticle != null)
        {
            Destroy(Instantiate(PregabParticle, hitPos, Quaternion.identity), 2f);
        }

        // Audio
        if (Audio != null)
        {
            Audio.Play();
        }

        GameManager.Singleton.SetScore(1);
        DuckCounter.Singleton.OnCountDuck(GameManager.Singleton.jumlahMenangkap);

        // 🔥 DELETE pakai ID dari name
        if (int.TryParse(other.gameObject.name, out int id))
        {
            GameManager.Singleton.Controller.DeleteById(id);
        }
        else
        {
            Debug.LogWarning("ID tidak valid: " + other.gameObject.name);
        }

        Destroy(other.gameObject);
        Destroy(gameObject);

        switch (GameManager.Singleton.modegame)
        {
            case ModeGame.Tutorial:
                GameManager.Singleton.TutorialGame();
                break;

            case ModeGame.PlayGame:
                FireBase.instance.UpdateScore(
                    PlayerPrefs.GetString("ID"),
                    GameManager.Singleton.jumlahMenangkap
                );
                if (!isListening)
                {
                    isListening = true;

                    FireBase.instance.GetTopScore(PlayerPrefs.GetString("ID"), (rank) =>
                    {
                        if (rank != -1)
                        {
                            UIManager.Instance.SetRankTF(rank);
                        }
                        else
                        {
                            Debug.Log("Rank tidak tersedia");
                        }
                    });
                }
                break;
        }
    }
}