using UnityEngine;
using System.Collections.Generic;

public class InteractiveBackground : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int gridWidth = 20;
    [SerializeField] private int gridHeight = 15;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private GameObject gridPointPrefab;

    [Header("Interaction Settings")]
    [SerializeField] private float interactionRadius = 3f;
    [SerializeField] private float pushForce = 5f;
    [SerializeField] private float returnSpeed = 2f;
    [SerializeField] private AnimationCurve forceCurve;

    [Header("Visual Settings")]
    [SerializeField] private float pulseSpeed = 1f;
    [SerializeField] private float pulseAmount = 0.1f;
    [SerializeField] private Gradient colorGradient;

    private GridPoint[,] gridPoints;
    private Transform cursorTransform;

    private class GridPoint
    {
        public GameObject gameObject;
        public Vector3 originalPosition;
        public Vector3 currentPosition;
        public Vector3 velocity;
        public float distanceToCursor;
        public SpriteRenderer spriteRenderer;
        public LineRenderer[] connections;
    }

    private void Start()
    {
        CreateGrid();
        FindCursor();
    }

    private void FindCursor()
    {
        InteractiveCursor cursor = FindObjectOfType<InteractiveCursor>();
        if (cursor != null)
        {
            cursorTransform = cursor.transform;
        }
    }

    private void CreateGrid()
    {
        gridPoints = new GridPoint[gridWidth, gridHeight];

        Vector3 startPos = transform.position - new Vector3(
            gridWidth * cellSize / 2f,
            gridHeight * cellSize / 2f,
            0
        );

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 position = startPos + new Vector3(x * cellSize, y * cellSize, 0);

                GameObject point = Instantiate(gridPointPrefab, position, Quaternion.identity, transform);
                point.name = $"GridPoint_{x}_{y}";

                GridPoint gridPoint = new GridPoint
                {
                    gameObject = point,
                    originalPosition = position,
                    currentPosition = position,
                    velocity = Vector3.zero,
                    spriteRenderer = point.GetComponent<SpriteRenderer>()
                };

                gridPoints[x, y] = gridPoint;
            }
        }

        // Create connections (lines between points)
        CreateConnections();
    }

    private void CreateConnections()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                List<LineRenderer> connections = new List<LineRenderer>();

                // Connect to right neighbor
                if (x < gridWidth - 1)
                {
                    connections.Add(CreateLine(gridPoints[x, y], gridPoints[x + 1, y]));
                }

                // Connect to bottom neighbor
                if (y < gridHeight - 1)
                {
                    connections.Add(CreateLine(gridPoints[x, y], gridPoints[x, y + 1]));
                }

                gridPoints[x, y].connections = connections.ToArray();
            }
        }
    }

    private LineRenderer CreateLine(GridPoint p1, GridPoint p2)
    {
        GameObject lineObj = new GameObject("Connection");
        lineObj.transform.SetParent(transform);

        LineRenderer line = lineObj.AddComponent<LineRenderer>();
        line.startWidth = 0.02f;
        line.endWidth = 0.02f;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = new Color(1f, 1f, 1f, 0.2f);
        line.endColor = new Color(1f, 1f, 1f, 0.2f);
        line.positionCount = 2;

        return line;
    }

    private void Update()
    {
        if (cursorTransform == null) return;

        UpdateGridPoints();
        UpdateConnections();
    }

    private void UpdateGridPoints()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                GridPoint point = gridPoints[x, y];

                // Calculate distance to cursor
                point.distanceToCursor = Vector3.Distance(
                    point.currentPosition,
                    cursorTransform.position
                );

                // Apply cursor force
                if (point.distanceToCursor < interactionRadius)
                {
                    Vector3 direction = point.currentPosition - cursorTransform.position;
                    float normalizedDistance = point.distanceToCursor / interactionRadius;
                    float force = forceCurve.Evaluate(1f - normalizedDistance) * pushForce;

                    point.velocity += direction.normalized * force * Time.deltaTime;
                }

                // Return to original position
                Vector3 returnForce = (point.originalPosition - point.currentPosition) * returnSpeed;
                point.velocity += returnForce * Time.deltaTime;

                // Apply damping
                point.velocity *= 0.95f;

                // Update position
                point.currentPosition += point.velocity * Time.deltaTime;
                point.gameObject.transform.position = point.currentPosition;

                // Update visual
                UpdatePointVisual(point);
            }
        }
    }

    private void UpdatePointVisual(GridPoint point)
    {
        if (point.spriteRenderer == null) return;

        // Pulse efekti
        float pulse = Mathf.Sin(Time.time * pulseSpeed +
            point.originalPosition.x + point.originalPosition.y) * pulseAmount + 1f;
        point.gameObject.transform.localScale = Vector3.one * pulse;

        // Renk gradient (mesafeye göre)
        float t = Mathf.Clamp01(1f - point.distanceToCursor / interactionRadius);
        point.spriteRenderer.color = colorGradient.Evaluate(t);

        // Velocity based brightness
        float velocityMagnitude = point.velocity.magnitude;
        Color color = point.spriteRenderer.color;
        color.a = Mathf.Lerp(0.3f, 1f, Mathf.Clamp01(velocityMagnitude));
        point.spriteRenderer.color = color;
    }

    private void UpdateConnections()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                GridPoint point = gridPoints[x, y];

                if (point.connections == null) continue;

                int connectionIndex = 0;

                // Update right connection
                if (x < gridWidth - 1)
                {
                    LineRenderer line = point.connections[connectionIndex];
                    line.SetPosition(0, point.currentPosition);
                    line.SetPosition(1, gridPoints[x + 1, y].currentPosition);

                    // Line color based on distance
                    float avgDistance = (point.distanceToCursor +
                        gridPoints[x + 1, y].distanceToCursor) / 2f;
                    float alpha = Mathf.Lerp(0.5f, 0.1f, avgDistance / interactionRadius);
                    line.startColor = new Color(1f, 1f, 1f, alpha);
                    line.endColor = new Color(1f, 1f, 1f, alpha);

                    connectionIndex++;
                }

                // Update bottom connection
                if (y < gridHeight - 1)
                {
                    LineRenderer line = point.connections[connectionIndex];
                    line.SetPosition(0, point.currentPosition);
                    line.SetPosition(1, gridPoints[x, y + 1].currentPosition);

                    float avgDistance = (point.distanceToCursor +
                        gridPoints[x, y + 1].distanceToCursor) / 2f;
                    float alpha = Mathf.Lerp(0.5f, 0.1f, avgDistance / interactionRadius);
                    line.startColor = new Color(1f, 1f, 1f, alpha);
                    line.endColor = new Color(1f, 1f, 1f, alpha);
                }
            }
        }
    }
}