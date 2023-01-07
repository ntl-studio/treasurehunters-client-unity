using UnityEngine;
using UnityEngine.Scripting;

public class DisableGC : MonoBehaviour
{
    private const bool _disableGC = false;

    void Start()
    {
        if (_disableGC)
        {
#if !UNITY_EDITOR
            Debug.Log("Disabling GC");
            GarbageCollector.GCMode = GarbageCollector.Mode.Disabled;
#endif
        }
    }
}
