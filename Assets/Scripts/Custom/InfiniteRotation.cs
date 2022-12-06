using UnityEngine;

namespace Custom
{
    public class InfiniteRotation : MonoBehaviour
    {
        public float Speed { get; set; }

        public void Awake()
        {
            Speed = 2.0f;
        }

        public void FixedUpdate()
        {
            transform.Rotate(Vector3.up, Speed * Time.fixedDeltaTime);
        }
    }
}