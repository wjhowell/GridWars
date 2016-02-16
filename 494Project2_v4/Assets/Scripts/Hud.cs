using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Hud : MonoBehaviour {

    public static Hud instance;

    public Text location_text;
    public Text owner_text;
    public Text type_text;
    public Text remaining_text;
    public Text defense_text;
    public Text info_text;
    public Text option_0_text;
    public Text option_1_text;
    public Text option_2_text;
    public Text instruction_text;
    public Text Continue_text;

    public Button option_0;
    public Button option_1;
    public Button option_2;
    public Button end_turn;
    public Button Continue;




    public Camera guicame;

    public Transform Panel1;
    public Transform Panel2;
    public Transform Panel3;
    public GameObject Instruction_cube;


    // Use this for initialization
    void Start () {
        instance = this;
        Instruction_cube = GameObject.Find("Instruction_cube");
        Panel1 = transform.Find("Panel1");
        Panel2 = transform.Find("Panel2");
        Panel3 = transform.Find("Panel3");
        Panel1.gameObject.SetActive(false);
        Panel2.gameObject.SetActive(false);
        Panel3.gameObject.SetActive(true);
        Continue_text.text = "Start";
        instruction_text.text = "GRID WARS";
        guicame.backgroundColor = Color.blue;



        end_turn.onClick.AddListener(() => play_data.instance.next_turn());
        option_0.onClick.AddListener(() => play_data.instance.option_0());
        option_1.onClick.AddListener(() => play_data.instance.option_1());
        option_2.onClick.AddListener(() => play_data.instance.option_2());
        Continue.onClick.AddListener(() => play_data.instance.Continue());
    }
	
	// Update is called once per frame
	void Update () {
		
		int cur_row = play_data.instance.current_select_row;
		int cur_col = play_data.instance.current_select_col;
		int whos_turn = play_data.instance.whosturn;

        if (play_data.instance.game_start)
        {
            Continue_text.text = "Continue";
			guicame.backgroundColor = play_data.OwnerIntToColor(whos_turn);
        }

		location_text.text = "Row: " + cur_col.ToString() + "      Column: " + cur_row.ToString();
		if (play_data.instance.owner[cur_col, cur_row] == -1)
        {
            owner_text.text = "Owners: None";
        }
        else
        {
			owner_text.text = "Owner: Player_" + play_data.instance.owner[cur_col, cur_row].ToString();
        } 
		type_text.text = "Type: " + play_data.instance.tile_type[cur_col, cur_row].ToString();
		remaining_text.text = "Remaining: " + play_data.instance.remaining[cur_col, cur_row].ToString();
        if (play_data.instance.defense_type[cur_col, cur_row] == type.Empty)
        {
            defense_text.text = "Defense: None";
        }
        else
        {
            defense_text.text = "Defense: " + play_data.instance.defense[cur_col, cur_row].ToString()
                          + "     type: " + play_data.instance.defense_type[cur_col, cur_row].ToString();
        }

        info_text.fontSize = 8;
		info_text.text = "Player_" + whos_turn.ToString() + "'s turn   " + play_data.instance.moves_remain.ToString()+"/2"
			+ "\nFire: " + play_data.instance.player_resource[whos_turn, 0].ToString()+"/"+ play_data.instance.player_income[whos_turn, 0].ToString()
			+ "\nWater: " + play_data.instance.player_resource[whos_turn, 1].ToString() + "/" + play_data.instance.player_income[whos_turn, 1].ToString()
			+ "\nGrass: " + play_data.instance.player_resource[whos_turn, 2].ToString() + "/" + play_data.instance.player_income[whos_turn, 2].ToString();

		// select possible player actions
        if (play_data.instance.moves_remain != 0 && play_data.instance.IsSelectable[cur_col, cur_row])
        {
            #region if you still have move and selectable
            //need path_find function to determine if it's accessible

			// for super weapon: user selects a tile 
			if (SuperWeapon.instance.col == cur_col && SuperWeapon.instance.row == cur_row && SuperWeapon.instance.owner == whos_turn) {
				option_0.interactable = option_1.interactable = option_2.interactable = false;
				option_1_text.text = option_2_text.text = "";
				option_0_text.text = SuperWeapon.instance.ActivationText();
				if (SuperWeapon.instance.at_max_charge) {
					option_0.interactable = true;
				}
			} else if (play_data.instance.owner[cur_col, cur_row] == -1) {
				// if the tile has no owner
                if (play_data.instance.tile_type[cur_col, cur_row] == type.Empty) {
                    option_0_text.text = "Claim";
                    option_1_text.text = "";
                    option_2_text.text = "";
                    option_0.interactable = true;
                    option_1.interactable = false;
                    option_2.interactable = false;
                }
                else
                {
					// unowned resource
                    option_0_text.text = "Claim one-time";
                    option_1_text.text = "Claim long-term";
                    option_2_text.text = "";
                    option_0.interactable = true;
                    option_1.interactable = true;
                    option_2.interactable = false;
                }
			} else if (play_data.instance.owner[cur_col, cur_row] == whos_turn) {
				// Defense (tile's owner = player of current turn)
                option_0_text.text = "Fire Defend";
                option_1_text.text = "Water Defend";
                option_2_text.text = "Grass Defend";
                switch (play_data.instance.defense_type[cur_col, cur_row]) {
                    case type.Fire:
						if (play_data.instance.player_resource[whos_turn, 0] >= play_data.COST_TO_DEFEND) 
							option_0.interactable = true;//replace 0 with cost to defend
                        else 
							option_0.interactable = false;
                        option_1.interactable = false;
                        option_2.interactable = false;
                        break;
                    case type.Water:
                        option_0.interactable = false;
						if (play_data.instance.player_resource[whos_turn, 1] >= play_data.COST_TO_DEFEND) 
							option_1.interactable = true;
                        else 
							option_1.interactable = false;
                        option_2.interactable = false;
                        break;
                    case type.Earth:
                        option_0.interactable = false;
                        option_1.interactable = false;
						if (play_data.instance.player_resource[whos_turn, 2] >= play_data.COST_TO_DEFEND) 
							option_2.interactable = true;
                        else
							option_2.interactable = false;
                        break;
                    case type.Empty:
						if (play_data.instance.player_resource[whos_turn, 0] >= play_data.COST_TO_DEFEND)
							option_0.interactable = true;
                        else
							option_0.interactable = false;
						if (play_data.instance.player_resource[whos_turn, 1] >= play_data.COST_TO_DEFEND)
							option_1.interactable = true;
                        else 
							option_1.interactable = false;
						if (play_data.instance.player_resource[whos_turn, 2] >= play_data.COST_TO_DEFEND) 
							option_2.interactable = true;
                        else
							option_2.interactable = false;
                        break;
                }
            }
            else //Attack(tile's owner != player of current turn)
            {
				//replace 0 with cost to attack
                // done! -AJ
				option_0.interactable = (play_data.instance.player_resource[whos_turn, 0] >= play_data.COST_TO_ATTACK);
				option_1.interactable = (play_data.instance.player_resource[whos_turn, 1] >= play_data.COST_TO_ATTACK);
				option_2.interactable = (play_data.instance.player_resource[whos_turn, 2] >= play_data.COST_TO_ATTACK);

                option_0_text.text = "Fire Attack";
                option_1_text.text = "Water Attack";
                option_2_text.text = "Grass Attack";
            }
            #endregion 
        }
        else
        {
            #region if you don't have move left
            option_0_text.text = "";
            option_1_text.text = "";
            option_2_text.text = "";
            option_0.interactable = false;
            option_1.interactable = false;
            option_2.interactable = false;
            #endregion
        }


    }
}
