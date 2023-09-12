using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CodeLibrary24.PointToPointSpawner.Editor
{
    public partial class SpawnerWindow : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;

        [SerializeField] private SpawnData _spawnData;
        private const string SpawnDataPath = "SpawnData";
        private VisualElement _container;
        private Transform _selectedTransform;
        private VisualElement _dataContainer;
        private VisualElement _spawningContainer;

        private VisualElement _activeSelectionMissingLabel;
        private VisualElement _spawnItemMissingLabel;

        private ScrollView _scrollView;
        private List<Transform> _selectedTransforms;

        private List<Label> _selectedTransformLabels;

        // TODO: Add an enum field to select the shape

        private CircleMaker _circleMaker;
        private ShapeMaker<ShapeData> _selectedShapeMaker;


        [MenuItem("CodeLibrary24/PointToPointSpawner/SpawnerWindow")]
        public static void ShowWindow()
        {
            SpawnerWindow wnd = GetWindow<SpawnerWindow>();
            wnd.titleContent = new GUIContent("SpawnerWindow");
        }

        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void LoadSpawnData()
        {
            _spawnData = Resources.Load<SpawnData>(SpawnDataPath);
            if (_spawnData == null)
            {
                Debug.LogError("SpawnData is null. Create from the asset menu into the resources folder");
            }
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            _container = m_VisualTreeAsset.CloneTree();
            root.Add(_container);

            LoadSpawnData();
            Initialize();
            BindSpawnData();
            OpenSceneView();
        }

        private void Initialize()
        {
            _selectedTransformLabels = new List<Label>();
            InitializeShapes();
        }

        private void InitializeShapes()
        {
            _circleMaker = new CircleMaker(_spawnData.circleData, _spawnData);
            //_selectedShapeMaker = _circleMaker; // TODO: Do this when the shapes enum value changes in editor
        }

        private void OpenSceneView()
        {
            SceneView sceneView = SceneView.lastActiveSceneView;
            if (sceneView == null)
            {
                sceneView = GetWindow<SceneView>();
            }
            sceneView.Focus();
        }

        private void BindSpawnData()
        {
            // bind to prefab
            var prefabField = _container.Q<ObjectField>("ItemToSpawn");
            prefabField.objectType = typeof(GameObject);
            prefabField.value = _spawnData.itemToSpawn;
            prefabField.bindingPath = nameof(_spawnData.itemToSpawn);

            // bind to radius

            // bind to spawnCount
            var spawnCountField = _container.Q<IntegerField>("SpawnCount");
            spawnCountField.value = _spawnData.spawnCount;
            spawnCountField.bindingPath = nameof(_spawnData.spawnCount);


            _dataContainer = _container.Q<VisualElement>("DataContainer");
            _spawningContainer = _container.Q<VisualElement>("SpawningContainer");

            _activeSelectionMissingLabel = _container.Q<VisualElement>("ActiveSelectionMissingLabel");
            _spawnItemMissingLabel = _container.Q<VisualElement>("SpawnItemMissingLabel");

            rootVisualElement.Bind(new SerializedObject(_spawnData));

            _scrollView = _container.Q<ScrollView>("ScrollView");

            // bind to spawn button
            var spawnButton = _container.Q<Button>("SpawnButton");
            spawnButton.clicked += InstantiateObjects;
        }

        private void OnSceneGUI(SceneView view)
        {
            ClearSelectedTransformLabels();

            _selectedTransform = Selection.activeTransform; // TODO: There can be multiple selections here too in case of custom shape
            _selectedTransforms = new List<Transform>(Selection.transforms);

            if (_selectedTransform == null)
            {
                _dataContainer.style.display = DisplayStyle.None;
                _activeSelectionMissingLabel.style.display = DisplayStyle.Flex;
                return;
            }
            else
            {
                _dataContainer.style.display = DisplayStyle.Flex;
                _activeSelectionMissingLabel.style.display = DisplayStyle.None;
            }

            ShowSelectedTransformNames();

            if (_spawnData.itemToSpawn == null)
            {
                _spawningContainer.style.display = DisplayStyle.None;
                _spawnItemMissingLabel.style.display = DisplayStyle.Flex;
                return;
            }
            else
            {
                _spawningContainer.style.display = DisplayStyle.Flex;
                _spawnItemMissingLabel.style.display = DisplayStyle.None;
            }

            //TODO: How do I solve this?       //_selectedShapeMaker.OnGUI();
            _circleMaker.OnGUI();


        }

        private void ShowSelectedTransformNames()
        {
            if (_selectedTransforms.Count == 0)
            {
                AddSelectedTransformLabel("No transforms selected");
            }
            else
            {
                for (int i = 0; i < _selectedTransforms.Count; i++)
                {
                    AddSelectedTransformLabel(_selectedTransforms[i].name);
                }
            }
        }

        private void AddSelectedTransformLabel(string text)
        {
            Label newLabel = new Label(text);
            newLabel.style.color = Color.green;
            newLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            _scrollView.Add(newLabel);
            _selectedTransformLabels.Add(newLabel);
        }

        private void ClearSelectedTransformLabels()
        {
            foreach (Label label in _selectedTransformLabels)
            {
                _scrollView.Remove(label);
            }
            _selectedTransformLabels.Clear();
        }


        private void InstantiateObjects()
        {
            // draw a circle and get points on the boundary
            Vector3[] points = _selectedShapeMaker.GetPointsOnBoundary();

            foreach (Vector3 point in points)
            {
                GameObject newPoint = GameObject.Instantiate(_spawnData.itemToSpawn, point, Quaternion.identity);
                Undo.RegisterCreatedObjectUndo(newPoint, "Spawned Object");
                newPoint.transform.parent = _selectedTransform;
                newPoint.transform.LookAt(_selectedTransform.position);
            }
        }
    }
}