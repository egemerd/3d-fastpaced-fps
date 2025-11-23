using UnityEngine;

public class Climbing : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private Transform climbingPoint;
    InputManager input;
    CharacterController characterController;

    [Header("Climbing Settings")]
    [SerializeField] private float verticalClimbSpeed = 7f;
    [SerializeField] private float horizontalClimbSpeed = 3f;
    [SerializeField] private float sphereRadius;
    [SerializeField] private float maxDistance;
    [SerializeField] private LayerMask whatIsWall;

    [Header("Climb Timer")]
    [SerializeField] private float maxClimbTime;
    private float climbTimer;
    
    

    RaycastHit wallHit;
    public bool isWallHit;
    public bool isClimbing;

    

    private void Start()
    {
        input = InputManager.Instance;
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        WallHit();
    }
    public bool IsMovingForward()
    {
        return input.IsMovingForward;
    }
    private void WallHit()
    {
        isWallHit = Physics.SphereCast(transform.position, sphereRadius, 
            climbingPoint.forward, out wallHit, maxDistance,whatIsWall);
  
    }
    public void StartClimbing()
    {
        if(!isWallHit) return;
        isClimbing = true;
        climbTimer = maxClimbTime;
    }

    public void StopClimbing()
    {
        isClimbing = false; 
    }

    public void ClimbingMovement()
    {
        if(!isClimbing) return;

        float verticalInput = input.moveInput.y;
        float horizontalInput = input.moveInput.x;

        Vector3 climbMove = new Vector3(
        horizontalInput * horizontalClimbSpeed * Time.deltaTime,  // X: Yan hareket
        verticalInput * verticalClimbSpeed * Time.deltaTime,      // Y: Yukarý/aþaðý
        0                                                          // Z: Duvara girme/çýkma
        );
        characterController.Move(climbMove);
        if(climbTimer > 0) { climbTimer -= Time.deltaTime; }
        if(climbTimer < 0) { StopClimbing();}

    }

    private void OnDrawGizmos()
    {
        Vector3 startPos = transform.position;
        Vector3 direction = climbingPoint.forward;
        float distance = maxDistance;

      
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(startPos, sphereRadius);

       
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(startPos, direction * distance);

       
        Gizmos.color = Color.red;
        Vector3 endPos = startPos + direction * distance;
        Gizmos.DrawWireSphere(endPos, sphereRadius);

       
        if (Application.isPlaying && isWallHit)
        {
            // Hit noktasýný göster
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(wallHit.point, 0.1f); // Küçük solid sphere

            // Hit noktasýna çizgi
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(startPos, wallHit.point);

            // Hit normal vektörü
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(wallHit.point, wallHit.normal * 0.5f);

            // Hit anýndaki sphere pozisyonu
            Gizmos.color = Color.green;
            float hitDistance = Vector3.Distance(startPos, wallHit.point);
            Vector3 hitSpherePos = startPos + direction * hitDistance;
            Gizmos.DrawWireSphere(hitSpherePos, sphereRadius);
        }

        
        else if (Application.isPlaying)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.3f); // Kýrmýzý, þeffaf
            Gizmos.DrawLine(startPos, endPos);
        }
    }
}
