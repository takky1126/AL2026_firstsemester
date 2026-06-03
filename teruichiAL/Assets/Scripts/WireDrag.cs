using UnityEngine;

public class WireDrag : MonoBehaviour
{
    private LineRenderer line;
    private bool dragging = false;

    private Vector3 startPos;
    void Start()
    {
        line = GetComponent<LineRenderer>();

        line.material = new Material(Shader.Find("Sprites/Default"));

        startPos = transform.position;

        line.positionCount = 2;
        line.SetPosition(0, startPos);
        line.SetPosition(1, startPos);

        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (sr != null)
        {
            float wireThickness = sr.bounds.size.y;

            line.startWidth = wireThickness;
            line.endWidth = wireThickness;

            line.startColor = sr.color;
            line.endColor = sr.color;
            line.material.color = sr.color;
        }
    }

    void OnMouseDown()
    {
        dragging = true;
    }

    void Update()
    {
        if (!dragging)
            return;

        Vector3 mouse =
            Camera.main.ScreenToWorldPoint(Input.mousePosition);

        mouse.z = 0;

        line.SetPosition(1, mouse);
    }

    void OnMouseUp()
    {
        dragging = false;

        CheckConnection();
    }

    void CheckConnection()
    {
        Collider2D hit =
            Physics2D.OverlapCircle(
                line.GetPosition(1),
                0.5f);

        if (hit == null)
        {
            ResetWire();
            return;
        }

        WirePoint target =
            hit.GetComponent<WirePoint>();

        if (target == null)
        {
            ResetWire();
            return;
        }

        WirePoint self =
            GetComponent<WirePoint>();

        if (self.wireColor == target.wireColor)
        {
            line.SetPosition(1, target.transform.position);

            enabled = false;
        }
        else
        {
            ResetWire();
        }
    }

    void ResetWire()
    {
        line.SetPosition(1, startPos);
    }
}

