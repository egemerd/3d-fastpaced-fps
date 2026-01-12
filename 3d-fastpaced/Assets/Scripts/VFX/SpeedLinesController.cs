using UnityEngine;

public class SpeedLinesController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ParticleSystem speedlinesParticle;
    [SerializeField] private PlayerController playerController;

    private bool wasSliding = false;

    private void Awake()
    {
        // PlayerController'ý otomatik bul
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }

        // Particle System'i kontrol et
        if (speedlinesParticle == null)
        {
            speedlinesParticle = GetComponent<ParticleSystem>();
        }

        // Baþlangýçta kapalý
        if (speedlinesParticle != null)
        {
            speedlinesParticle.Stop();
        }
    }

    private void Update()
    {
        if (playerController == null || speedlinesParticle == null) return;

        // Slide durumunu kontrol et
        bool isSliding = playerController.isSliding;

        // Slide baþladý
        if (isSliding && !wasSliding)
        {
            speedlinesParticle.Play();
        }
        // Slide bitti
        else if (!isSliding && wasSliding)
        {
            speedlinesParticle.Stop();
        }

        wasSliding = isSliding;
    }
}
