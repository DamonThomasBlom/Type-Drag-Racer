using UnityEngine;

public class ColourChangerSpaceBar : MonoBehaviour
{
    public MeshRenderer renderer;

    private void Start()
    {
        renderer = GetComponent<MeshRenderer>();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeColour();
        }
    }

    void ChangeColour()
    {
        renderer.materials[0].color = Random.ColorHSV();
    }
}
