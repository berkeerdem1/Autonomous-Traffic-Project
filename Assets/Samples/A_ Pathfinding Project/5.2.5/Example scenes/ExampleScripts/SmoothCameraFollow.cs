using System;
using System.Collections;
using UnityEngine;

namespace Pathfinding.Examples {
	/// <summary>
	/// Smooth Camera Following.
	/// \author http://wiki.unity3d.com/index.php/SmoothFollow2
	/// </summary>
	[HelpURL("https://arongranberg.com/astar/documentation/stable/smoothcamerafollow.html")]
	public class SmoothCameraFollow : VersionedMonoBehaviour {
		public Transform target;
		public float distance = 3.0f;
		public float height = 3.0f;
		public float damping = 5.0f;
		public bool enableRotation = true;
		public bool smoothRotation = true;
		public float rotationDamping = 10.0f;
		public bool staticOffset = false;
        public float sprintMultiplier = 2.0f;
        public float moveSpeed = 10.0f; // W, A, S, D Momement speed
        public float rotationSpeed = 100.0f;
        public bool isFreeMode = false;
        /// <summary>
        /// 
        /// </summary>

        private void Awake()
        {
            StartCoroutine(wait());
        }

        IEnumerator wait() 
        {
            yield return new WaitForSeconds(1f);

            GameObject[] cars = GameObject.FindGameObjectsWithTag("Car");
            if (cars.Length > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, cars.Length);
                target = cars[randomIndex].transform;
            }
            else
            {
                Debug.LogWarning("Sahnede 'car' tagine sahip obje bulunamadý.");
            }
        }
        void Update()
        {
            HandleInput();
            HandleMouseClick();
        }
        void LateUpdate () 
        {
            if (isFreeMode)
            {
                FreeModeControl();
            }
            else
            {
                FollowTarget(); 
            }
        }

        private void HandleInput()
        {
            if (Input.GetMouseButtonDown(1))
            {
                isFreeMode = !isFreeMode;
            }
        }

        private void HandleMouseClick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.collider.CompareTag("Car"))
                    {
                        target = hit.collider.transform;
                        SetTarget(target);
                        Debug.Log($"Yeni hedef: {target.name}");
                    }
                }
            }
        }
        public Transform GetTarget()
        {
            return target;
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        private void FreeModeControl()
        {
            float speedMultiplier = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? sprintMultiplier : 1.0f;
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            Vector3 moveDirection = new Vector3(horizontal, 0, vertical).normalized;
            transform.Translate(moveDirection * moveSpeed * speedMultiplier * Time.deltaTime, Space.Self);

            if (Input.GetMouseButton(0))
            {
                float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
                float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

                transform.Rotate(Vector3.up, mouseX, Space.World);
                transform.Rotate(Vector3.right, -mouseY, Space.Self);
            }
        }

        private void FollowTarget()
        {
            if (target == null) return;

            Vector3 wantedPosition;

            if (staticOffset)
            {
                wantedPosition = target.position + new Vector3(0, height, distance);
            }
            else
            {
                wantedPosition = target.TransformPoint(0, height, -distance);
            }
            transform.position = Vector3.Lerp(transform.position, wantedPosition, Time.deltaTime * damping);

            if (enableRotation)
            {
                if (smoothRotation)
                {
                    Quaternion wantedRotation = Quaternion.LookRotation(target.position - transform.position, target.up);
                    transform.rotation = Quaternion.Slerp(transform.rotation, wantedRotation, Time.deltaTime * rotationDamping);
                }
                else transform.LookAt(target, target.up);
            }
        }
    }
}
