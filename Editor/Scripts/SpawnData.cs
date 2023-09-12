using UnityEngine;
using static CodeLibrary24.PointToPointSpawner.Editor.SpawnerWindow;

namespace CodeLibrary24.PointToPointSpawner.Editor
{
    [CreateAssetMenu(fileName = "SpawnData", menuName = "CodeLibrary24/PointToPointSpawner/SpawnData", order = 1)]
    public class SpawnData : ScriptableObject
    {
        public GameObject itemToSpawn;
        public int spawnCount = 10;
        [SerializeField] public CircleData circleData;
    }
}
