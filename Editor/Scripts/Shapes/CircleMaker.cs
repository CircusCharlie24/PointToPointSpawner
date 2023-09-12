using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CodeLibrary24.PointToPointSpawner.Editor
{
    public class CircleMaker : ShapeMaker<CircleData>
    {
        protected override string UxmlPath => "";

        public CircleMaker(CircleData shapeData, SpawnData spawnData) : base(shapeData, spawnData)
        {
        }

        public override VisualElement LoadGUI()
        {
            VisualElement container = new VisualElement(); // TODO: Load from path
            var radiusField = container.Q<FloatField>("Radius");
            radiusField.value = shapeData.radius;
            radiusField.bindingPath = nameof(shapeData.radius);

            // bind to normal direction field
            var dirNormal = container.Q<EnumField>("DirectionNormal");
            dirNormal.Init(shapeData.dirNormal);
            dirNormal.bindingPath = nameof(shapeData.dirNormal);


            return container; // TODO: To be added in by the spawner window
        }


        protected override void DrawShape()
        {
            Handles.DrawWireDisc(selectedTransform.position, GetDirNormal(), shapeData.radius);
        }

        public override Vector3[] GetPointsOnBoundary()
        {
            return GetPointsOnCircumference(selectedTransform.position, shapeData.radius, spawnData.spawnCount);
        }

        protected override void DrawPreview()
        {
            Renderer renderer = spawnData.itemToSpawn.GetComponent<Renderer>();
            if (renderer == null)
            {
                return;
            }

            Material material = renderer.sharedMaterial;
            if (material == null)
            {
                return;
            }

            Mesh mesh = spawnData.itemToSpawn.GetComponent<MeshFilter>().sharedMesh;
            if (mesh == null)
            {
                return;
            }

            material.SetPass(0);

            Transform itemToSpawn = spawnData.itemToSpawn.transform;

            Vector3[] points = GetPointsOnCircumference(selectedTransform.position, shapeData.radius, spawnData.spawnCount);
            foreach (Vector3 point in points)
            {
                Graphics.DrawMeshNow(mesh, Matrix4x4.TRS(point, RotateTowards(point, itemToSpawn.transform.position), itemToSpawn.transform.localScale));
            }
        }

        private Quaternion RotateTowards(Vector3 sourcePosition, Vector3 targetPosition)
        {
            Vector3 direction = targetPosition - sourcePosition;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            return lookRotation;
        }

        public Vector3[] GetPointsOnCircumference(Vector3 center, float radius, int points)
        {
            Vector3[] result = new Vector3[points];
            float slice = 2 * Mathf.PI / points;
            for (int i = 0; i < points; i++)
            {
                float angle = slice * i;
                result[i] = CalculatePosition(shapeData.dirNormal, center, radius, angle);
            }

            return result;
        }

        public static Vector3 CalculatePosition(DirectionNormal dirNormal, Vector3 center, float radius, float angle)
        {
            float x = center.x;
            float y = center.y;
            float z = center.z;

            switch (dirNormal)
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
