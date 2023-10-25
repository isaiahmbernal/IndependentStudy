using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Contains methods for managing all ragdoll
/// calculations.
/// </summary>
public class Ragdoll : MonoBehaviour {

    private class BoneTransform {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
    }
    
    [Header("Auto References")]
    [SerializeField] private CharacterStateManager _myState;
    [SerializeField] private NavMeshAgent _myNavAgent;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Animator _anim;
    [SerializeField] private Transform _myObj;
    [SerializeField] private GameObject _myGameObj;
    [SerializeField] private Transform _myHips;
    [SerializeField] private Rigidbody[] _ragdollRigidbodies;

    [Header("Ragdoll Variables")]
    [SerializeField] private float _myStunTime;
    [SerializeField] private float _timeBeforeRagdoll;
    [SerializeField] private float _standUpTime;
    [SerializeField] private string _standUpAnimationName;
    [SerializeField] private string _standUpClipName;

    [Header("Do Not Modify")]
    [SerializeField] private bool _resettingBones;
    [SerializeField] private float _timeToResetBones;
    [SerializeField] private float _elapsedResetBonesTime;

    [Header("All Bones")]
    [SerializeField] private Transform[] bones;

    [Header("Stand Up Bones")]
    [SerializeField] private BoneTransform[] standUpBoneTransforms;

    [Header("Ragdoll Bones")]
    [SerializeField] private BoneTransform[] ragdollBoneTransforms;
    

    void Awake() {

        _myState = GetComponent<CharacterStateManager>();
        _myNavAgent = GetComponent<NavMeshAgent>();
        _rb = GetComponent<Rigidbody>();
        _myObj = transform.Find("MyObj");
        _anim = _myObj.GetComponent<Animator>();
        _myGameObj = _myObj.gameObject;
        _myHips = _myObj.Find("mixamorig:Hips");

        // Find all rigidbodies in the heirarchy of the game object
        _ragdollRigidbodies = _myHips.GetComponentsInChildren<Rigidbody>();

        // Get all bones in the heirarchy of the bones
        bones = _myHips.GetComponentsInChildren<Transform>();

        // Prepare the arrays to hold the bone positions of the
        // stand up animation and the end position of the ragdoll
        standUpBoneTransforms = new BoneTransform[bones.Length];
        ragdollBoneTransforms = new BoneTransform[bones.Length];
        for (int boneIndex = 0; boneIndex < bones.Length; boneIndex++) {
            standUpBoneTransforms[boneIndex] = new BoneTransform();
            ragdollBoneTransforms[boneIndex] = new BoneTransform();
        }

        // Set the position and rotation of all bones based
        // on the starting point of the stand up animation
        PopulateAnimationStartBoneTransforms();

        // Make sure ragdoll is disabled
        DisableRagdoll();

    }

    private void FixedUpdate() {
        // If we're supposed to be resetting the bones
        // state, run the ResetBones() method
        if (_resettingBones) {
            ResetBones();
    
        }
    }

    /// <summary>
    /// 
    /// Stores the local position of all bones passed into
    /// the method. Used to set the starting position when
    /// transitioning from Ragdoll state to Stand Up state
    /// 
    /// </summary>
    /// <param name="boneTransforms"></param>
    private void PopulateBoneTransforms(BoneTransform[] boneTransforms) {
        for (int boneIndex = 0; boneIndex < bones.Length; boneIndex++) {
            boneTransforms[boneIndex].Position = bones[boneIndex].localPosition;
            boneTransforms[boneIndex].Rotation = bones[boneIndex].localRotation;
        }
    }

    /// <summary>
    /// 
    /// Stores the local position of all bones passed into
    /// the method. Used to set the starting position when
    /// transitioning from Ragdoll state to Stand Up state
    /// 
    /// </summary>
    /// <param name="boneTransforms"></param>
    private void PopulateBoneTransformsTest(BoneTransform[] boneTransforms) {
        for (int boneIndex = 0; boneIndex < bones.Length; boneIndex++) {
            boneTransforms[boneIndex].Position = bones[boneIndex].position;
            boneTransforms[boneIndex].Rotation = bones[boneIndex].rotation;
        }
    }

    /// <summary>
    /// 
    /// Used to store the bone position and rotations
    /// of the beginning of the stand up animation,
    /// to which we'll lerp to from the ragdoll position.
    /// 
    /// </summary>
    /// <param name="_standUpClipName"></param>
    /// <param name="boneTransforms"></param>
    private void PopulateAnimationStartBoneTransforms() {

        Vector3 positionBeforeSampling = _myObj.position;
        Quaternion rotationBeforeSampling = _myObj.rotation;

        foreach (AnimationClip clip in _anim.runtimeAnimatorController.animationClips) {
            if (clip.name == _standUpClipName) {
                clip.SampleAnimation(_myGameObj, 0);
                PopulateBoneTransforms(standUpBoneTransforms);
                break;
            }
        }
        
        _myObj.position = positionBeforeSampling;
        _myObj.rotation = rotationBeforeSampling;

    }

    // private void AlignRotationToHips()
    // {
    //     Vector3 originalHipsPosition = _myHips.position;
    //     Quaternion originalHipsRotation = _myHips.rotation;

