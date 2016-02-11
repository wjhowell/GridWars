using UnityEngine;
using System.Collections;

public class tile : MonoBehaviour {
    tile instance;
	public Rigidbody rb;
    public int row;
    public int col;
    public int owner;
    public type tile_type;
    public int defense;
    public type defense_type;
    public int remaining;
    public Sprite[] sprites;

    public float duration = 1f;
    public float alpha = 0f;

	// Use this for initialization
	void Start () {
        instance = this;
		rb = GetComponents<Rigidbody> ()[0];
        //row = (int)transform.position.y;
        //col = (int)transform.position.x;
        sprites = Resources.LoadAll<Sprite>("terrain");
    }

    public Sprite get_sprite_by_name(string sprite_name){
        for (int i = 0; i < sprites.Length; i++){
            if (sprites[i].name.Equals(sprite_name)){
                return sprites[i];
            }
        }
        return null;
    }
	
	// Update is called once per frame
	void Update () {

		if (transform.position.y<row)
		{
			rb.velocity = new Vector3(0f, 0f, 0f);
			transform.position = new Vector3(col,row,0);

		}

        string sprite_name = "";

        owner = play_data.instance.owner[col, row];
        tile_type = play_data.instance.tile_type[col, row];
        defense = play_data.instance.defense[col, row];
        defense_type = play_data.instance.defense_type[col, row];

        sprite_name += (owner + 1).ToString() + "_";

        switch (tile_type){
            case type.Empty:
                sprite_name += "empty_";
                break;
            case type.Fire:
                sprite_name += "fire_";
                break;
            case type.Water:
                sprite_name += "water_";
                break;
            case type.Earth:
                sprite_name += "grass_";
                break;
        }

        if(defense > 0){
            switch (defense_type){
                case type.Empty:
                    sprite_name += "empty";
                    break;
                case type.Fire:
                    sprite_name += "fire";
                    break;
                case type.Water:
                    sprite_name += "water";
                    break;
                case type.Earth:
                    sprite_name += "grass";
                    break;
            }
        }
        else{
            sprite_name += "empty";
        }

        GetComponent<SpriteRenderer>().sprite = get_sprite_by_name(sprite_name);

        GameObject blinker = instance.transform.GetChild(0).gameObject;

        float lerp = Mathf.PingPong(Time.time, duration) / duration;
        float alpha = Mathf.Lerp(.3f, .5f, lerp);

        Color blinker_color = blinker.GetComponent<Renderer>().material.color;
        blinker_color.a = 0f;

        if (play_data.instance.current_select_row == row && play_data.instance.current_select_col == col)
        {
            blinker_color.a = 0.3f;
        }
        else if (play_data.instance.whosturn != owner && play_data.instance.IsSelectable[col, row])
        {
            blinker_color.a = alpha;
        }

        blinker.GetComponent<Renderer>().material.color = blinker_color;

    }


    void OnMouseDown()
    {
        play_data.instance.current_select_row = row;
        play_data.instance.current_select_col = col;
    }
}
