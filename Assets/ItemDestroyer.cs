using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDestroyer : MonoBehaviour
{
    private GameObject unityChan;
    private float distance;


    // Start is called before the first frame update
    void Start()
    {
        this.unityChan = GameObject.Find("unitychan");
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(unityChan.transform.position.z);

        this.distance = this.transform.position.z - this.unityChan.transform.position.z;
        if (this.distance < -5)
        {
            Destroy(this.gameObject);
        }
    }
}
