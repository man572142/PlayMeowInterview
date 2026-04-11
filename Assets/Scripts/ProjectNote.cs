using UnityEngine;

namespace PlayMeow.Editor
{
    public class ProjectNote : MonoBehaviour
    {
#if UNITY_EDITOR
        public ProjectNoteBook noteBook;
        public int noteIndex;
#endif
    }
}
