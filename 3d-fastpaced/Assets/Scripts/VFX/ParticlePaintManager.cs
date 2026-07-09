using UnityEngine;

public class ParticlePaintManager : MonoBehaviour
{
    public static ParticlePaintManager Instance { get; private set; }


    [SerializeField] private ParticlePaintSO particlePaintSettings;
    public Color CurrentColor { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Sahne geçiþlerinde Manager'ýn kalýcý olmasýný istemiyorsan bu satýrý sil
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        CurrentColor = Color.white;
    }
    public void ApplyOverride(Color color)
    {
        CurrentColor = color;
    }


}
