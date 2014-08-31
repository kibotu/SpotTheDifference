using UnityEngine;

namespace Assets.Sources
{
    public class CameraShake : MonoBehaviour
    {
        private bool Shaking;
        private float CurrentShakeIntensity;
        private float CurrentShakeDecay;
        private Vector3 OriginalPos;
        private Quaternion OriginalRot;

        public float ShakeIntensity;
        public float ShakeDecay;

        // Update is called once per frame
        private void Update()
        {
            if (CurrentShakeIntensity > 0)
            {
                var pos = OriginalPos + Random.insideUnitSphere*CurrentShakeIntensity;
                pos.z = -10;
                transform.position = pos;
                transform.rotation = new Quaternion(OriginalRot.x + Random.Range(-CurrentShakeIntensity, CurrentShakeIntensity)*.2f,
                    OriginalRot.y + Random.Range(-CurrentShakeIntensity, CurrentShakeIntensity)*.2f,
                    OriginalRot.z ,
                    OriginalRot.w + Random.Range(-CurrentShakeIntensity, CurrentShakeIntensity)*.2f);

                CurrentShakeIntensity -= CurrentShakeDecay;
            }
            else if (Shaking)
            {
                Shaking = false;
            }
        }

        public void DoShake()
        {
            OriginalPos = transform.position;
            OriginalRot = transform.rotation;

            CurrentShakeIntensity = ShakeIntensity;
            CurrentShakeDecay = ShakeDecay;
            Shaking = true;
        }
    }
} 