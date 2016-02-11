using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Hud : MonoBehaviour {

    public Text location_text;
    public Text owner_text;
    public Text type_text;
    public Text remaining_text;
    public Text defense_text;
    public Text info_text;
    public Text option_0_text;
    public Text option_1_text;
    public Text option_2_text;

    public Button option_0;
    public Button option_1;
    public Button option_2;
    public Button end_turn;

    public Camera guicame;

    Color purple = new Color(0.453f, 0.270f, 0.809f);

    // Use this for initialization
    void Start () {
        end_turn.onClick.AddListener(() => play_data.instance.next_turn());
        option_0.onClick.AddListener(() => play_data.instance.option_0());
        option_1.onClick.AddListener(() => play_data.instance.option_1());
        option_2.onClick.AddListener(() => play_data.instance.option_2());
    }
	
	// Update is called once per frame
	void Update () {
        switch (play_data.instance.whosturn)
        {
            case 0:
                guicame.backgroundColor = Color.green;
                break;
            case 1:
                guicame.backgroundColor=purple;
                break;
            case 2:
                guicame.backgroundColor = Color.red;
                break;
            case 3:
                guicame.backgroundColor = Color.yellow;
                break;
        }
        
        location_text.text = "Row: " + play_data.instance.current_select_col.ToString() + "      Column: " + play_data.instance.current_select_row.ToString();
        if (play_data.instance.owner[play_data.instance.current_select_col, play_data.instance.current_select_row]==-1)
        {
            owner_text.text = "Owners: None";
        }
        else
        {
            owner_text.text = "Owners: Player_" + play_data.instance.owner[play_data.instance.current_select_col, play_data.instance.current_select_row].ToString();
        } 
        type_text.text = "Type: " + play_data.instance.tile_type[play_data.instance.current_select_col, play_data.instance.current_select_row].ToString();
        remaining_text.text = "Remaining: " + play_data.instance.remaining[play_data.instance.current_select_col, play_data.instance.current_select_row].ToString();
        if (play_data.instance.defense_type[play_data.instance.current_select_col, play_data.instance.current_select_row] == type.Empty)
        {
            defense_text.text = "Defense: None";
        }
        else
        {
            defense_text.text = "Defense: " + play_data.instance.defense[play_data.instance.current_select_col, play_data.instance.current_select_row].ToString()
                          + "     type: " + play_data.instance.defense_type[play_data.instance.current_select_col, play_data.instance.current_select_row].ToString();
        }

        info_text.fontSize = 8;
        info_text.text = "Player_" + play_data.instance.whosturn.ToString() + " 's turn   " + play_data.instance.moves_remain.ToString()+"/2"
                        + "\nFire: " + play_data.instance.player_resource[play_data.instance.whosturn, 0].ToString()+"/"+ play_data.instance.player_income[play_data.instance.whosturn, 0].ToString()
                        + "\nWater: " + play_data.instance.player_resource[play_data.instance.whosturn, 1].ToString() + "/" + play_data.instance.player_income[play_data.instance.whosturn, 1].ToString()
                        + "\nGrass: " + play_data.instance.player_resource[play_data.instance.whosturn, 2].ToString() + "/" + play_data.instance.player_income[play_data.instance.whosturn, 2].ToString();

        if (play_data.instance.moves_remain!=0 && play_data.instance.IsSelectable[play_data.instance.current_select_col, play_data.instance.current_select_row])
        {
            #region if you still have move and selectable
            //need path_find function to determine if it's accessible
            if (play_data.instance.owner[play_data.instance.current_select_col, play_data.instance.current_select_row] == -1) //if the tile has no owner
            {
                if (play_data.instance.tile_type[play_data.instance.current_select_col, play_data.instance.current_select_row] == type.Empty)
                {
                    option_0_text.text = "Claim";
                    option_1_text.text = "";
                    option_2_text.text = "";
                    option_0.interactable = true;
                    option_1.interactable = false;
                    option_2.interactable = false;
                }
                else
                {
                    option_0_text.text = "Claim one-time";
                    option_1_text.text = "Claim long-term";
                    option_2_text.text = "";
                    option_0.interactable = true;
                    option_1.interactable = true;
                    option_2.interactable = false;
                }

            }
            else if (play_data.instance.owner[play_data.instance.current_select_col, play_data.instance.current_select_row] == play_data.instance.whosturn)// Defense (tile's owner = player of current turn)
            {
                option_0_text.text = "Fire Defend";
                option_1_text.text = "Water Defend";
                option_2_text.text = "Grass Defend";
                switch (play_data.instance.defense_type[play_data.instance.current_select_col, play_data.instance.current_select_row])
                {
                    case type.Fire:
                        option_0.interactable = true;
                        option_1.interactable = false;
                        option_2.interactable = false;
                        break;
                    case type.Water:
                        option_0.interactable = false;
                        option_1.interactable = true;
                        option_2.interactable = false;
                        break;
                    case type.Earth:
                        option_0.interactable = false;
                        option_1.interactable = false;
                        option_2.interactable = true;
                        break;
                    case type.Empty:
                        option_0.interactable = true;
                        option_1.interactable = true;
                        option_2.interactable = true;
                        break;
                }
            }
            else //Attack(tile's owner != player of current turn)
            {
                option_0_text.text = "Fire Attack";
                option_1_text.text = "Water Attack";
                option_2_text.text = "Grass Attack";
                option_0.interactable = true;
                option_1.interactable = true;
                option_2.interactable = true;
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
