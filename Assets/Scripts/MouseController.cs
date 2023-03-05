using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    public GameObject characterPrefab;
    private CharacterInfo character;
    public Animator characterAnimationSprite;

    private Pathfinder pathfinder;

    public float speed;

    private List<OverlayTile> path = new List<OverlayTile>();

    public OverlayTile startingTile;
    public OverlayTile activeTile;


    // Start is called before the first frame update
    void Start()
    {
        //character = characterPrefab.GetComponent<CharacterInfo>();
        pathfinder = new Pathfinder();
        
        character = GameObject.Find("dragon_child").GetComponent<CharacterInfo>();
        characterAnimationSprite = GameObject.Find("dragon_child").GetComponent<Animator>();
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        var focusedTileHit = GetFocusedOnTile();

        var focusedOnTileFromPlayer = GetFocusedOnTileFromPlayer();

        var isMoving = false;

        if (focusedOnTileFromPlayer.HasValue)
        {
            character.startingTile = focusedOnTileFromPlayer.Value.collider.gameObject.GetComponent<OverlayTile>();

        }

        if (focusedTileHit != null && focusedTileHit.HasValue)
        {
            OverlayTile overlayTile = focusedTileHit.Value.collider.gameObject.GetComponent<OverlayTile>();

            if (overlayTile != null)
            {
                transform.position = overlayTile.transform.position;
                gameObject.GetComponent<SpriteRenderer>().sortingOrder = overlayTile.GetComponent<SpriteRenderer>().sortingOrder;
                if (Input.GetMouseButtonDown(0) && overlayTile != null)
                {
                    if (character.activeTile == null)
                    {
                        // Set startingTile to the tile that the player is currently on
                        character.startingTile = focusedOnTileFromPlayer?.collider.gameObject.GetComponent<OverlayTile>();

                        path = pathfinder.FindPath(character.startingTile, overlayTile);

                        character.activeTile = character.startingTile;

                    }
                    else if (character.activeTile != overlayTile) // add this condition
                    {
                        path = pathfinder.FindPath(character.activeTile, overlayTile);
                        character.activeTile = overlayTile;
                    }
                }
            }
        }

        if (path != null && path.Count > 0)
        {
            isMoving = true;

            if (isMoving)
            {
                characterAnimationSprite.SetBool("isWalking", true);
                MoveAlongPath();
            }
        }
        else
        {
            characterAnimationSprite.SetBool("isWalking", false);
            isMoving = false;
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


    private void PositionCharacterOnTile(OverlayTile tile)
    {
        character.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, tile.transform.position.z);

        character.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;

        character.activeTile = tile;

        
         character.startingTile = tile;

     
    }


    public void MoveAlongPath()
    {
      
        var step = speed * Time.deltaTime;

        var zIndex = path[0].transform.position.z;

        character.transform.position = Vector2.MoveTowards(character.transform.position, path[0].transform.position, step);

        character.transform.position = new Vector3(character.transform.position.x, character.transform.position.y, zIndex);

        //if(Vector2.Distance(character.transform.position, path[0].transform.position) < 0.0001f)
        if ((character.transform.position - path[0].transform.position).magnitude < 0.0001f)
        {
            PositionCharacterOnTile(path[0]);
            path.RemoveAt(0);
        }

    }



    public RaycastHit2D? GetFocusedOnTileFromPlayer()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(character.transform.position, Vector2.zero);
        // Debug.Log(hits);
        if (hits.Length > 0)
        {
            return hits.OrderByDescending(i => i.collider.transform.position.z).First();
        }

        return null;
    }

}
