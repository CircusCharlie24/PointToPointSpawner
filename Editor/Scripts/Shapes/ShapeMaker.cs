using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CodeLibrary24.PointToPointSpawner.Editor
{
    public abstract class ShapeMaker<T> where T : ShapeData
    {
        protected T shapeData;
        protected SpawnData spawnData;

        protected Transform selectedTransform;

        protected abstract string UxmlPath { get; }

        public ShapeMaker(T shapeData, SpawnData spawnData)
        {
            this.shapeData = shapeData;
            this.spawnData = spawnData;
        }

        public virtual void OnGUI()
        {
            selectedTransform = Selection.activeTransform;
            DrawPreview();
            DrawShape();
        }

        public abstract VisualElement LoadGUI();
        protected abstract void DrawPreview();
        protected abstract void DrawShape();
        
        public abstract Vector3[] GetPointsOnBoundary();

        protected Vector3 GetDirNormal()
        {
            Vector3 direction = Vector3.zero;
            switch (shapeData.dirNormal)
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

    }
}
