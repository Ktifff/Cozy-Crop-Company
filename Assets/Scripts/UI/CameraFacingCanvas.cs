using UnityEngine;

public class WorldCanvas : MonoBehaviour
{
    private void Awake()
    {
        transform.rotation = Camera.main.transform.rotation;
    }
}
