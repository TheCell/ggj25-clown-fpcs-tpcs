using UnityEngine;

public class RandomGameObjectInstancer : MonoBehaviour
{
    public GameObject[] meshes;

    private void Start()
    {
        int randomIndex = Random.Range(0, meshes.Length);
        GameObject obj = Instantiate(meshes[randomIndex], transform.position, Quaternion.identity);
        obj.transform.parent = transform;
        RandomizeMaterialTint(0.3f);

    }

    //Funktioniert ned und ich be z müed zums debugge, oh weeeell
    private void RandomizeMaterialTint(float deviationPercent)
    {
        foreach (MeshRenderer meshRenderer in GetComponentsInChildren<MeshRenderer>())
        {
            Material material = meshRenderer.material; // Use material to create a unique instance
            Color color = material.color;
            color.r = Random.ColorHSV().r;//Mathf.Clamp(Random.Range(color.r - deviationPercent * color.r, color.r + deviationPercent * color.r), 0f, 1f);
            color.g = Random.ColorHSV().g;//Mathf.Clamp(Random.Range(color.g - deviationPercent * color.g, color.g + deviationPercent * color.g), 0f, 1f);
            color.b = Random.ColorHSV().b;//Mathf.Clamp(Random.Range(color.b - deviationPercent * color.b, color.b + deviationPercent * color.b), 0f, 1f);
            material.color = color;
        }
    }
}
