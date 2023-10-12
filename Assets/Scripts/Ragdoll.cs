using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR;

/// <summary>
/// Contains methods for managing all ragdoll
/// calculations.
/// </summary>
public class Ragdoll : MonoBehaviour {

    private class BoneTransform {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
    }
    
    [Header("References")]
    [SerializeField] private CharacterStateManager myState;
    [SerializeField] private Rigidbody[] ragdollRigidbodies;
    [SerializeField] private Rigidbody myRb;
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject myGameObject;
    [SerializeField] private Transform myTransform;
    [SerializeField] private Transform myHips;
    [SerializeField] private NavMeshAgent myNavAgent;

    [Header("Ragdoll Variables")]
    [SerializeField] private float myStunTime;
    [SerializeField] private float timeBeforeRagdoll;
    [SerializeField] private float standUpTime;
    [SerializeField] private string standUpAnimationName;
    [SerializeField] private bool resettingBones;
    [SerializeField] private float timeToResetBones;
    [SerializeField] private float elapsedResetBonesTime;
    [SerializeField] private Vector3 storedHipsPos;

    [Header("All Bones")]
    [SerializeField] private Transform[] bones;

    [Header("Stand Up Bones")]
    [SerializeField] private BoneTransform[] standUpBoneTransforms;

    [Header("Ragdoll Bones")]
    [SerializeField] private BoneTransform[] ragdollBoneTransforms;
    

    void Awake() {

        // Find all rigidbodies in the heirarchy of the game object
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();

        // Get all bones in the heirarchy of the bones
        bones = myHips.GetComponentsInChildren<Transform>();

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
        PopulateAnimationStartBoneTransforms(standUpAnimationName, standUpBoneTransforms);

        // Make sure ragdoll is disabled
        DisableRagdoll();

    }

    private void FixedUpdate() {
        // If we're supposed to be resetting the bones
        // state, run the ResetBones() method
        if (resettingBones) {
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
    /// <param name="standUpAnimationName"></param>
    /// <param name="boneTransforms"></param>
    private void PopulateAnimationStartBoneTransforms(string standUpAnimationName, BoneTransform[] boneTransforms) {

        Vector3 positionBeforeSampling = myGameObject.transform.position;
        Quaternion rotationBeforeSampling = myGameObject.transform.rotation;

        foreach (AnimationClip clip in anim.runtimeAnimatorController.animationClips) {
            if (clip.name == standUpAnimationName) {
                clip.SampleAnimation(myGameObject, 0);
                PopulateBoneTransforms(standUpBoneTransforms);
                break;
            }
        }
        
        myGameObject.transform.position = positionBeforeSampling;
        myGameObject.transform.rotation = rotationBeforeSampling;

    }

    // private void AlignRotationToHips()
    // {
    //     Vector3 originalHipsPosition = myHips.position;
    //     Quaternion originalHipsRotation = myHips.rotation;

    //     Vector3 desiredDirection = myHips.up * -1;
    //     desiredDirection.y = 0;
    //     desiredDirection.Normalize();

    //     Quaternion fromToRotation = Quaternion.FromToRotation(myGameObject.transform.forward, desiredDirection);
    //     myGameObject.transform.rotation *= fromToRotation;

    //     myHips.position = originalHipsPosition;
    //     myHips.rotation = originalHipsRotation;
    // }

    // private void AlignPositionToHips()
    // {
    //     Vector3 originalHipsPosition = myHips.position;
    //     myGameObject.transform.position = myHips.position;

    //     Vector3 positionOffset = standUpBoneTransforms[0].Position;
    //     positionOffset.y = 0;
    //     positionOffset = myGameObject.transform.rotation * positionOffset;
    //     myGameObject.transform.position -= positionOffset;

    //     myHips.position = originalHipsPosition;
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

        elapsedResetBonesTime += Time.deltaTime;
        float elapsedPercentage = elapsedResetBonesTime / timeToResetBones;

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
            resettingBones = false;
            elapsedResetBonesTime = 0;
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

        foreach (var rigidbody in ragdollRigidbodies) {
            rigidbody.isKinematic = true;
        }
        myRb.isKinematic = false;

        // myTransform.position = myHips.position;
        anim.enabled = true;

    }

    /// <summary>
    /// 
    /// Enables the ragdoll for the character
    /// by disabling kinematics for all bones
    /// and disabling the animator.
    /// 
    /// </summary>
    private void EnableRagdoll() {
        
        foreach (var rigidbody in ragdollRigidbodies) {
            rigidbody.isKinematic = false;
        }
        myRb.isKinematic = true;

        anim.enabled = false;

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

        myStunTime = stunTime;
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
        if (myState.GetAbleState() == CharacterStateManager.AbleState.Dead) {

            yield return new WaitForSeconds(timeBeforeRagdoll);
            myState.CurrentActionChange(CharacterStateManager.CurrentAction.Ragdoll);
            EnableRagdoll();
            yield return null;

        // If I'm *NOT* dead
        } else {

            myState.AbleStateChange(CharacterStateManager.AbleState.Incapacitated);
            myState.CurrentActionChange(CharacterStateManager.CurrentAction.Stunned);

            yield return new WaitForSeconds(timeBeforeRagdoll);

            myState.CurrentActionChange(CharacterStateManager.CurrentAction.Ragdoll);
            EnableRagdoll();

            yield return new WaitForSeconds(myStunTime);
            
            // AlignRotationToHips();
            // AlignPositionToHips();

            PopulateBoneTransforms(ragdollBoneTransforms);

            // Start resetting bones within FixedUpdate()
            resettingBones = true;

            yield return new WaitForSeconds(timeToResetBones);

            DisableRagdoll();
            myState.CurrentActionChange(CharacterStateManager.CurrentAction.StandingUp);
            anim.Play("Stand");

            yield return new WaitForSeconds(standUpTime);

            // * NOTE *
            // Within PlayerWeapon.cs in HitEnemy() we disabled the
            // NavMeshAgent to avoid bugs involving adding force, so
            // now we must re-enable it.
            myNavAgent.enabled = true;

            myState.AbleStateChange(CharacterStateManager.AbleState.Normal);
            myState.CurrentActionChange(CharacterStateManager.CurrentAction.Idle);

        }


    }

}
