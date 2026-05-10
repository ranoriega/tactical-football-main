using UnityEngine;

public class GridHoverManager : MonoBehaviour
{

    Labeller lastLabeller;

    void Update()
    {
        Labeller current = GetLabellerUnderMouse();

        if (current != lastLabeller)
        {
            if (lastLabeller != null)
                lastLabeller.ResetHighlight();

            if (current != null)
                current.Highlight();

            lastLabeller = current;
        }
    }

    Labeller GetLabellerUnderMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            return hit.collider.GetComponent<Labeller>();
        }

        return null;
    }
}
