using UnityEditor;
using UnityEngine;

public class SaveComponentMenu : MonoBehaviour
{
    [MenuItem("CONTEXT/Component/Save %s")]
    private static void SaveComponent(MenuCommand command)
    {
        var component = command.context as Component;
        if (component != null)
        {
            EditorUtility.SetDirty(component);
        }
    }
}
