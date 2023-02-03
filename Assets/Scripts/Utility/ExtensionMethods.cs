using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using DG.Tweening;

public static class ExtensionMethods
{
    #region Transform
    /// <summary>
    /// Looks for components of type T with specified Tag. Returns the first component of type T found.
    /// </summary>
    public static T FindComponentInChildrenWithTag<T>(this Transform parent, string tag) where T : Component
    {
        Transform[] children = parent.GetComponentsInChildren<Transform>();
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].tag == tag)
            {
                return children[i].GetComponent<T>();
            }
        }
        return null;
    }

    /// <summary>
    /// Looks for components of type T with specified Tag. Returns all components of type T found.
    /// </summary>
    public static T[] FindComponentsInChildrenWithTag<T>(this Transform parent, string tag) where T : Component
    {
        Transform[] children = parent.GetComponentsInChildren<Transform>();
        List<T> list = new List<T>();
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].tag.Contains(tag))
            {
                list.Add(children[i].GetComponent<T>());
            }
        }
        T[] returnArray = new T[list.Count];
        list.CopyTo(returnArray);
        return returnArray;
    }

    public static T GetRequiredComponent<T>(this Transform transform)
    {
        T component = transform.gameObject.GetComponent<T>();

        if (component == null)
        {
            Debug.LogError("Expected to find component of type "
               + typeof(T) + " but found none", transform);
        }

        return component;
    }

    public static T GetRequiredComponentInParent<T>(this Transform transform)
    {
        while (transform.GetComponent<T>() == null)
        {
            if (transform.parent == null)
            {
                Debug.LogError("Expected to find component of type "
               + typeof(T) + " in parents of " + transform.name + ", but found none.", transform);
                break;
            }

            transform = transform.parent;
        }
        return transform.GetComponent<T>();
    }

    // TODO: Not Recursive | Maybe can't detect Interfaces because of null statement | optional bool for search in parent
    public static T GetRequiredComponentInChildren<T>(this Transform transform)
    {
        T component = default(T);

        for (int i = 0; i < transform.childCount; i++)
        {
            component = transform.GetChild(i).GetComponent<T>();
            // TODO: here :D
            if (component != null) return component;
        }

        Debug.LogError("Expected to find component of type "
               + typeof(T) + " in children of " + transform.name + ", but found none.", transform);
        return default(T);
    }
    #endregion



    #region AudioSource
 /*   /// <summary>
    /// Plays the clip with a specified Fade-In time.
    /// </summary>
    /// <param name="fadeInTime">Length of the Fade-In in seconds</param>
    public static void Play(this AudioSource source, float fadeInTime, Ease easeType = Ease.OutExpo)
    {
        DOTween.Complete(source);

        float originalVolume = source.volume;
        source.volume = 0f;
        source.Play();

        source.DOFade(originalVolume, fadeInTime).SetEase(easeType);
    }

    /// <summary>
    /// Stops playing the clip with a specified Fade-Out time.
    /// </summary>
    /// <param name="fadeOutTime">Length of the Fade-Out in seconds</param>
    public static void Stop(this AudioSource source, float fadeOutTime, Ease easeType = Ease.InExpo)
    {
        DOTween.Complete(source);

        float originalVolume = source.volume;
        source.DOFade(0f, fadeOutTime).SetEase(easeType).OnComplete(() =>
        {
            source.Stop();
            source.volume = originalVolume;
        });
    }

    /// <summary>
    /// Cross-Fades between two AudioSources over the specified time.
    /// </summary>
    /// <param name="otherSource">Reference to the AudioSource that shall fade in</param>
    /// <param name="fadingTime">Length of the Cross-Fade in seconds</param>
    public static void CrossFade(this AudioSource thisSource, AudioSource otherSource, float fadingTime, bool startAndStopSources = true)
    {
        if (startAndStopSources)
        {
            float originalVolumeThis = thisSource.volume;
            float originalVolumeOther = otherSource.volume;

            thisSource.DOFade(0f, fadingTime).SetEase(Ease.InOutQuad).OnComplete(() => { thisSource.Stop(); thisSource.volume = originalVolumeThis; });
            otherSource.volume = 0f;
            otherSource.Play();
            otherSource.DOFade(originalVolumeOther, fadingTime).SetEase(Ease.InOutQuad);
        }
        else
        {
            thisSource.DOFade(0f, fadingTime).SetEase(Ease.InOutQuad);
            otherSource.volume = 0f;
            otherSource.DOFade(1f, fadingTime).SetEase(Ease.InOutQuad);
        }
    }*/
    #endregion



    #region General Static Helper Methods
    /// <summary>
    /// Remaps a value from range 1 to range 2.
    /// </summary>
    /// <param name="value">The value to remap.</param>
    /// <param name="from1">Beginning of the first range.</param>
    /// <param name="to1">End of the first range.</param>
    /// <param name="from2">Beginning of the second range.</param>
    /// <param name="to2">End of the second range.</param>
    /// <returns></returns>
    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    /// <summary>
    /// Remaps a value from range 1 to range 2. Return-Value is guaranteed to be between from2 and to2.
    /// </summary>
    /// <param name="value">The value to remap.</param>
    /// <param name="from1">Beginning of the first range.</param>
    /// <param name="to1">End of the first range.</param>
    /// <param name="from2">Beginning of the second range.</param>
    /// <param name="to2">End of the second range.</param>
    /// <returns>Value between from2 and to2.</returns>
    public static float RemapExclusive(this float value, float from1, float to1, float from2, float to2)
    {
        if ((from1 < to1 && value < from1) || (from1 > to1 && value > from1)) return from2;
        if ((from1 < to1 && value > to1) || (from1 > to1 && value < to1)) return to2;

        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static Vector2 ToXZ(this Vector3 v3)
    {
        return new Vector2(v3.x, v3.z);
    }
    #endregion
}
