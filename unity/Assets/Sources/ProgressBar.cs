using UnityEngine;

namespace Assets.Sources
{
    public class ProgressBar : MonoBehaviour
    {
        public Transform Bar;
        private float _startPosX;

        [Range(0,1)]
        public float Percent;

        public void Update()
        {
            SetProgress(Percent);
        }

        public void Awake()
        {
            _startPosX = Bar.transform.localScale.x;
        }

        public void SetProgress(float percent)
        {
            Percent = Mathf.Clamp01(percent);
            var pos = Bar.transform.localScale;
            pos.x = _startPosX*percent;
            Bar.transform.localScale = pos;
        }
    }
}
