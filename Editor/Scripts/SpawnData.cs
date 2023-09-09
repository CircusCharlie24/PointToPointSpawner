using UnityEngine;
using static CodeLibrary24.PointToPointSpawner.Editor.SpawnerWindow;

namespace CodeLibrary24.PointToPointSpawner.Editor
{
    [CreateAssetMenu(fileName = "SpawnData", menuName = "CodeLibrary24/PointToPointSpawner/SpawnData", order = 1)]
    public class SpawnData : ScriptableObject
    {
        public GameObject prefab;
        public  float radius = 1f;
        public int spawnCount = 10;
        public DirectionNormal dirNormal;
    }
}
