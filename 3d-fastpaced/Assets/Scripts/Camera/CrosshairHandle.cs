using UnityEngine;

public class CrosshairHandle : MonoBehaviour
{
    PlayerController playerController;
    RectTransform crosshairRect;

    [SerializeField] float maxScale = 3f;
    [SerializeField] float jumpMaxScale = 3f;
    [SerializeField] float jumpMultiplier = 2;
    [SerializeField] float scaleMultipler;
    [SerializeField] float animSpeed;
    Vector2 minScale;
    private void Awake()
    {
        crosshairRect = GetComponent<RectTransform>();
        playerController = FindObjectOfType<PlayerController>();
    }

    private void OnEnable()
    {
        minScale = crosshairRect.localScale;
    }

    private void Update()
    {
        CrosshairAnim();
    }
    private void CrosshairAnim()
    {
        float jump = playerController.isGrounded ? 0 : jumpMultiplier;
        Vector2 jumpOffset = Vector2.one * jump;
        Vector2 targetSize = playerController.GetVelocity() * scaleMultipler * Vector2.one;
        targetSize.x = Mathf.Clamp(targetSize.x, minScale.x, maxScale);
        targetSize.y = Mathf.Clamp(targetSize.y, minScale.y, maxScale);
        Debug.Log("Target Size:" + targetSize); 
        crosshairRect.localScale = Vector3.Lerp(crosshairRect.localScale, 
            targetSize + jumpOffset, Time.deltaTime * animSpeed);
    }
}
