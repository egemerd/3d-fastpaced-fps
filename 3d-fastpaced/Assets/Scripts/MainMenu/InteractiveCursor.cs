using UnityEngine;
using System.Collections.Generic;

public class InteractiveCursor : MonoBehaviour
{
    [Header("Cursor Settings")]
    [SerializeField] private Camera uiCamera;
    [SerializeField] private float cursorZ = 10f;
    [SerializeField] private float smoothSpeed = 15f;

    [Header("Trail Settings")]
    [SerializeField] private GameObject trailPrefab;
    [SerializeField] private int maxTrailPoints = 20;
    [SerializeField] private float trailLifetime = 0.5f;
    [SerializeField] private float spawnInterval = 0.02f;

    [Header("Particle Settings")]
    [SerializeField] private ParticleSystem cursorParticles;
    [SerializeField] private float particleEmissionRate = 50f;

    [Header("Glow Settings")]
    [SerializeField] private SpriteRenderer glowSprite;
    [SerializeField] private float glowPulseSpeed = 2f;
    [SerializeField] private float glowMinScale = 0.8f;
    [SerializeField] private float glowMaxScale = 1.2f;

    [Header("Motion Settings")]
    [SerializeField] private float velocityInfluence = 0.1f;
    [SerializeField] private AnimationCurve velocityCurve;

    private Vector3 targetPosition;
    private Vector3 currentVelocity;
    private Queue<TrailPoint> trailPoints = new Queue<TrailPoint>();
    private float lastSpawnTime;
    private LineRenderer lineRenderer;

    private struct TrailPoint
    {
        public Vector3 position;
        public float spawnTime;
    }

    private void Awake()
    {
        // Hardware cursor'ý gizle
        Cursor.visible = false;

        // Line Renderer setup
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        SetupLineRenderer();

        if (uiCamera == null)
        {
            uiCamera = Camera.main;
        }
    }

    private void SetupLineRenderer()
    {
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = new Color(1f, 1f, 1f, 0.8f);
        lineRenderer.endColor = new Color(1f, 1f, 1f, 0f);
        lineRenderer.numCapVertices = 5;
        lineRenderer.numCornerVertices = 5;
    }

    private void Update()
    {
        UpdateCursorPosition();
        UpdateTrail();
        UpdateGlow();
        UpdateParticles();
    }

    private void UpdateCursorPosition()
    {
        // Mouse pozisyonunu world space'e çevir
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = cursorZ;
        targetPosition = uiCamera.ScreenToWorldPoint(mousePos);

        // Smooth movement
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref currentVelocity,
            1f / smoothSpeed
        );

        // Velocity based rotation
        if (currentVelocity.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(currentVelocity.y, currentVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                Quaternion.Euler(0, 0, angle),
                Time.deltaTime * 10f
            );
        }
    }

    private void UpdateTrail()
    {
        // Trail point spawn
        if (Time.time - lastSpawnTime > spawnInterval)
        {
            TrailPoint newPoint = new TrailPoint
            {
                position = transform.position,
                spawnTime = Time.time
            };

            trailPoints.Enqueue(newPoint);
            lastSpawnTime = Time.time;
        }

        // Remove old points
        while (trailPoints.Count > 0 &&
               Time.time - trailPoints.Peek().spawnTime > trailLifetime)
        {
            trailPoints.Dequeue();
        }

        // Update line renderer
        if (trailPoints.Count > 1)
        {
            lineRenderer.positionCount = trailPoints.Count;
            int index = 0;
            foreach (var point in trailPoints)
            {
                lineRenderer.SetPosition(index, point.position);
                index++;
            }
        }
        else
        {
            lineRenderer.positionCount = 0;
        }
    }

    private void UpdateGlow()
    {
        if (glowSprite == null) return;

        // Pulse efekti
        float pulse = Mathf.Lerp(
            glowMinScale,
            glowMaxScale,
            (Mathf.Sin(Time.time * glowPulseSpeed) + 1f) / 2f
        );
        glowSprite.transform.localScale = Vector3.one * pulse;

        // Velocity based glow intensity
        float velocityMagnitude = currentVelocity.magnitude;
        float intensity = velocityCurve.Evaluate(velocityMagnitude * velocityInfluence);
        Color glowColor = glowSprite.color;
        glowColor.a = Mathf.Lerp(0.3f, 1f, intensity);
        glowSprite.color = glowColor;
    }

    private void UpdateParticles()
    {
        if (cursorParticles == null) return;

        // Velocity based particle emission
        var emission = cursorParticles.emission;
        float velocityMagnitude = currentVelocity.magnitude;
        emission.rateOverTime = particleEmissionRate * (1f + velocityMagnitude * 2f);

        // Particle direction
        var shape = cursorParticles.shape;
        if (currentVelocity.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(currentVelocity.y, currentVelocity.x) * Mathf.Rad2Deg;
            shape.rotation = new Vector3(0, 0, angle + 180f);
        }
    }

    private void OnDestroy()
    {
        Cursor.visible = true;
    }
}