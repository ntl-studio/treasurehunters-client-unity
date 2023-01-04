using UnityEngine;
using UnityEngine.UI;

public class Version : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Text>().text = "Version: " + Application.version;
        Debug.Log("Version: " + Application.version);
    }
}
