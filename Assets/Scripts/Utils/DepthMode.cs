using UnityEngine;

[ExecuteInEditMode]
public class DepthMode : MonoBehaviour
{
    private void OnEnable()
    {
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
    }
}
