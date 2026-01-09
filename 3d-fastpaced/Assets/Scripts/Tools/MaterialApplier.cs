#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class MaterialApplier : EditorWindow
{
    private GameObject targetParent;
    private Material materialToApply;

    [MenuItem("Tools/Apply Material to Children")]

    
    public static void ShowWindow()
    {
        GetWindow<MaterialApplier>("Material Applier");
    }

    private void OnGUI()
    {
        GUILayout.Label("Apply Material to All Children", EditorStyles.boldLabel);

        targetParent = (GameObject)EditorGUILayout.ObjectField(
            "Parent Object",
            targetParent,
            typeof(GameObject),
            true
        );

        materialToApply = (Material)EditorGUILayout.ObjectField(
            "Material",
            materialToApply,
            typeof(Material),
            false
        );

        GUILayout.Space(10);

        if (GUILayout.Button("Apply Material"))
        {
            if (targetParent == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign a parent object!", "OK");
                return;
            }

            if (materialToApply == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign a material!", "OK");
                return;
            }

            ApplyMaterialToChildren();
        }
    }

    private void ApplyMaterialToChildren()
    {
        // Tüm child objeleri al (kendisi dahil)
        Renderer[] renderers = targetParent.GetComponentsInChildren<Renderer>(true);

        int count = 0;
        foreach (Renderer renderer in renderers)
        {
            Undo.RecordObject(renderer, "Apply Material");

            // Tüm material slot'larýna uygula
            Material[] materials = new Material[renderer.sharedMaterials.Length];
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = materialToApply;
            }
            renderer.sharedMaterials = materials;

            count++;
        }

        EditorUtility.DisplayDialog(
            "Success",
            $"Material applied to {count} renderer(s)!",
            "OK"
        );

        Debug.Log($"[MaterialApplier] Material '{materialToApply.name}' applied to {count} objects under '{targetParent.name}'");
    }
}
#endif