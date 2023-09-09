using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CodeLibrary24.PointToPointSpawner.Editor
{
    public class SpawnerWindow : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;
        private SpawnData _spawnData;
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
        }

        private void LoadSpawnData()
        {
            if (_spawnData == null)
            {
                _spawnData = Resources.Load<SpawnData>("SpawnData");
                if (_spawnData == null)
                {
                    Debug.LogError("SpawnData is null. Create from the asset menu into the resources folder");
                }
            }
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            VisualElement container = m_VisualTreeAsset.Instantiate();
            root.Add(container);
            LoadSpawnData();
            BindSpawnData();
        }

        private void BindSpawnData()
        {
            string bindingRootObjectName = "_spawnData";
            // bind to prefab
            var prefabField = rootVisualElement.Q<ObjectField>("Prefab");
            prefabField.objectType = typeof(GameObject);
            prefabField.bindingPath = bindingRootObjectName + ".prefab";

            // bind to radius
            var radiusField = rootVisualElement.Q<FloatField>("Radius");
            radiusField.bindingPath = bindingRootObjectName + ".radius";

            // bind to spawnCount
            var spawnCountField = rootVisualElement.Q<IntegerField>("SpawnCount");
            spawnCountField.bindingPath = bindingRootObjectName + ".spawnCount";

            // bind to discDirection
            var discDirectionField = rootVisualElement.Q<EnumField>("DiscDirection");
            discDirectionField.bindingPath = bindingRootObjectName + ".discDirection";
        }

        private void OnSceneGUI(SceneView view)
        {
            if (_selectedTransform == null)
            {
                _selectedTransform = Selection.activeTransform;
                return;
            }
            if (_spawnData == null)
            {
                LoadSpawnData();
                return;
            }

            Handles.DrawWireDisc(_selectedTransform.position, GetDiscDirection(), _spawnData.radius);
        }


        private Vector3 GetDiscDirection()
        {
            Vector3 direction = Vector3.zero;
            switch (_spawnData.discDirection)
            {
                case DiscDirection.Up:
                    direction = Vector3.up;
                    break;
                case DiscDirection.Right:
                    direction = Vector3.right;
                    break;
                case DiscDirection.Forward:
                    direction = Vector3.forward;
                    break;
            }
            return direction;
        }

        [MenuItem("CircusCharlie/Draw Circumference")] // TODO: Where does this belong?

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
                result[i] = CalculatePosition(_spawnData.discDirection, center, radius, angle);
            }

            return result;
        }

        public static Vector3 CalculatePosition(DiscDirection discDirection, Vector3 center, float radius, float angle)
        {
            float x = center.x;
            float y = center.y;
            float z = center.z;

            switch (discDirection)
            {
                case DiscDirection.Up:
                    x += radius * Mathf.Cos(angle);
                    z += radius * Mathf.Sin(angle);
                    break;
                case DiscDirection.Right:
                    y += radius * Mathf.Cos(angle);
                    z += radius * Mathf.Sin(angle);
                    break;
                case DiscDirection.Forward:
                    x += radius * Mathf.Cos(angle);
                    y += radius * Mathf.Sin(angle);
                    break;
            }

            return new Vector3(x, y, z);
        }

        public enum DiscDirection
        {
            Up,
            Right,
            Forward
        }
    }

}