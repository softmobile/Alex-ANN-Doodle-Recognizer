using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasDrawer : MonoBehaviour
{
    private int canvasSize = 28;
    public int brushRadius = 1;
    public float brushStrength = 0.1f;
    public GameObject tilePrefab;
    public List<GameObject> tiles;
    public GameObject tilesContainer;
    public Transform background;
    Camera cam;

    private List<float> image = new List<float>();
    private System.Random random = new System.Random();

    private float overallGridOffset;
    private void Start() {
        overallGridOffset = canvasSize/2;
        cam = Camera.main;
        for(int i = canvasSize; i > 0; i--)
        {
            for(int j = 0; j < canvasSize; j++)
            {
                float xLocation = (j-overallGridOffset+0.5f+background.position.x*4.0f)*0.25f;
                float yLocation = (i-overallGridOffset-0.5f+background.position.y*4.0f)%canvasSize*0.25f;
                Vector2 pos = new Vector2(xLocation, yLocation);
                GameObject tile = Instantiate(tilePrefab, pos, Quaternion.identity);
                tile.GetComponent<TileData>().x = j;
                tile.GetComponent<TileData>().y = canvasSize-i;
                tile.transform.parent = tilesContainer.transform;
                tiles.Add(tile);
            }
        }
    }
    
    private bool clicking = false;
    private void Update() {
        if(Input.GetMouseButtonDown(0))
        {
            clicking = true;
        }
        if(Input.GetMouseButtonUp(0))
        {
            clicking = false;
        }

        if(clicking)
        {
            Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            
            if(hit.collider != null)
            {
                GameObject hitObject = hit.collider.gameObject;
                if(hitObject.transform.parent = tilesContainer.transform)
                {
                    int hitX = hitObject.GetComponent<TileData>().x;
                    int hitY = hitObject.GetComponent<TileData>().y;

                    List<GameObject> sTiles = GetSurroundingTiles(brushRadius, hitX, hitY);
                    
                    foreach(GameObject tile in sTiles)
                    {
                        Color c = tile.GetComponent<SpriteRenderer>().color;
                        float brushReduction = 1/(tile.GetComponent<TileData>().distFromBrush+0.05f);
                        float addition = brushStrength*brushReduction;
                        float finalAddition = c.r + addition/(2f*c.r+1f);
                        float clampedColor = Mathf.Clamp(finalAddition, 0f, 1f);
                        tile.GetComponent<SpriteRenderer>().color = new Color(
                            clampedColor,
                            clampedColor,
                            clampedColor
                        );
                    }
                }
            }
        }
    }

    private List<GameObject> GetSurroundingTiles(int radius, int x, int y)
    {
        List<GameObject> sTiles = new List<GameObject>();

        int totalTiles = (int)Math.Pow(2*radius + 1, 2);
        int xOffset = -radius;
        int yOffset = -radius;

        for(int i = 0; i < totalTiles; i ++)
        {
            int tileX = x+xOffset;
            int tileY = y+yOffset;

            int location = tileY * 28 + tileX;
            if(tileX >= 0 && tileY >= 0 && tileX < canvasSize && tileY < canvasSize && location < (int)Math.Pow(canvasSize, 2))
            {
                tiles[location].GetComponent<TileData>().distFromBrush = Mathf.Sqrt(Mathf.Pow(xOffset, 2) + Mathf.Pow(yOffset, 2));
                sTiles.Add(tiles[location]);
            }

            xOffset++;
            if((i+1) % (2*radius+1) == 0)
            {
                xOffset = -radius;
                yOffset++;
            }
        }

        return sTiles;
    }

    public void ClearPixels()
    {
        for(int i = 0; i < Math.Pow(canvasSize, 2); i++)
        {
            tiles[i].GetComponent<SpriteRenderer>().color = Color.black;
        }
    }

    public void ShowNextDrawing()
    {
        
    }
}