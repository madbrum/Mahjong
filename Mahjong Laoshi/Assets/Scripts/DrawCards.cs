using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawCards : MonoBehaviour
{
    //public GameObject Dot1;
    //public GameObject Bam8;

    public GameObject Tile;
    public GameObject PlayerArea;
    public GameObject OpponentArea;
    public GameObject DealArea;
    //public Sprite DotSprite;

    private List<GameObject> tiles = new List<GameObject>();
    private Object[] TileSprites;

    //List<GameObject> tiles = new List<GameObject>();

    void Start()
    {
        //KEY THING is that you can't just randomize these tiles! there are only a set number of dot, bam, ect tiles that exist! no dupes beyond a certain point! maybe a counter?  
        //eventually you will also need to tag these with type identities and probably make a new tile array but o well
        TileSprites = Resources.LoadAll("Tile Sprites");
        Debug.Log("Sprites Loaded, Size " + TileSprites.Length);
        for (int i = 0; i < TileSprites.Length; i++)
        {
            //create a Tile GameObject, load it with the current tile sprite
            //for int i = 0 i < 4, add a new "instance" (not actually in the map, just... existing somewhere, like any other variable) of this object to tiles 
            //basically you want to load this array with copies of overloaded prefabs, remove each upon actual instantiation...
            //so if i do something like List.add(dot1) four times... what happens? 
            //in essence this is you building your SET of tiles, which are then dealt out at random
            for (int j = 0; j < 4; j++)
            {
                GameObject single = Instantiate(Tile, new Vector3(0, 0, 0), Quaternion.identity);
                single.GetComponent<Image>().sprite = (Sprite) TileSprites[i];
                single.hideFlags = HideFlags.HideInHierarchy;
                single.SetActive(false);
                tiles.Add(single);
            }
            Debug.Log("Adding Tile " + i + " was successful");
        }

        //tiles.Add(Dot1);
        //tiles.Add(Bam8);
    }

    public void OnClick()
    {
        for (int i = 0; i < 13; i++)
        {
            //GameObject playerTile = Instantiate(tiles[Random.Range(0, tiles.Count)], new Vector3(0, 0, 0), Quaternion.identity);
            //Resources.loadAll
            //playerTile.GetComponent<Image>().sprite = DotSprite;

            int randomIndex = Random.Range(0, 8);
            Debug.Log("Index is " + randomIndex);
            Debug.Log("Tile list count is " + tiles.Count);
            GameObject playerTile = tiles[randomIndex];
            tiles.RemoveAt(randomIndex);
            playerTile.hideFlags = HideFlags.None;
            playerTile.SetActive(true);
            playerTile.transform.SetParent(PlayerArea.transform, false);


            //GameObject opponentTile = Instantiate(tiles[Random.Range(0, tiles.Count)], new Vector3(0, 0, 0), Quaternion.identity);
            int randomOppIndex = Random.Range(8, 16);
            Debug.Log("Index is " + randomOppIndex);
            Debug.Log("Tile list count is " + tiles.Count);
            GameObject opponentTile = tiles[randomOppIndex];
            tiles.RemoveAt(randomOppIndex);
            opponentTile.hideFlags = HideFlags.None;
            opponentTile.SetActive(true);
            opponentTile.transform.SetParent(OpponentArea.transform, false);
        }
    }
}