    //     Vector3 desiredDirection = _myHips.up * -1;
    //     desiredDirection.y = 0;
    //     desiredDirection.Normalize();

    //     Quaternion fromToRotation = Quaternion.FromToRotation(_myObj.forward, desiredDirection);
    //     _myObj.rotation *= fromToRotation;

    //     _myHips.position = originalHipsPosition;
    //     _myHips.rotation = originalHipsRotation;
    // }

    // private void AlignPositionToHips()
    // {
    //     Vector3 originalHipsPosition = _myHips.position;
    //     _myObj.position = _myHips.position;

    //     Vector3 positionOffset = standUpBoneTransforms[0].Position;
    //     positionOffset.y = 0;
    //     positionOffset = _myObj.rotation * positionOffset;
    //     _myObj.position -= positionOffset;

    //     _myHips.position = originalHipsPosition;
    // }


    /// <summary>
    /// 
    /// Process of lerping the character object's
    /// bones from the end of the ragdoll to the beginning
    /// of the stand up animation, so the animation can
    /// begin smoothly without snapping.
    /// 
    /// Meant to be called within FixedUpdate() after
    /// every frame
    /// 
    /// </summary>
    private void ResetBones() {

        _elapsedResetBonesTime += Time.deltaTime;
        float elapsedPercentage = _elapsedResetBonesTime / _timeToResetBones;

        // For every bone
        for (int boneIndex = 0; boneIndex < bones.Length; boneIndex++) {
            
            // Move the bone to its new position
            bones[boneIndex].localPosition = Vector3.Lerp(
                ragdollBoneTransforms[boneIndex].Position,
                standUpBoneTransforms[boneIndex].Position,
                elapsedPercentage
            );

            // Rotate the bone to its new rotation
            bones[boneIndex].localRotation = Quaternion.Lerp(
                ragdollBoneTransforms[boneIndex].Rotation,
                standUpBoneTransforms[boneIndex].Rotation,
                elapsedPercentage
            );

        }

        if (elapsedPercentage >= 1)
        {
            _resettingBones = false;
            _elapsedResetBonesTime = 0;
        }

    }

    /// <summary>
    /// 
    /// Turns off ragdoll for the character
    /// by enabling kinematics on all bones
    /// and re-enabling the animator.
    /// 
    /// </summary>
    private void DisableRagdoll() {

        foreach (var rigidbody in _ragdollRigidbodies) {
            rigidbody.isKinematic = true;
        }
        _rb.isKinematic = false;
        // _myObj.position = _myHips.position;

        _anim.enabled = true;

    }

    /// <summary>
    /// 
    /// Enables the ragdoll for the character
    /// by disabling kinematics for all bones
    /// and disabling the animator.
    /// 
    /// </summary>
    private void EnableRagdoll() {
        
        foreach (var rigidbody in _ragdollRigidbodies) {
            rigidbody.isKinematic = false;
        }
        _rb.isKinematic = true;

        _anim.enabled = false;

    }

    /// <summary>
    /// 
    /// Method to be called wherever the character's
    /// damage is calculated, likely within a Health
    /// script.
    /// 
    /// </summary>
    /// <param name="stunTime"></param>
    public void HandleRagdoll(float stunTime) {

        _myStunTime = stunTime;
        StopCoroutine("RagdollSteps");
        StartCoroutine("RagdollSteps");

    }

    /// <summary>
    /// 
    /// Initiates a ragdoll sequence for the character,
    /// alternating between able states and actions and
    /// enabling the ragdoll state.
    /// 
    /// Uses the global ragdoll variables to calculate
    /// time between state flips.
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator RagdollSteps() {

        // If I'm dead, make me ragdoll forever
        if (_myState.GetAbleState() == CharacterStateManager.AbleState.Dead) {

            yield return new WaitForSeconds(_timeBeforeRagdoll);
            _myState.SetCurrentAction(CharacterStateManager.CurrentAction.Ragdoll);
            EnableRagdoll();
            yield return null;

        // If I'm *NOT* dead
        } else {

            _myState.SetAbleState(CharacterStateManager.AbleState.Incapacitated);
            _myState.SetCurrentAction(CharacterStateManager.CurrentAction.Stunned);

            yield return new WaitForSeconds(_timeBeforeRagdoll);

            _myState.SetCurrentAction(CharacterStateManager.CurrentAction.Ragdoll);
            EnableRagdoll();

            yield return new WaitForSeconds(_myStunTime);
            
            // AlignRotationToHips();
            // AlignPositionToHips();

            PopulateBoneTransforms(ragdollBoneTransforms);

            // Start resetting bones within FixedUpdate()
            _resettingBones = true;

            yield return new WaitForSeconds(_timeToResetBones);

            DisableRagdoll();
            _myState.SetCurrentAction(CharacterStateManager.CurrentAction.StandingUp);
            _anim.Play(_standUpAnimationName);

            yield return new WaitForSeconds(_standUpTime);

            // * NOTE *
            // Within PlayerWeapon.cs in HitEnemy() we disabled the
            // NavMeshAgent to avoid bugs involving adding force, so
            // now we must re-enable it.
            // _myNavAgent.enabled = true;

            _myState.SetAbleState(CharacterStateManager.AbleState.Normal);
            _myState.SetCurrentAction(CharacterStateManager.CurrentAction.Idle);

        }


    }

}
