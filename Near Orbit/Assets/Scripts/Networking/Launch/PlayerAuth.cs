using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAuth : MonoBehaviour
{

    public GameObject OculusAuthPrefab;

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(OculusAuthPrefab);
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
