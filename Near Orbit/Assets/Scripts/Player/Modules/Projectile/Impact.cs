using UnityEngine;
using Bolt;

public class Impact : MonoBehaviour
{
    [SerializeField]
    private float duration;

    void Update()
    {
        duration -= Time.deltaTime;
        if (BoltNetwork.IsServer && duration <= 0)
        {
            BoltNetwork.Destroy(gameObject);
        }
    }
}
