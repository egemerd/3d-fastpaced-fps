using UnityEngine;

public class WeaponAnim : MonoBehaviour
{
    private Vector2 lookInput;
    private InputManager input;

    [Header("Sway Settings")]
    [SerializeField] private float swaySpeed = 2f;
    [SerializeField] private float swayAmount = 0.04f;
    [SerializeField] private float maxSway = 5f;


    private Vector3 initialSwayPos;
    private Vector3 targetSwayPos;

    
    private void Start()
    {
        input = InputManager.Instance;
        initialSwayPos = transform.localPosition;
    }

    private void Update()
    {
        WeaponSway();
    }


    public void WeaponSway()
    {
        lookInput = input.lookInput;
        lookInput = Vector2.ClampMagnitude(lookInput, maxSway);
        Vector3 targetSwayPos = initialSwayPos + new Vector3(lookInput.x, lookInput.y, 0) * swayAmount;
        transform.localPosition = Vector3.Lerp(transform.localPosition, 
            targetSwayPos, 
            Time.deltaTime * swaySpeed);
    }
   
    
}
