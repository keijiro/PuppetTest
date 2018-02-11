using UnityEngine;
using UnityEditor;

namespace Puppet
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Dancer))]
    public class DancerEditor : Editor
    {
        SerializedProperty _footDistance;
        SerializedProperty _stepFrequency;
        SerializedProperty _stepHeight;
        SerializedProperty _stepAngle;
        SerializedProperty _maxDistance;

        SerializedProperty _hipHeight;
        SerializedProperty _hipPositionNoise;
        SerializedProperty _hipRotationNoise;

        SerializedProperty _spineBend;
        SerializedProperty _spineRotationNoise;

        SerializedProperty _handPosition;
        SerializedProperty _handPositionNoise;

        SerializedProperty _headMove;

        SerializedProperty _noiseFrequency;
        SerializedProperty _randomSeed;

        static class Styles
        {
            static public GUIContent bendAngle = new GUIContent("Bend Angle");
            static public GUIContent basePosition = new GUIContent("Base Position");
            static public GUIContent lookAtMove = new GUIContent("Look At Move");
            static public GUIContent noiseToPosition = new GUIContent("Position Noise");
            static public GUIContent noiseToRotation = new GUIContent("Rotation Noise");
        }


        void OnEnable()
        {
            _footDistance = serializedObject.FindProperty("_footDistance");
            _stepFrequency = serializedObject.FindProperty("_stepFrequency");
            _stepHeight = serializedObject.FindProperty("_stepHeight");
            _stepAngle = serializedObject.FindProperty("_stepAngle");
            _maxDistance = serializedObject.FindProperty("_maxDistance");

            _hipHeight = serializedObject.FindProperty("_hipHeight");
            _hipPositionNoise = serializedObject.FindProperty("_hipPositionNoise");
            _hipRotationNoise = serializedObject.FindProperty("_hipRotationNoise");

            _spineBend = serializedObject.FindProperty("_spineBend");
            _spineRotationNoise = serializedObject.FindProperty("_spineRotationNoise");

            _handPosition = serializedObject.FindProperty("_handPosition");
            _handPositionNoise = serializedObject.FindProperty("_handPositionNoise");

            _headMove = serializedObject.FindProperty("_headMove");

            _noiseFrequency = serializedObject.FindProperty("_noiseFrequency");
            _randomSeed = serializedObject.FindProperty("_randomSeed");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_randomSeed);
            EditorGUILayout.PropertyField(_noiseFrequency);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Foot/Step", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_footDistance);
            EditorGUILayout.PropertyField(_stepFrequency);
            EditorGUILayout.PropertyField(_stepHeight);
            EditorGUILayout.PropertyField(_stepAngle);
            EditorGUILayout.PropertyField(_maxDistance);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Hip", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_hipHeight);
            EditorGUILayout.PropertyField(_hipPositionNoise, Styles.noiseToPosition);
            EditorGUILayout.PropertyField(_hipRotationNoise, Styles.noiseToRotation);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Spine", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_spineBend, Styles.bendAngle);
            EditorGUILayout.PropertyField(_spineRotationNoise, Styles.noiseToRotation);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Hand", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_handPosition, Styles.basePosition);
            EditorGUILayout.PropertyField(_handPositionNoise, Styles.noiseToPosition);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Head", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_headMove, Styles.lookAtMove);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
