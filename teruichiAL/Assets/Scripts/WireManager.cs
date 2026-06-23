using UnityEngine;

public class WireManager : MonoBehaviour
{
    public WireDrag[] wires;

    void Update()
    {
        bool clear = true;

        foreach (var wire in wires)
        {
            if (wire.enabled)
            {
                clear = false;
                break;
            }
        }

        if (clear)
        {
            Debug.Log("Task Complete!");
        }
    }
}