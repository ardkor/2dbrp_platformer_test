using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    public Renderer[] backgrounds;
    public float[] parallaxCoef;

    //lists
    private Vector2[] offsets;

    private Material[] materials;
    private Transform cam;
    private Vector3 lastCamPos;
    private bool _inited;

    public void Initialize()
    {
        offsets = new Vector2[6];
        materials = new Material[6];
        for (int i = 0; i < backgrounds.Length; ++i)
        {
            materials[i] = backgrounds[i].material;
        }
        cam = Camera.main.transform;
        lastCamPos = cam.position;
        _inited = true;
    }

    void Update()
    {

        float deltaX = cam.position.x - lastCamPos.x;
        for (int i = 0; i < backgrounds.Length; ++i)
        {
            offsets[i] += new Vector2(deltaX * parallaxCoef[i], 0);
            materials[i].mainTextureOffset = offsets[i];
        }
        lastCamPos = cam.position;  
    }
}
