using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeController : MonoBehaviour
{

    public GameObject Center;
    
    private bool _canMoveTop = false;
    private bool _canMoveRight = false;
    private bool _canMoveBottom = false;
    private bool _canMoveLeft = false;

    private NodeController _nodeTop;
    private NodeController _nodeRight;
    private NodeController _nodeBottom;
    private NodeController _nodeLeft;

    public bool CanMoveTop { get => _canMoveTop; set => _canMoveTop = value; }
    public bool CanMoveRight { get => _canMoveRight; set => _canMoveRight = value; }
    public bool CanMoveBottom { get => _canMoveBottom; set => _canMoveBottom = value; }
    public bool CanMoveLeft { get => _canMoveLeft; set => _canMoveLeft = value; }
    public NodeController NodeTop { get => _nodeTop; set => _nodeTop = value; }
    public NodeController NodeRight { get => _nodeRight; set => _nodeRight = value; }
    public NodeController NodeBottom { get => _nodeBottom; set => _nodeBottom = value; }
    public NodeController NodeLeft { get => _nodeLeft; set => _nodeLeft = value; }


    // Start is called before the first frame update
    void Awake()
    {
        RaycastHit hit;
        // Top Node
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 1f))
        {
            if (hit.transform.gameObject.tag == "Node")
            {
                NodeTop = hit.transform.gameObject.GetComponent<NodeController>();
                CanMoveTop = true;
            }
        }
        // Right Node
        if (Physics.Raycast(transform.position, -1f * transform.TransformDirection(Vector3.left), out hit, 1f))
        {
            if (hit.transform.gameObject.tag == "Node")
            {
                NodeRight = hit.transform.gameObject.GetComponent<NodeController>();
                CanMoveRight = true;
            }
        }
        // Bottom Node
        if (Physics.Raycast(transform.position, -1f * transform.TransformDirection(Vector3.forward), out hit, 1f))
        {
            if (hit.transform.gameObject.tag == "Node")
            {
                NodeBottom = hit.transform.gameObject.GetComponent<NodeController>();
                CanMoveBottom = true;
            }
        }
        // Left Node
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out hit, 1f))
        {
            if(hit.transform.gameObject.tag == "Node")
            {
                NodeLeft = hit.transform.gameObject.GetComponent<NodeController>();
                CanMoveLeft = true;
            }
        }

    }
}
