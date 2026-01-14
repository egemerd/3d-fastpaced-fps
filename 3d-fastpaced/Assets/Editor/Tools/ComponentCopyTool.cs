using UnityEditor;
using UnityEngine;

public class ComponentCopyTool : EditorWindow
{
    private GameObject sourceObject;
    private GameObject targetObject;

    [MenuItem("Tools/Component Copy Tool")]
    public static void ShowWindow()
    {
        GetWindow<ComponentCopyTool>("Copy Components");
    }

    private void OnGUI()
    {
        GUILayout.Label("Copy Multiple Components", EditorStyles.boldLabel);

        sourceObject = (GameObject)EditorGUILayout.ObjectField("Source Object", sourceObject, typeof(GameObject), true);
        targetObject = (GameObject)EditorGUILayout.ObjectField("Target Object", targetObject, typeof(GameObject), true);

        if (GUILayout.Button("Copy All Components"))
        {
            CopyAllComponents();
        }
    }

    private void CopyAllComponents()
    {
        if (sourceObject == null || targetObject == null)
        {
            Debug.LogError("Source or Target object is null!");
            return;
        }

        Component[] components = sourceObject.GetComponents<Component>();

        foreach (Component comp in components)
        {
            // Transform ve GameObject'i atla
            if (comp is Transform) continue;

            // Component tipini al
            System.Type type = comp.GetType();

            // Hedef objede ayný tip varsa kopyala, yoksa ekle
            Component targetComp = targetObject.GetComponent(type);
            if (targetComp == null)
            {
                targetComp = targetObject.AddComponent(type);
            }

            // Component'in tüm field'larýný kopyala
            UnityEditorInternal.ComponentUtility.CopyComponent(comp);
            UnityEditorInternal.ComponentUtility.PasteComponentValues(targetComp);
        }

        Debug.Log($"Copied {components.Length - 1} components from {sourceObject.name} to {targetObject.name}");
    }
}
