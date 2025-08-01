using UnityEditor;

[CustomEditor(typeof(Game.Data.BuildingData))]
public class BuildingData : Editor
{
    private SerializedProperty _name;
    private SerializedProperty _icon;
    private SerializedProperty _prefab;
    private SerializedProperty _size;
    private SerializedProperty _buildMaterials;
    private SerializedProperty _isItemProducer;
    private SerializedProperty _producibleItems;
    private SerializedProperty _isItemProcessor;
    private SerializedProperty _recipes;

    private void OnEnable()
    {
        _name = serializedObject.FindProperty("_name");
        _icon = serializedObject.FindProperty("_icon");
        _prefab = serializedObject.FindProperty("_prefab");
        _size = serializedObject.FindProperty("_size");
        _buildMaterials = serializedObject.FindProperty("_buildMaterials");
        _isItemProducer = serializedObject.FindProperty("_isItemProducer");
        _producibleItems = serializedObject.FindProperty("_producibleItems");
        _isItemProcessor = serializedObject.FindProperty("_isItemProcessor");
        _recipes = serializedObject.FindProperty("_recipes");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(_name);
        EditorGUILayout.PropertyField(_icon);
        EditorGUILayout.PropertyField(_prefab);
        EditorGUILayout.PropertyField(_size);
        EditorGUILayout.PropertyField(_buildMaterials);
        EditorGUILayout.PropertyField(_isItemProducer);
        if (_isItemProducer.boolValue)
        {
            EditorGUILayout.PropertyField(_producibleItems);
        }
        EditorGUILayout.PropertyField(_isItemProcessor);
        if (_isItemProcessor.boolValue)
        {
            EditorGUILayout.PropertyField(_recipes);
        }
        serializedObject.ApplyModifiedProperties();
    }
}