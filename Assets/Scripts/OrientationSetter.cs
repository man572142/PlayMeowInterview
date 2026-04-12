using UnityEngine;

namespace PlayMeow
{
    public class OrientationSetter : MonoBehaviour
    {
        [SerializeField] private ScreenOrientation orientation;

        void Start()
        {
            Screen.orientation = orientation;
        }
    }
}
