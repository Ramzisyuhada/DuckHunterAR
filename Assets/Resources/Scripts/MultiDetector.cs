using UnityEngine;

public class MultiDetector : MonoBehaviour
{



    void SetDuckVisible(GameObject duck, bool state)
    {
        Renderer[] renderers = duck.GetComponentsInChildren<Renderer>();

        foreach (var r in renderers)
        {
            r.enabled = state;
        }
    }
    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Duck"))
        {
            SetDuckVisible(other.gameObject, true);

            Duck duck = other.GetComponent<Duck>();
            if (duck != null)
            {
                duck.isPlayerInside = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Duck"))
        {
            SetDuckVisible(other.gameObject, false);

            Duck duck = other.GetComponent<Duck>();
            if (duck != null)
            {
                duck.isPlayerInside = false;
            }
        }
    }
}
