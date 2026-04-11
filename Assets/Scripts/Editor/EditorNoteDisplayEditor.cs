using UnityEngine;
using UnityEditor;

namespace PlayMeow.Editor
{
    [CustomEditor(typeof(EditorNoteDisplay))]
    public class EditorNoteDisplayEditor : UnityEditor.Editor
    {
        private SerializedProperty _noteBookProp;
        private SerializedProperty _noteIndexProp;

        private void OnEnable()
        {
            _noteBookProp = serializedObject.FindProperty("noteBook");
            _noteIndexProp = serializedObject.FindProperty("noteIndex");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_noteBookProp);

            var display = (EditorNoteDisplay)target;
            var noteBook = display.noteBook;

            if (noteBook != null && noteBook.notes != null && noteBook.notes.Count > 0)
            {
                string[] noteNames = noteBook.GetNoteNames();

                // Clamp index to valid range
                int currentIndex = Mathf.Clamp(_noteIndexProp.intValue, 0, noteBook.notes.Count - 1);

                int selectedIndex = EditorGUILayout.Popup("Note", currentIndex, noteNames);
                _noteIndexProp.intValue = selectedIndex;

                EditorGUILayout.Space(4);
                EditorGUILayout.LabelField("Description", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox(noteBook.notes[selectedIndex].description, MessageType.Info);
            }
            else if (noteBook != null)
            {
                EditorGUILayout.HelpBox("The note book has no notes. Add notes to the ScriptableObject.", MessageType.Warning);
            }
            else
            {
                EditorGUILayout.HelpBox("Assign an Editor Note Book to display a note.", MessageType.Info);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
