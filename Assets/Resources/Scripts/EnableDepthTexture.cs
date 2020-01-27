using UnityEngine;

//behaviour which should lie on the same gameobject as the main camera
public class EnableDepthTexture : MonoBehaviour
{
    private void Start()
    {
        //get the camera and tell it to render a depth texture
        Camera cam = GetComponent<Camera>();
        cam.depthTextureMode = DepthTextureMode.Depth;
    }
}