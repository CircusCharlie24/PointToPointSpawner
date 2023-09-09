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
        private VisualElement _container;
       [SerializeField] private SpawnData _spawnData;
        private Transform _selectedTransform;

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
            DestroySpawnData();
        }

        private void DestroySpawnData()
        {
            if(_spawnData != null)
            {
                DestroyImmediate(_spawnData);
            }
        }

        private void LoadSpawnData()
        {
            _spawnData = ScriptableObject.CreateInstance<SpawnData>();
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
            BindSpawnData();
        }

        private void BindSpawnData()
        {

            // bind to prefab
            var prefabField = _container.Q<ObjectField>("Prefab");
            prefabField.objectType = typeof(GameObject);
            prefabField.value = _spawnData.prefab;
            prefabField.bindingPath = nameof(_spawnData.prefab);

            // bind to radius
            var radiusField = _container.Q<FloatField>("Radius");
            radiusField.value = _spawnData.radius;
            radiusField.bindingPath = nameof(_spawnData.radius);

            // bind to spawnCount
            var spawnCountField = _container.Q<IntegerField>("SpawnCount");
            spawnCountField.value = _spawnData.spawnCount;
            spawnCountField.bindingPath = nameof(_spawnData.spawnCount);

            // bind to discDirection
            var discDirectionField = _container.Q<EnumField>("DirectionNormal");
            discDirectionField.Init(_spawnData.dirNormal);
            discDirectionField.bindingPath = nameof(_spawnData.dirNormal);

            rootVisualElement.Bind(new SerializedObject(_spawnData));

            // bind to spawn button
            var spawnButton = _container.Q<Button>("SpawnButton");
            spawnButton.clicked += DrawCubes;

        }

        private void OnSceneGUI(SceneView view)
        {
            _selectedTransform = Selection.activeTransform;

            if (_selectedTransform == null)
            {
                return;
            }

            Handles.DrawWireDisc(_selectedTransform.position, GetDiscNormal(), _spawnData.radius);
        }
        private Vector3 GetDiscNormal()
        {
            Vector3 direction = Vector3.zero;
            switch (_spawnData.dirNormal)
            {
                case DirectionNormal.Up:
                    direction = Vector3.up;
                    break;
                case DirectionNormal.Right:
                    direction = Vector3.right;
                    break;
                case DirectionNormal.Forward:
                    direction = Vector3.forward;
                    break;
            }
            return direction;
        }


        private void DrawCubes()
        {

            // draw a circle and get points on the boundary
            Vector3[] points = GetPointsOnCircumference(_selectedTransform.position, _spawnData.radius, _spawnData.spawnCount);

            foreach (Vector3 point in points)
            {
                GameObject newPoint = GameObject.Instantiate(_spawnData.prefab, point, Quaternion.identity);
                newPoint.transform.parent = _selectedTransform;
                newPoint.transform.LookAt(_selectedTransform.position);
            }
        }

        public Vector3[] GetPointsOnCircumference(Vector3 center, float radius, int points)
        {
            Vector3[] result = new Vector3[points];
            float slice = 2 * Mathf.PI / points;
            for (int i = 0; i < points; i++)
            {
                float angle = slice * i;
                result[i] = CalculatePosition(_spawnData.dirNormal, center, radius, angle);
            }

            return result;
        }

        public static Vector3 CalculatePosition(DirectionNormal discDirection, Vector3 center, float radius, float angle)
        {
            float x = center.x;
            float y = center.y;
            float z = center.z;

            switch (discDirection)
            {
                case DirectionNormal.Up:
                    x += radius * Mathf.Cos(angle);
                    z += radius * Mathf.Sin(angle);
                    break;
                case DirectionNormal.Right:
                    y += radius * Mathf.Cos(angle);
                    z += radius * Mathf.Sin(angle);
                    break;
                case DirectionNormal.Forward:
                    x += radius * Mathf.Cos(angle);
                    y += radius * Mathf.Sin(angle);
                    break;
            }

            return new Vector3(x, y, z);
        }
    }

}