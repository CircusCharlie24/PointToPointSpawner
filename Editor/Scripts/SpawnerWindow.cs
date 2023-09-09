using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class SpawnerWindow : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("CodeLibrary24/PointToPointSpawner/SpawnerWindow")]
    public static void ShowWindow()
    {
        SpawnerWindow wnd = GetWindow<SpawnerWindow>();
        wnd.titleContent = new GUIContent("SpawnerWindow");
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        VisualElement container = m_VisualTreeAsset.Instantiate();
        root.Add(container);
    }
}
