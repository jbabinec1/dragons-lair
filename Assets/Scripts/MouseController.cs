using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        var focusedTileHit = GetFocusedOnTile();

        if (focusedTileHit.HasValue)
        {
            GameObject tile_sprite = focusedTileHit.Value.collider.gameObject;
            transform.position = tile_sprite.transform.position;
            gameObject.GetComponent<SpriteRenderer>().sortingOrder= tile_sprite.GetComponent<SpriteRenderer>().sortingOrder;

        }
        
    }

    public RaycastHit2D? GetFocusedOnTile()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2 = new Vector2(mousePos.x, mousePos.y);

        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2, Vector2.zero);

        if (hits.Length > 0)
        {
            return hits.OrderByDescending(i => i.collider.transform.position.z).First();
        }

        return null;
    }
}
