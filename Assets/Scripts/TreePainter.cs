using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TreePainter : MonoBehaviour
{
    public List<GameObject> Trees = new List<GameObject>();

    public float RandomMidTreeSize = 1;
    public float RandomMaxTreeSize = 1;

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Vector3 mouseScreenPosition = Input.mousePosition;

        // Define the ray from the object's position in its forward direction
        Ray ray = Camera.main.ScreenPointToRay(mouseScreenPosition);
        RaycastHit hit;

        // Perform the raycast
        if (Physics.Raycast(ray, out hit, 999f))
        {
            // If the ray hits something, log the name of the hit object
            Debug.Log("Raycast hit: " + hit.collider.gameObject.name);

            // You can also access other hit information
            int randomTreeIndex = Random.Range(0, Trees.Count);
            var tree = Trees[randomTreeIndex];

            var newTree = Instantiate(tree, hit.point, Quaternion.identity);

            newTree.transform.localScale = Vector3.one * Random.Range(RandomMidTreeSize, RandomMaxTreeSize);
        }
        else
        {
            // If the ray didn't hit anything
            Debug.Log("Raycast missed.");
        }
    }
}
