using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Hud : MonoBehaviour {

    public static Hud instance;
    public Text player_text;
    public Text coordinate_text;
    public GameObject tile;
    public GameObject[] dpills;
    public GameObject[] rpills;
    public Sprite[] sprites;
    public Text fire_inventory;
    public Text water_inventory;
    public Text grass_inventory;
    public Text fire_income;
    public Text water_income;
    public Text grass_income;
    public Text action_title;
    public Text moves;
    public Text action_type;
    public Button option_0;
    public Button option_1;
    public Button option_2;
    public Button end_turn;
    public Camera guicam;
    public Transform Panel1;
    public Transform Panel2;

    // variables
    public int owner;
    public type tile_type;
    public int defense;
    public type def_type;
    public int resources_remaining;

    // icons
    public Sprite fire_pill;
    public Sprite water_pill;
    public Sprite grass_pill;
    public Sprite empty;
    public Sprite fire;
    public Sprite water;
    public Sprite grass;
    public Sprite fire_desat;
    public Sprite water_desat;
    public Sprite grass_desat;
    public Sprite one_time;
    public Sprite long_term;

    // start menu
    public Text instruction_text;
    public Text Continue_text;
    public Button Continue;
    public GameObject Instruction_cube;

    public Sprite get_sprite_by_name(string sprite_name){
        for (int i = 0; i < sprites.Length; i++){
            if (sprites[i].name.Equals(sprite_name)){
                return sprites[i];
            }
        }
        return null;
    }


    // Use this for initialization
    void Start () {
        // instance = this;
        // Instruction_cube = GameObject.Find("Instruction_cube");
        // Panel1 = transform.Find("Panel1");
        // Panel2 = transform.Find("Panel2");
        // Panel1.gameObject.SetActive(true);
        // Panel2.gameObject.SetActive(false);
        // Continue_text.text = "Start";
        // instruction_text.text = "GRID WARS";
        // guicam.backgroundColor = Color.black;
        sprites = Resources.LoadAll<Sprite>("terrain");
        end_turn.onClick.AddListener(() => play_data.instance.next_turn());
        option_0.onClick.AddListener(() => play_data.instance.option_0());
        option_1.onClick.AddListener(() => play_data.instance.option_1());
        option_2.onClick.AddListener(() => play_data.instance.option_2());
        // Continue.onClick.AddListener(() => play_data.instance.Continue());
    }
	
	// Update is called once per frame
	void Update () {
		
		

        // if (play_data.instance.game_start)
        // {
        //     Continue_text.text = "Continue";
        //     guicam.backgroundColor = play_data.OwnerIntToColor(play_data.instance.whosturn);
        // }


        // --

        // get information
        owner = play_data.instance.owner[play_data.instance.current_select_col, play_data.instance.current_select_row];
        tile_type = play_data.instance.tile_type[play_data.instance.current_select_col, play_data.instance.current_select_row];
        defense = play_data.instance.defense[play_data.instance.current_select_col, play_data.instance.current_select_row];
        def_type = play_data.instance.defense_type[play_data.instance.current_select_col, play_data.instance.current_select_row];
        resources_remaining = play_data.instance.remaining[play_data.instance.current_select_col, play_data.instance.current_select_row];

        // player text
        player_text.text = "Player " + (play_data.instance.whosturn + 1).ToString();

        // coordinate text
        coordinate_text.text = "Row: " + (play_data.instance.current_select_col + 1).ToString() + ", Column: " + (play_data.instance.current_select_row + 1).ToString();
        
        // get current tile sprite
        string sprite_name = (owner + 1).ToString() + "_";
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
            switch (def_type){
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
        tile.GetComponent<SpriteRenderer>().sprite = get_sprite_by_name(sprite_name);

        // load defense pills
        if(defense > 0){
            Sprite pill_type = fire_pill;
            if(def_type == type.Fire){
                pill_type = fire_pill;
            }
            else if(def_type == type.Water){
                pill_type = water_pill;
            }
            else if(def_type == type.Earth){
                pill_type = grass_pill;
            }
            for(int i = 0; i < defense; ++i){
                dpills[i].gameObject.SetActive(true);
                dpills[i].GetComponent<SpriteRenderer>().sprite = pill_type;
            }
        }
        for(int i = defense; i < 5; ++i){
            dpills[i].gameObject.SetActive(false);
        }

        // load resource pills
        if(resources_remaining > 0){
            Sprite pill_type = fire_pill;
            if(tile_type == type.Fire){
                pill_type = fire_pill;
            }
            else if(tile_type == type.Water){
                pill_type = water_pill;
            }
            else if(tile_type == type.Earth){
                pill_type = grass_pill;
            }
            for(int i = 0; i < resources_remaining; ++i){
                rpills[i].gameObject.SetActive(true);
                rpills[i].GetComponent<SpriteRenderer>().sprite = pill_type;
            }
        }
        for(int i = resources_remaining; i < 10; ++i){
            rpills[i].gameObject.SetActive(false);
        }
        
        // inventory
        fire_inventory.text = "x " + play_data.instance.player_resource[play_data.instance.whosturn, 0].ToString();
        water_inventory.text = "x " + play_data.instance.player_resource[play_data.instance.whosturn, 1].ToString();
        grass_inventory.text = "x " + play_data.instance.player_resource[play_data.instance.whosturn, 2].ToString();

        // income
        fire_income.text = "+" + play_data.instance.player_income[play_data.instance.whosturn, 0].ToString() + " per turn";
        water_income.text = "+" + play_data.instance.player_income[play_data.instance.whosturn, 1].ToString() + " per turn";
        grass_income.text = "+" + play_data.instance.player_income[play_data.instance.whosturn, 2].ToString() + " per turn";


        // moves remaining
        moves.text = play_data.instance.moves_remain + " of 2 actions remaining";


        if (play_data.instance.moves_remain != 0 && play_data.instance.IsSelectable[play_data.instance.current_select_col, play_data.instance.current_select_row])
        {

            action_type.text = "Select an action:";
            #region if you still have move and selectable
            if (play_data.instance.owner[play_data.instance.current_select_col, play_data.instance.current_select_row] == -1) //if the tile has no owner
            {
                action_title.text = "Claim";
                if (play_data.instance.tile_type[play_data.instance.current_select_col, play_data.instance.current_select_row] == type.Empty)
                {
                    option_0.image.sprite = empty;
                    option_1.image.sprite = one_time;
                    option_2.image.sprite = empty;
                    option_0.interactable = false;
                    option_1.interactable = true;
                    option_2.interactable = false;
                }
                else
                {
                    option_0.image.sprite = long_term;
                    option_1.image.sprite = one_time;
                    option_2.image.sprite = empty;
                    option_0.interactable = true;
                    option_1.interactable = true;
                    option_2.interactable = false;
                }

            }
            else if (play_data.instance.owner[play_data.instance.current_select_col, play_data.instance.current_select_row] == play_data.instance.whosturn)// Defense (tile's owner = player of current turn)
            {
                action_title.text = "Defend";

                switch (play_data.instance.defense_type[play_data.instance.current_select_col, play_data.instance.current_select_row])
                {
                case type.Fire:
                    if (play_data.instance.player_resource[play_data.instance.whosturn, 0] > 1){
                        option_0.image.sprite = fire;
                        option_0.interactable = true;
                    }
                    else{
                        option_0.image.sprite = fire_desat;
                        option_0.interactable = false;
                    }
                    option_1.image.sprite = water_desat;
                    option_1.interactable = false;
                    option_2.image.sprite = grass_desat;
                    option_2.interactable = false;
                    break;
                case type.Water:
                    option_0.image.sprite = fire_desat;
                    option_0.interactable = false;
                    if (play_data.instance.player_resource[play_data.instance.whosturn, 1] > 1){
                        option_1.image.sprite = water;
                        option_1.interactable = true;
                    }
                    else{
                        option_1.image.sprite = water_desat;
                        option_1.interactable = false;
                    }
                    option_2.image.sprite = grass_desat;
                    option_2.interactable = false;
                    break;
                case type.Earth:
                    option_0.image.sprite = fire_desat;
                    option_0.interactable = false;
                    option_1.image.sprite = water_desat;
                    option_1.interactable = false;
                    if (play_data.instance.player_resource[play_data.instance.whosturn, 2] > 1){
                        option_2.image.sprite = grass;
                        option_2.interactable = true;
                    }
                    else{
                        option_2.image.sprite = grass_desat;
                        option_2.interactable = false;
                    }
                    break;
                case type.Empty:
                    if (play_data.instance.player_resource[play_data.instance.whosturn, 0] > 1){
                        option_0.image.sprite = fire;
                        option_0.interactable = true;
                    }
                    else{
                        option_0.image.sprite = fire_desat;
                        option_0.interactable = false;
                    }
                    if (play_data.instance.player_resource[play_data.instance.whosturn, 1] > 1){
                        option_1.image.sprite = water;
                        option_1.interactable = true;
                    }
                    else{
                        option_1.image.sprite = water_desat;
                        option_1.interactable = false;
                    }
                    if (play_data.instance.player_resource[play_data.instance.whosturn, 2] > 1){
                        option_2.image.sprite = grass;
                        option_2.interactable = true;
                    }
                    else{
                        option_2.image.sprite = grass_desat;
                        option_2.interactable = false;
                    }
                    break;
                }
            }
            else //Attack(tile's owner != player of current turn)
            {
                if (play_data.instance.player_resource[play_data.instance.whosturn, 0] > 1)
                {
                    option_0.interactable = true;
                }
                else
                {
                    option_0.interactable = false;
                }
                if (play_data.instance.player_resource[play_data.instance.whosturn, 1] > 1)
                {
                    option_1.interactable = true;
                }
                else
                {
                    option_1.interactable = false;
                }
                if (play_data.instance.player_resource[play_data.instance.whosturn, 2] > 1)
                {
                    option_2.interactable = true;
                }
                else
                {
                    option_2.interactable = false;
                }
                action_title.text = "Attack";
            }
            #endregion 
        }
        else{
            action_title.text = "Explore";
            action_type.text = "Exploring...";
            option_0.image.sprite = empty;
            option_1.image.sprite = empty;
            option_2.image.sprite = empty;
            option_0.interactable = false;
            option_1.interactable = false;
            option_2.interactable = false;
        }

    }
}
