using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sumfulla.Atomica
{
    public class CameraZoomer : MonoBehaviour
    {
        private Coroutine _zooming;

        // Hardcoded values based on trial-and-error to make screen filled when zoom applied
        private readonly Dictionary<int, (float, float)> _zPos = new Dictionary<int, (float, float)>
        {
            { 1, (-0.9f, -0.06f)},
            { 3, (-1.13f, -0.0665f) },
            { 7, (-1.26f, -0.073f) },
            { 15, (-1.39f, -0.0795f) },
            { 31, (-1.42f, -0.086f) },
            { 63, (-1.55f, -0.0925f) },
            { 127, (-1.7f, -0.1f) }
        };

        /// <summary>
        /// Returns member value where bitmask drops off
        /// </summary>
        private int TrailingOnes(int mask)
        {
            int count = 0;

            while ((mask & 1) == 1)
            {
                count++;
                mask >>= 1;
            }

            return count;
        }

        /// <summary>
        /// Initial call to change camera position when bitmask value is updated
        /// </summary>
        public void CameraDepthFromMask(int mask)
        {
            int level = TrailingOnes(mask);

            // safety clamp: if mask somehow produces >7 trailing ones
            if (level < 0 || level >= _zPos.Count)
            {
                Debug.LogWarning($"Invalid level index: {level}");
                return;
            }

            DoReposition(level);
        }

        /// <summary>
        /// Checks and kills any current zooming coroutine and invokes new coroutine
        /// </summary>
        public void DoReposition(int z)
        {
            if (z == 0) return;
            if (_zooming != null)
            {
                StopCoroutine(_zooming);
            }
            _zooming = StartCoroutine(Reposition(z));

        }

        /// <summary>
        /// Lerps from current positon to new position based on earlier bitmask calulation
        /// </summary>
        private IEnumerator Reposition(int z)
        {
            Vector3 currentPosition = transform.position;
            Vector3 endPosition = new Vector3(0, _zPos[z].Item2, _zPos[z].Item1);

            float t = 0;
            while (t < 1f)
            {
                transform.position = Vector3.Lerp(currentPosition, endPosition, t);

                t += Time.deltaTime;

                yield return null;
            }
            transform.position = endPosition;
        }

    }
}