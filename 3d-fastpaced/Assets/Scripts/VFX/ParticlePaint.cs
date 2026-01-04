using System;
using UnityEngine;
using UnityEngine.UIElements;

public class ParticlePaint : MonoBehaviour
{
    [Header("Layer Settings")]
    [SerializeField] private LayerMask targetLayers;

    [Header("Decal Settings")]
    [SerializeField] private Material decalMaterial; // Inspector'dan circle material atayýn
    [SerializeField] private Color paintColor = Color.white;
    [SerializeField] private float decalSize = 0.2f;
    [SerializeField] private float decalLifetime = 5f;

    [Header("Advanced Settings")]
    [SerializeField] private float decalOffset = 0.001f; // Z-fighting önlemek için
    [SerializeField] private bool randomRotation = true;
    [SerializeField] private bool useFade = true;
    [SerializeField] private float fadeStartTime = 4f;


    private ParticleSystem ps;
    private ParticleCollisionEvent[] collisionEvents;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        collisionEvents = new ParticleCollisionEvent[16];
    }

    private bool IsInTargetLayer(GameObject obj)
    {
        // GameObject'in layer'ýný kontrol et
        return ((1 << obj.layer) & targetLayers) != 0;
    }


    private void OnParticleCollision(GameObject other)
    {

        if (!IsInTargetLayer(other))
        {
            return; // Bu layer'da deðilse iþlem yapma
        }

        int numCollisionEvents = ps.GetCollisionEvents(other, collisionEvents);
        for (int i = 0; i < numCollisionEvents; i++)
        {
            Vector3 position = collisionEvents[i].intersection;
            Vector3 normal = collisionEvents[i].normal;

            CreateDecalQuad(position, normal, other);
        }
    }

    private void CreateDecalQuad(Vector3 position, Vector3 normal, GameObject hitObject)
    {
        // Quad oluþtur
        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.name = "PaintDecal";

        // Collider'ý kaldýr (decal'a gerek yok)
        Collider collider = quad.GetComponent<Collider>();
        if (collider != null)
        {
            Destroy(collider);
        }

        // Pozisyon ayarla - yüzeyden hafif yukarý kaldýr
        quad.transform.position = position + normal * decalOffset;

        // Rotasyon ayarla - normal'e göre yönlendir
        quad.transform.rotation = Quaternion.LookRotation(normal);

        // Rastgele Z rotasyonu ekle
        if (randomRotation)
        {
            quad.transform.Rotate(0, 0, UnityEngine.Random.Range(0f, 360f));
        }

        // Scale ayarla
        quad.transform.localScale = Vector3.one * decalSize;

        // Parent'ý hit object'e ata (obje hareket ederse decal da takip eder)
        quad.transform.SetParent(hitObject.transform);

        // Material ve renk ayarla
        MeshRenderer renderer = quad.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            if (decalMaterial != null)
            {
                // Material'i clone'la (shared material deðiþmesin)
                renderer.material = new Material(decalMaterial);
                renderer.material.color = paintColor;
            }
            else
            {
                // Fallback: Basit unlit material
                Material fallbackMat = new Material(Shader.Find("Unlit/Color"));
                fallbackMat.color = paintColor;
                renderer.material = fallbackMat;
            }

            // Shadow kapatma (performans)
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;
        }

        // Fade ve destroy
        if (useFade)
        {
            DecalFadeDestroy fader = quad.AddComponent<DecalFadeDestroy>();
            fader.Setup(decalLifetime, fadeStartTime);
        }
        else
        {
            Destroy(quad, decalLifetime);
        }
    }
}
public class DecalFadeDestroy : MonoBehaviour
{
    private float lifetime;
    private float fadeStartTime;
    private float timer = 0f;
    private Material material;
    private Color originalColor;

    public void Setup(float life, float fadeStart)
    {
        lifetime = life;
        fadeStartTime = fadeStart;

        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            material = renderer.material;
            originalColor = material.color;
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;

        // Fade efekti
        if (material != null && timer >= fadeStartTime)
        {
            float fadeProgress = (timer - fadeStartTime) / (lifetime - fadeStartTime);
            float alpha = Mathf.Lerp(originalColor.a, 0f, fadeProgress);
            Color newColor = originalColor;
            newColor.a = alpha;
            material.color = newColor;
        }

        // Yok et
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}


