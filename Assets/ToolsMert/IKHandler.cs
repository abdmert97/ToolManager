using UnityEngine;

namespace Tools
{
    [RequireComponent(typeof(Animator))]
    public class IKHandler : MonoBehaviour
    {
        private Animator animator;

        public bool ikHandActive = false;
        public bool ikFootActive = false;
        [Range(0,1)]
        public float leftHandPositionWeight = .25f;
        [Range(0, 1)]
        public float rightHandPositionWeight = .25f;
        [Range(0, 1)]
        public float leftFootPositionWeight = 1;
        [Range(0, 1)]
        public float rightFootPositionWeight = 1;
        public Transform rightHandObj = null;
        public Transform leftHandObj = null;
        public Transform lookObj = null;
        private Transform _leftHand;
        private Transform _rightHand;
        public LayerMask layerMask; // Select all layers that foot placement applies to.
        private Vector3 _leftFootRotation;
        private Vector3 _rightFootRotation;
        private Transform _rightFootTransform;
        private Transform _leftFootTransform;

        private float _rightDistanceToGround; // Distance from where the foot transform is to the lowest possible position of the foot.
        private float _leftDistanceToGround; // Distance from where the foot transform is to the lowest possible position of the foot.
        void Start()
        {
           
            animator = GetComponent<Animator>();
            
            _leftHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);
            _rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
      
            _rightFootTransform = animator.GetBoneTransform(HumanBodyBones.RightFoot);
            _leftFootTransform = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
            _leftFootRotation = _leftFootTransform.rotation.eulerAngles;
            _rightFootRotation = _rightFootTransform.rotation.eulerAngles;
        }
        
        
    
        void OnAnimatorIK(int layer)
        {
            
            //Set layer IK Pass to call this function in every IK calculations
            if (animator)
            {
                //if the IK is active, set the position and rotation directly to the goal. 
                if (ikFootActive)
                    SetFootPositon();
                else
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0);
                    animator.SetLookAtWeight(0);
                }
                if(ikHandActive)
                    SetHandPosition();
                //if the IK is not active, set the position and rotation of the hand and head back to the original position
                else
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                    animator.SetLookAtWeight(0);
                }
            }
        }

        private void SetHandPosition()
        {
            // Set the look __target position__, if one has been assigned
            if (lookObj != null)
            {
                animator.SetLookAtWeight(.4f);
                animator.SetLookAtPosition(lookObj.position);
            }

            // Set the right hand target position and rotation, if one has been assigned
            if (rightHandObj != null)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, rightHandPositionWeight);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, .1f);
                animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
                animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
            }
            if (leftHandObj != null)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHandPositionWeight);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, .1f);
                animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandObj.position);
                animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandObj.rotation);
            }
        }

        private void SetFootPositon()
        {
            // Set the weights of left and right feet to the current value defined by the curve in our animations.
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftFootPositionWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, rightFootPositionWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);

            _leftDistanceToGround = _leftFootTransform.position.y - animator.GetBoneTransform(HumanBodyBones.LeftToes).position.y;
            _rightDistanceToGround = _rightFootTransform.position.y - animator.GetBoneTransform(HumanBodyBones.RightToes).position.y;
            // Left Foot
            RaycastHit hit;
            // We cast our ray from above the foot in case the current terrain/floor is above the foot position.
            Ray ray = new Ray(animator.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down * 2);
            if (Physics.Raycast(ray, out hit, _leftDistanceToGround + 1f, layerMask))
            {
                // We're only concerned with objects that are tagged as "Walkable"

                Vector3 footPosition = hit.point; // The target foot position is where the raycast hit a walkable object...
                footPosition.y += _leftDistanceToGround; // ... taking account the distance to the ground we added above.
                animator.SetIKPosition(AvatarIKGoal.LeftFoot, footPosition);
                animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(transform.forward, hit.normal));

            }

            // Right Foot
            ray = new Ray(animator.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down * 2);
            if (Physics.Raycast(ray, out hit, _rightDistanceToGround + 1f, layerMask))
            {
                Vector3 footPosition = hit.point;
                footPosition.y += _rightDistanceToGround;
                animator.SetIKPosition(AvatarIKGoal.RightFoot, footPosition);
                animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(transform.forward, hit.normal));
            }
        }
    }
}
