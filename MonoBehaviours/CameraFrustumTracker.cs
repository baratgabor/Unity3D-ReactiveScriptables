using UnityEngine;

namespace LeakyAbstraction.ReactiveScriptables
{
    [RequireComponent(typeof(Camera))]
    public class CameraFrustumTracker : MonoBehaviour
    {
        [SerializeField]
        private BoundsState_Writeable _cameraBoundsOutput = default;

        private Camera _camera;
        private Bounds _previousbounds = default;

        private float _previousScreenWidth = default;
        private float _previousScreenHeight = default;

        void Awake()
        {
            _camera = GetComponent<Camera>();

            _previousScreenWidth = Screen.width;
            _previousScreenHeight = Screen.height;

            var bounds = OrthographicBounds(_camera);
            _cameraBoundsOutput.Set(bounds);
            _previousbounds = bounds;
        }

        private void Update()
        {
            if (Screen.width != _previousScreenWidth || Screen.height != _previousScreenHeight)
                _cameraBoundsOutput.Set(OrthographicBounds(_camera));

            //var newBounds = OrthographicBounds(_camera);
            //if (newBounds != _previousbounds)
            //{
            //    _cameraBoundsOutput.Value = newBounds;
            //    _previousbounds = newBounds;
            //}
        }

        public static Bounds OrthographicBounds(Camera camera)
        {
            float screenAspect = (float)Screen.width / (float)Screen.height;
            float cameraHeight = camera.orthographicSize * 2;
            Bounds bounds = new Bounds(
                camera.transform.position,
                new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
            return bounds;
        }

    }
}
