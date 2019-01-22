using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenGL;

namespace SystemTrayApp
{
    public class GlCamera
    {
        private static float k_camera_mouse_zoom_scalar = 0.01f;
        private static float k_camera_mouse_pan_scalar = 0.5f;
        private static float k_camera_min_zoom = 1.0f;

        private PerspectiveProjectionMatrix _projectionMatrix;
        public PerspectiveProjectionMatrix ProjectionMatrix
        {
            get { return _projectionMatrix; }
        }

        private ModelMatrix _cameraViewMatrix;
        public ModelMatrix CameraViewMatrix
        {
            get { return _cameraViewMatrix; }
        }

        public Matrix4x4 ViewProjectionMatrix
        {
            get { return _projectionMatrix * _cameraViewMatrix; }
        }

        private float _cameraXOffset;
        public float XOffset
        {
            get { return _cameraXOffset; }
            set { _cameraXOffset = value; }
        }

        private float _cameraYOffset;
        public float YOffset
        {
            get { return _cameraYOffset; }
            set { _cameraYOffset = value; }
        }

        private float _cameraZOffset;
        public float ZOffset
        {
            get { return _cameraZOffset; }
            set { _cameraZOffset = value; }
        }

        private float _cameraOrbitYawDegrees;
        private float _cameraOrbitPitchDegrees;
        private float _cameraOrbitRadius;
        private Vertex3 _cameraPosition;
        private bool _isPanningOrbitCamera;
        private bool _isLocked;

        public GlCamera()
        {
            _cameraViewMatrix = new ModelMatrix();
            _projectionMatrix = new PerspectiveProjectionMatrix();

            _cameraOrbitYawDegrees = 0.0f;
            _cameraOrbitPitchDegrees = 0.0f;
            _cameraOrbitRadius = 1.0f;
            _cameraXOffset = 0.0f;
            _cameraYOffset = 0.0f;
            _cameraZOffset = 0.0f;
            _cameraPosition = new Vertex3(0.0f, 0.0f, 100.0f);
            _isPanningOrbitCamera = false;
            _isLocked = false;
            setCameraOrbitLocation(_cameraOrbitYawDegrees, _cameraOrbitPitchDegrees, _cameraOrbitRadius);
        }

        public void onMouseMotion(int deltaX, int deltaY)
        {
            if (!_isLocked && _isPanningOrbitCamera) {
                float deltaYaw = -(float)deltaX * k_camera_mouse_pan_scalar;
                float deltaPitch = (float)deltaY * k_camera_mouse_pan_scalar;

                setCameraOrbitLocation(
                    _cameraOrbitYawDegrees + deltaYaw,
                    _cameraOrbitPitchDegrees + deltaPitch,
                    _cameraOrbitRadius);
            }
        }

        public void onMouseButtonDown()
        {
            if (!_isLocked) {
                _isPanningOrbitCamera = true;
            }
        }

        public void onMouseButtonUp()
        {
            if (!_isLocked) {
                _isPanningOrbitCamera = false;
            }
        }

        public void onMouseWheel(int scrollAmount)
        {
            if (!_isLocked) {
                float deltaRadius = (float)scrollAmount * k_camera_mouse_zoom_scalar;

                setCameraOrbitLocation(
                    _cameraOrbitYawDegrees,
                    _cameraOrbitPitchDegrees,
                    _cameraOrbitRadius + deltaRadius);
            }
        }

        public void setIsLocked(bool locked)
        {
            if (locked) {
                _isLocked = true;
                _isPanningOrbitCamera = false;
            }
            else {
                _isLocked = false;
            }
        }

        public void setCameraOrbitLocation(float yawDegrees, float pitchDegrees, float radius)
        {
            _cameraOrbitYawDegrees = MathUtility.wrap_degrees(yawDegrees);
            _cameraOrbitPitchDegrees = MathUtility.clampf(pitchDegrees, 0.0f, 60.0f);
            _cameraOrbitRadius = Math.Max(radius, k_camera_min_zoom);

            float yawRadians = MathUtility.degrees_to_radians(_cameraOrbitYawDegrees);
            float pitchRadians = MathUtility.degrees_to_radians(_cameraOrbitPitchDegrees);
            float xzRadiusAtPitch = _cameraOrbitRadius * MathUtility.cosf(pitchRadians);
            _cameraPosition = new Vertex3(
                _cameraXOffset + xzRadiusAtPitch * MathUtility.sinf(yawRadians),
                _cameraYOffset + _cameraOrbitRadius*MathUtility.sinf(pitchRadians),
                _cameraZOffset + xzRadiusAtPitch * MathUtility.cosf(yawRadians));

            _cameraViewMatrix.LookAtTarget(
                _cameraPosition, 
                Vertex3.Zero, // Look at tracking origin
                Vertex3.UnitY); // // Up is up.
        }

        public void setCameraOrbitYaw(float yawDegrees)
        {
            setCameraOrbitLocation(yawDegrees, _cameraOrbitPitchDegrees, _cameraOrbitRadius);
        }

        public void setCameraOrbitPitch(float pitchDegrees)
        {
            setCameraOrbitLocation(_cameraOrbitYawDegrees, pitchDegrees, _cameraOrbitRadius);
        }

        public void setCameraOrbitRadius(float radius)
        {
            setCameraOrbitLocation(_cameraOrbitYawDegrees, _cameraOrbitPitchDegrees, radius);
        }

        public void resetOrientation()
        {
            setCameraOrbitLocation(0.0f, 0.0f, _cameraOrbitRadius);
        }

        public void reset()
        {
            setCameraOrbitLocation(0.0f, 0.0f, k_camera_min_zoom);
        }
    }
}
