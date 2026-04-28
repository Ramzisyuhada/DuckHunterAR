using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpawnDuckTool))]
public class SpawnDuckToolEditor : Editor
{
    private SpawnDuckTool tool;
    private bool enableSceneClick = true;

    private Vector3 previewPosition;
    private bool hasHit = false;

    private void OnEnable()
    {
        tool = (SpawnDuckTool)target;
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    public override void OnInspectorGUI()
    {
        GUILayout.Label("Spawn Duck Tool", EditorStyles.boldLabel);

        tool.objectName = EditorGUILayout.TextField("Name", tool.objectName);

        enableSceneClick = EditorGUILayout.Toggle("Enable Scene Spawn", enableSceneClick);

        EditorGUILayout.HelpBox("SHIFT + Click di Scene untuk spawn object", MessageType.Info);

        if (GUILayout.Button("Spawn di Posisi Object Ini"))
        {
            CreateObject(tool.transform.position);
        }
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (!enableSceneClick) return;

        Event e = Event.current;

        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

        // cek posisi mouse di dunia
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            previewPosition = hit.point;
            hasHit = true;

            // 🎯 DRAW PREVIEW SPHERE
            Handles.color = new Color(0, 1, 0, 0.5f);
            Handles.SphereHandleCap(0, previewPosition, Quaternion.identity, 0.5f, EventType.Repaint);
        }
        else
        {
            hasHit = false;
        }

        // klik untuk spawn
        if (e.type == EventType.MouseDown && e.button == 0 && e.shift && hasHit)
        {
            CreateObject(previewPosition);
            e.Use();
        }
    }

    void CreateObject(Vector3 position)
    {
        GameObject obj = new GameObject(tool.objectName);
        obj.transform.position = position;
        obj.transform.parent = tool.transform;

        // 🎯 Tambahkan visual sphere permanen
        obj.AddComponent<SpawnVisual>();

        Undo.RegisterCreatedObjectUndo(obj, "Spawn Object");
        Selection.activeGameObject = obj;
    }
}