using UnityEngine;

public class LiquidMeshTrail : MonoBehaviour
{
    [Header("Referanslar")]
    public Material metaballMaterial;
    public Camera mainCamera;

    [Header("Renkler")]
    public Color backgroundColor = new Color(0.05f, 0.05f, 0.1f, 1f);
    public Color blobColor = Color.white;
    public Color glowColor = new Color(0.8f, 0.9f, 1f, 1f);

    [Header("Blob Ayarlarý")]
    [Range(3, 20)]
    public int blobCount = 8;

    [Range(0.1f, 2f)]
    public float blobSize = 0.5f;

    [Range(0f, 1f)]
    public float threshold = 0.5f;

    [Range(0f, 0.5f)]
    public float edgeSoftness = 0.1f;

    [Header("Hareket Ayarlarý")]
    [Range(0f, 2f)]
    public float movementSpeed = 0.3f;

    [Range(0f, 5f)]
    public float mouseInfluence = 2.0f;

    [Range(0f, 2f)]
    public float mouseRadius = 0.8f;

    [Range(1f, 30f)]
    public float mouseSmoothSpeed = 10f;

    [Header("Görsel Efektler")]
    [Range(0f, 5f)]
    public float glowStrength = 2.0f;

    private Vector2 targetMousePos;
    private Vector2 currentMousePos;
    private MeshRenderer meshRenderer;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        // Mesh renderer ve filter ekle
        SetupMesh();

        // Material ayarla
        if (metaballMaterial == null)
        {
            metaballMaterial = new Material(Shader.Find("Custom/MetaballBackground"));
        }

        meshRenderer.material = metaballMaterial;

        // Ýlk deðerleri ayarla
        UpdateMaterialProperties();
    }

    void SetupMesh()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh = CreateFullscreenQuad();
        }
    }

    void Update()
    {
        // Mouse pozisyonunu UV koordinatlarýna çevir
        Vector3 mousePos = Input.mousePosition;
        targetMousePos = new Vector2(
            mousePos.x / Screen.width,
            mousePos.y / Screen.height
        );

        // Smooth mouse hareketi
        currentMousePos = Vector2.Lerp(currentMousePos, targetMousePos, Time.deltaTime * mouseSmoothSpeed);

        // Material'ý güncelle
        UpdateMaterialProperties();
    }

    void UpdateMaterialProperties()
    {
        if (metaballMaterial == null) return;

        // Renkler
        metaballMaterial.SetColor("_BackgroundColor", backgroundColor);
        metaballMaterial.SetColor("_BlobColor", blobColor);
        metaballMaterial.SetColor("_GlowColor", glowColor);

        // Blob ayarlarý
        metaballMaterial.SetFloat("_BlobCount", blobCount);
        metaballMaterial.SetFloat("_BlobSize", blobSize);
        metaballMaterial.SetFloat("_Threshold", threshold);
        metaballMaterial.SetFloat("_EdgeSoftness", edgeSoftness);

        // Hareket ayarlarý
        metaballMaterial.SetFloat("_Speed", movementSpeed);
        metaballMaterial.SetFloat("_MouseInfluence", mouseInfluence);
        metaballMaterial.SetFloat("_MouseRadius", mouseRadius);

        // Görsel efektler
        metaballMaterial.SetFloat("_GlowStrength", glowStrength);

        // Mouse pozisyonu
        metaballMaterial.SetVector("_MousePosition", new Vector4(currentMousePos.x, currentMousePos.y, 0, 0));
    }

    Mesh CreateFullscreenQuad()
    {
        Mesh mesh = new Mesh();
        mesh.name = "Metaball Background Quad";

        // Ekraný tamamen kaplayacak büyüklükte
        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(-10, -10, 0),
            new Vector3(10, -10, 0),
            new Vector3(-10, 10, 0),
            new Vector3(10, 10, 0)
        };

        Vector2[] uv = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };

        int[] triangles = new int[6]
        {
            0, 2, 1,
            2, 3, 1
        };

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    void OnValidate()
    {
        if (Application.isPlaying && metaballMaterial != null)
        {
            UpdateMaterialProperties();
        }
    }
}