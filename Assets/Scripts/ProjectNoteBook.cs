using System.Collections.Generic;
using UnityEngine;

namespace PlayMeow.Editor
{
    [CreateAssetMenu(fileName = "NewEditorNoteBook", menuName = "PlayMeow/Editor Note Book")]
    public class ProjectNoteBook : ScriptableObject
    {
        public List<ProjectNoteData> notes = new List<ProjectNoteData>();

        public string[] GetNoteNames()
        {
            if (notes == null || notes.Count == 0)
            {
                return new string[] { "(No Notes)" };
            }

            var names = new string[notes.Count];
            for (int i = 0; i < notes.Count; i++)
            {
                names[i] = string.IsNullOrEmpty(notes[i].name) ? $"(Unnamed #{i})" : notes[i].name;
            }

            return names;
        }
    }
}