using UnityEngine;
namespace GooglyEyes
{
    public class GooglyEye : MonoBehaviour
    {
        [Tooltip("Googly sensitivity to parent's movement")]
        [Range(0.0005f, 0.01f)]
        public float MovementSensitivity = 0.0005f;

        [Tooltip("Googly sensitivity to gravity")]
        [Range(0.01f, 0.1f)]
        public float GravitySensitivity = 0.025f;

        [Tooltip("How much force is lost when the googly hits the max radius (1 means no force lost)")]
        [Range(0, 1)]
        public float Bounciness = 0.7f;

        [Tooltip("The interpolation factor for the googly movement (lower is faster)")]
        [Range(0, 10)] public float Smoothing = 5;

        [Tooltip("The maximum distance the googly can move from the parent's center")]
        [Range(0, 1000)]
        public float MaxRadius = 0.5f;

        [Tooltip("If true, the googly will set its position in the z-axis")]
        public bool ThreeDimensional = false;

        private Vector3 _lastParentPosition;
        private Vector3 _parentTransformVelocity;
        private Vector3 _lastFramePosition;
        private Vector3 _velocity;
        private float _zOffset;

        void Start()
        {
            _lastParentPosition = transform.parent.position;
            _zOffset = transform.localPosition.z;
        }

        void Update()
        {
            if (transform.parent == null) return; // se não tiver pai, não faz nada

            if (Time.deltaTime > 0f)
            {
                _parentTransformVelocity = (transform.parent.position - _lastParentPosition) / Time.deltaTime;
            }
            else
            {
                _parentTransformVelocity = Vector3.zero;
            }

            _lastParentPosition = transform.parent.position;

        }
        void LateUpdate()
        {

            if (transform.parent == null) return;

            _velocity += Physics.gravity * (Time.deltaTime * GravitySensitivity);
            _velocity -= _parentTransformVelocity * MovementSensitivity;

            var offsetFromCenter = (transform.position - transform.parent.position) + _velocity;

            if (offsetFromCenter.magnitude > MaxRadius)
            {
                offsetFromCenter = offsetFromCenter.normalized * MaxRadius;
                _velocity = Vector3.Reflect(_velocity, offsetFromCenter.normalized) * Bounciness;
            }

            Vector3 targetPosition = transform.parent.position + offsetFromCenter;

            // Se tiver NaN, cancela
            if (float.IsNaN(targetPosition.x) || float.IsNaN(targetPosition.y) || float.IsNaN(targetPosition.z))
                return;

            transform.position = Vector3.MoveTowards(transform.position, transform.parent.position + offsetFromCenter, Time.deltaTime * Smoothing);

            if (ThreeDimensional)
            {
                var localPosition = transform.localPosition;
                localPosition.z = _zOffset;
                transform.localPosition = localPosition;
            }
        }
    }
}
