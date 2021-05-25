
using UnityEngine;

public class pathPrefDestroy : MonoBehaviour
{

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Destroy(gameObject);
        }
        Destroy(gameObject, 0.1f);
    }
}
