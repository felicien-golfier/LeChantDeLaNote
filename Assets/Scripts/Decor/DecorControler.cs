using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorControler : MonoBehaviour
{

    public GameObject [] limits;
    public SpriteRenderer Background;

    // Start is called before the first frame update
    void Start()
    {
        Background.size = new Vector2(2 * Tools.limitX, 2 * Tools.limitY);
        Background.transform.localPosition = Vector3.zero;

        // Top boundary
        limits[0].transform.position = new Vector3(0, Tools.limitY, 0);
        limits[0].transform.localScale = new Vector3(2 * Tools.limitX, 1, 1);

        // Bottom boundary
        limits[1].transform.position = new Vector3(0, -Tools.limitY, 0);
        limits[1].transform.localScale = new Vector3(2 * Tools.limitX, 1, 1);

        // Right boundary
        limits[2].transform.position = new Vector3(Tools.limitX, 0, 0);
        limits[2].transform.localScale = new Vector3(1, 2* Tools.limitY, 1);

        // Left boundary
        limits[3].transform.position = new Vector3(-Tools.limitX, 0, 0);
        limits[3].transform.localScale = new Vector3(1, 2 * Tools.limitY, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
