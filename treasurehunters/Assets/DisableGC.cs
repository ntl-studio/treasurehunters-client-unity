using UnityEngine;
using UnityEngine.Scripting;

public class DisableGC : MonoBehaviour
{
    void Start()
    {
#if !UNITY_EDITOR
        Debug.Log("Disabling GC");
        GarbageCollector.GCMode = GarbageCollector.Mode.Disabled;
#endif
    }
}
