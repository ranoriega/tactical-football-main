using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[ExecuteAlways]
public class Labeller : MonoBehaviour
{ 
    Renderer tileRenderer;
    Color originalColor;

    public TextMeshPro label;
    [SerializeField]private Renderer rangeSprite;
    public Vector2Int cords = new Vector2Int();
    GridManager gridManager;

    [SerializeField] Color defaultColor = Color.white;
    [SerializeField] Color blockedColor = Color.red;
    [SerializeField] Color exploredColor = Color.yellow;
    [SerializeField] Color rangeColor = Color.blue;

    [SerializeField] Color pathColor = new Color(1f, 0.5f, 0f);

    private void Awake()
    {
        tileRenderer = GetComponentInChildren<Renderer>();
        if (tileRenderer != null)
        {
            originalColor = tileRenderer.material.color;
        }

        gridManager = FindAnyObjectByType<GridManager>();
        label = GetComponentInChildren<TextMeshPro>();
        
        DisplayCords();
           
         if (!Application.isPlaying)
    {
        label.enabled = true;
    }
    }

private void OnEnable()
{
    
    if (rangeSprite != null)
        rangeSprite.enabled = false;

    if (label != null)
        label.enabled = false;
        
}

    void Start()
    {
     
    }

    private void Update()
    {

    

        DisplayCords();
        transform.name = cords.ToString();
        ToggleLables();
    
    }

    public void Highlight()
    {
        if (tileRenderer != null)
            tileRenderer.material.color = Color.white;
    }

    public void ResetHighlight()
    {
        if (tileRenderer != null)
            tileRenderer.material.color = originalColor;
    }

   public void Refresh()
{
    Node node = gridManager.GetNode(cords);
    if (node == null) return;

    if (!node.walkable)
        label.color = blockedColor;
    else if (node.path)
        label.color = pathColor;
    else if (node.explored)
        label.color = exploredColor;
    else
        label.color = defaultColor;

    rangeSprite.enabled = node.inRange;
}

    private void DisplayCords()
    {
        if (!gridManager) { return; }
        cords.x = Mathf.RoundToInt(transform.position.x / gridManager.UnityGridSize);
        cords.y = Mathf.RoundToInt(transform.position.z / gridManager.UnityGridSize);
        label.text = $"{cords.x}, {cords.y}";
    }

    void ToggleLables()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            label.enabled = !label.IsActive();
        }
    }
}
