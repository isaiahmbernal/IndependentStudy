using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach to objects which are breakable, and set
/// the prefab that will spawn when the object is broken.
/// </summary>
public class ObjectBreakable : MonoBehaviour
{

    [Header("Manual References")]
    [SerializeField] private Transform _brokenPrefab;
    
    /// <summary>
    /// 
    /// Called by the object that destroyed this one.
    /// Spawns the broken version of the game object and
    /// deletes the original one.
    /// 
    /// </summary>
    public void Break() {
        Instantiate(_brokenPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }

}
