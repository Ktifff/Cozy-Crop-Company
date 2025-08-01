using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    public class FieldGeneratorEditor : EditorWindow
    {
        private GameObject _blockPrefabA;
        private GameObject _blockPrefabB;
        private int _width = 10;
        private int _depth = 10;
        private Vector2 _blockSize = Vector2.one;
        private string _parentName = "GeneratedField";

        [MenuItem("Tools/Field Generator")]
        public static void ShowWindow()
        {
            GetWindow<FieldGeneratorEditor>("Field Generator");
        }

        private void OnGUI()
        {
            GUILayout.Label("Field settings", EditorStyles.boldLabel);
            _blockPrefabA = (GameObject)EditorGUILayout.ObjectField("Block prefab A", _blockPrefabA, typeof(GameObject), false);
            _blockPrefabB = (GameObject)EditorGUILayout.ObjectField("Block prefab B", _blockPrefabB, typeof(GameObject), false);
            _width = EditorGUILayout.IntField("Width", _width);
            _depth = EditorGUILayout.IntField("Depth", _depth);
            _blockSize = EditorGUILayout.Vector2Field("Block size", _blockSize);
            _parentName = EditorGUILayout.TextField("Parent object name", _parentName);
            GUILayout.Space(10);
            if (GUILayout.Button("Generate field"))
            {
                GenerateField();
            }
        }

        private void GenerateField()
        {
            if (_blockPrefabA == null || _blockPrefabB == null)
            {
                Debug.LogWarning("Please assign both block prefabs");
                return;
            }
            GameObject parent = new GameObject(_parentName);
            for (int x = 0; x < _width; x++)
            {
                for (int z = 0; z < _depth; z++)
                {
                    GameObject selectedPrefab = (x + z) % 2 == 0 ? _blockPrefabA : _blockPrefabB;
                    Vector3 position = new Vector3(x * _blockSize.x, 0, z * _blockSize.y);
                    GameObject block = (GameObject)PrefabUtility.InstantiatePrefab(selectedPrefab);
                    block.transform.position = position;
                    block.transform.SetParent(parent.transform);
                    block.name = $"Block_{x}_{z}";
                }
            }
        }
    }
}