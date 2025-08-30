using System;
using UnityEngine;

/// <summary>
/// An abstract class that helps to ensure that only one instance of a derived class exists at any time.
/// </summary>
/// <remarks>
/// Use it ONLY IF you know what you're doing. 
/// When deriving, it's advised to use the <see langword="sealed" /> keyword.
/// </remarks>
/// <typeparam name="T">Type of the derived class.</typeparam>
public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
{
    private static T s_instance;

    /// <summary>
    /// Gets the current instance of the object. Throws an exception if there is no instance.
    /// </summary>
    public static T Instance
    {
        get
        {
            if (!s_instance)
                throw new InvalidOperationException($"There is no instance of the singleton type {typeof(T)}.");

            return s_instance;
        }
    }

    /// <summary>
    /// Gets a flag indicating whether there is an instance of the singleton on the current scene.
    /// </summary>
    public static bool Exists => s_instance;

    protected virtual void Awake()
    {
        if (s_instance)
        {
            Debug.LogWarning($"Unexpected multiple instances of the singleton type {typeof(T)}.");
        }

        // Assign the current instance
        s_instance = this as T;
    }

    protected virtual void OnDestroy()
    {
        // Set the instance to null to indicate that the singleton instance is no longer available
        s_instance = null;
    }
}