using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAuth : MonoBehaviour
{

    public GameObject oculusAuth;

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(oculusAuth);
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
