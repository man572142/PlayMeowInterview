using UnityEngine;

namespace PlayMeow.Editor
{
    public class EditorNoteDisplay : MonoBehaviour
    {
#if UNITY_EDITOR
        public EditorNoteBook noteBook;
        public int noteIndex;
#endif
    }
}
