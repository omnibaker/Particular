using UnityEngine;

namespace Sumfulla.Atomica
{
    public class AnimateBackground : MonoBehaviour
    {
        [SerializeField] private Transform _bouncingObject = null;
        [SerializeField] private float _distance = 2f;
        [SerializeField] private float _rate = 2f;
        [SerializeField] private float _startTimeOffset = 0;

        private enum MovementType { Oscilate, Force }
        private Vector3 _startPosition;
        private float _oscFactor;

        private void Start()
        {
            _startPosition = _bouncingObject.localPosition;
        }

        private void Update()
        {
            OscillateBackAndForth();
        }
        
        /// <summary>
        /// Slowly moves camera left and right to give some live ambiance
        /// </summary>
        private void OscillateBackAndForth()
        {
            _oscFactor = Mathf.Sin((Time.time + _startTimeOffset) * _rate);
            _bouncingObject.localPosition = _startPosition + _distance * _oscFactor * Vector3.right;
        }
    }
}