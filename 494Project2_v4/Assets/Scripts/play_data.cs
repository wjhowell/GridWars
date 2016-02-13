﻿using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

public enum type { Fire, Water, Earth, Empty };

public struct coordinate {
    public int x;
    public int y;
    public coordinate(int a, int b)
    {
        x = a;
        y = b;
    }
}

public class play_data : MonoBehaviour {
    public static play_data instance;
	//Audio
	public AudioSource audSource;
	public AudioClip fireball;
	public AudioClip waterSplash;
	public AudioClip grassCut;

	public float lowPitchRange = .75F;
	public float highPitchRange = 1.25F;

    public tile tilePrefab;
    //map data
    public int[,] owner = new int[14,10];
    public type[,] tile_type = new type[14, 10];
    public int[,] remaining = new int[14, 10];
    public type[,] defense_type = new type[14, 10];
    public int[,] defense = new int[14, 10];
    public bool[,] IsSelectable = new bool[14, 10];
	public tile[,] tiles;
    //select data
    public int current_select_col;
    public int current_select_row;
    //4 player data in order of Fire, Water, Earth
    public int[,] player_resource = new int[4,3];
    public int[,] player_income = new int[4, 3];
    //public List<coordinate>[] player_property = new List<coordinate>[4];
    //turn moves
    public int moves_remain;
    //map sprites
    public Sprite[] empty;
    public Sprite[] fire;
    public Sprite[] water;
    public Sprite[] earth;

    //public coordinate[] player_0;
    //public coordinate[] player_1;
    //public coordinate[] player_2;
    //public coordinate[] player_3;

    public List<coordinate>[] player_mine = new List<coordinate>[4];

    public int whosturn;


    // Use this for initialization
    void Start () {
		audSource = GetComponent<AudioSource> ();

        moves_remain = 2;
        whosturn = 0;
        instance = this;
        SetupBoard(14, 10);
        for (int p=0; p < 4; p++)
        {
            for(int q=0; q < 3; q++)
            {
                player_resource[p, q] = 0;
                player_income[p, q] = 0;
            }
        }
        
    }
	
	// Update is called once per frame
	void Update () {
        //print(owner[0,1]);
	}

    void SetupBoard(int cols, int rows)
    {
        owner = new int[cols, rows];
        tile_type = new type[cols, rows];
        remaining = new int[cols, rows];
        defense_type = new type[cols, rows];
        defense = new int[cols, rows];
		tiles = new tile[cols, rows];
        for (int x = 0; x != cols; ++x)
        {
            for (int y = 0; y != rows; ++y)
            {
                owner[x, y] = -1;
                defense_type[x, y] = tile_type[x, y] = type.Empty;
                remaining[x, y] = 0;
                defense[x, y] = 0;
                tile nt = Instantiate<tile>(tilePrefab);
                //nt.GetComponent<Transform>().position = new Vector3((float)x, (float)y, 0f);
				nt.GetComponent<Transform>().position = new Vector3((float)x, (float)y+10.0f + 10.0f * Random.value, 0f);
				nt.GetComponent<Rigidbody>().velocity = new Vector3(0f, -20.0f, 0f);
				nt.col = x;
                nt.row = y;
                nt.owner = -1;
				tiles [x, y] = nt;
//				nt.gameObject.SetActive (false);
//				StartCoroutine (genTile (nt));
            }
        }

        for (int p = 0; p < 14; p++)
        {
            for (int q = 0; q < 10; q++)
            {
                if (Random.value<0.1)
                    tile_type[p, q] = type.Fire;
                else if(Random.value < 0.2)
                    tile_type[p, q] = type.Water;
                else if (Random.value < 0.3)
                    tile_type[p, q] = type.Earth;
            }
        }


        owner[0, 0] = 0;
        owner[0, 9] = 1;
        owner[13, 9] = 2;
        owner[13, 0] = 3;
        tile_type[0, 0] = type.Empty;
        tile_type[0, 9] = type.Empty;
        tile_type[13, 9] = type.Empty;
        tile_type[13, 0] = type.Empty;
        for (int p = 0; p < 14; p++)
        {
            for (int q = 0; q < 10; q++)
            {
                if (tile_type[p, q] != type.Empty)
                {
                    remaining[p, q] = 10;
                }
            }
        }

        UpdateSelectableTiles();
    }

	IEnumerator genTile(tile nt){
		yield return new WaitForSeconds(Random.Range(0,1.25f));
		nt.gameObject.SetActive (true);
	}

    public void UpdateSelectableTiles() {
        for (int p = 0; p < 14; p++)
        {
            for (int q = 0; q < 10; q++)
            {
               if(owner[p, q] == whosturn)
                {
                    IsSelectable[p, q] = true;
                }
                else
                {
                    IsSelectable[p, q] = false;
                }
               
            }
        }
        for (int p = 0; p < 14; p++)
        {
            for (int q = 0; q < 10; q++)
            {

                if (owner[p,q] == whosturn) {
                    if (p > 0) IsSelectable[p-1, q] = true;
                    if (p + 1 < 14) IsSelectable[p + 1, q] = true;
                    if (q > 0) IsSelectable[p, q - 1] = true;
                    if (q + 1 < 10) IsSelectable[p, q + 1] = true;
                }
            }
        }
    }

    public void next_turn()
    {
        whosturn++;
        moves_remain = 2;
        if (whosturn == 4)
        {
            whosturn = 0;
        }

        for (int p = 0; p < 3; p++)
        {
                player_resource[whosturn, p] += player_income[whosturn, p];
        }
        for (int p = 0; p < 3; p++)
        {
            for (int q = 0; q < 3; q++)
            {
                if (owner[p, q] == whosturn && tile_type[p,q]!=type.Empty&& remaining[p, q]!=10)
                {
                    remaining[p, q] --;
                    if (remaining[p, q] == 0)
                    {
                        player_income[whosturn, type2int(tile_type[p, q])]--;
                        tile_type[p, q] = type.Empty;
                    }
                }
            }
        }
        UpdateSelectableTiles();
    }

    public void option_0()
    {
        moves_remain--;
        if (owner[current_select_col, current_select_row] == -1) //one-time claim
        {
            owner[current_select_col, current_select_row] = whosturn;  
            remaining[current_select_col, current_select_row] = 0;
			int type_index = type2int(tile_type [current_select_col, current_select_row]);
			if (type_index != -1)
				player_resource[whosturn, type_index] += 5;
            tile_type[current_select_col, current_select_row] = type.Empty;
        }
        else if (owner[current_select_col, current_select_row] == whosturn) //Fire Defense
        {
            defense_type[current_select_col, current_select_row] = type.Fire;
            defense[current_select_col, current_select_row]++;
            player_resource[whosturn, 0] -= 5;
        }
        else //Fire attack
        {
			DoAttack (current_select_col, current_select_row, type.Fire, whosturn);
        }
        UpdateSelectableTiles();
    }

    public void option_1()
    {
        moves_remain--;
        if (owner[current_select_col, current_select_row] == -1) //long-term claim
        {
            owner[current_select_col, current_select_row] = whosturn;
            //coordinate temp = new coordinate(current_select_col, current_select_row);
            //player_property[whosturn].Add(temp);
            switch (tile_type[current_select_col, current_select_row])
            {
                case type.Fire:
                    //player_0[0]=new coordinate(1,1);
                    player_resource[whosturn, 0]++;
                    remaining[current_select_col, current_select_row]--;
                    player_income[whosturn, 0] ++;
                    break;
                case type.Water:
                    player_resource[whosturn, 1]++;
                    remaining[current_select_col, current_select_row]--;
                    player_income[whosturn, 1] ++;
                    break;
                case type.Earth:
                    player_resource[whosturn, 2]++;
                    remaining[current_select_col, current_select_row]--;
                    player_income[whosturn, 2] ++;
                    break;
            }

        }
        else if (owner[current_select_col, current_select_row] == whosturn)//Water Defense
        {
            defense_type[current_select_col, current_select_row] = type.Water;
            defense[current_select_col, current_select_row]++;
            player_resource[whosturn, 1] -= 5;
        }
        else //Water attack
        { 
			DoAttack (current_select_col, current_select_row, type.Water, whosturn);
        }
        UpdateSelectableTiles();
    }
    public void option_2()
    {
        moves_remain--;
        if (owner[current_select_col, current_select_row] == whosturn)//Earth Defense
        {
            defense_type[current_select_col, current_select_row] = type.Earth;
            defense[current_select_col, current_select_row]++;
            player_resource[whosturn, 2] -= 5;
        }
        else //Earth attack
        {
			DoAttack (current_select_col, current_select_row, type.Earth, whosturn);
        }
        UpdateSelectableTiles();
    }

	/// <summary>
	/// Performs an elemental attack onto a (an enemy's) tile.
	/// :TODO: what if attacker does not have sufficient resoures to attack.
	/// </summary>
	/// <param name="def_col">Defender's column.</param>
	/// <param name="def_row">Defender's row.</param>
	/// <param name="element">Element of the attack.</param>
	/// <param name="current_turn">Player index, for who's current turn.</param>
	void DoAttack(int def_col, int def_row, type element, int current_turn) {
		//Audio
		audSource.pitch = Random.Range (lowPitchRange, highPitchRange);
		switch (element) {
		case type.Fire:
			audSource.PlayOneShot (fireball, 1.0f);
			break;
		case type.Water:
			audSource.PlayOneShot (waterSplash, 1.0f);
			break;
		case type.Earth:
			audSource.PlayOneShot (grassCut, 1.0f);
			break;
		default:
			break;
		}

		defense [def_col, def_row]--;
		if (player_resource [whosturn, type2int(element)] > 0) {
			player_resource [whosturn, type2int(element)] -= 5;
		} else {
			// tell player, insufficent resoures
			// better yet, grey out the box
			++moves_remain;
		}

		// barrier down, player takes immediately
		if (defense[def_col, def_row] <= 0)
		{
			defense_type[def_col, def_row] = type.Empty;
			defense[def_col, def_row] = 0;
			owner[def_col, def_row] = current_turn;
		}

		// make the tile shake :)
		tiles[def_col, def_row].shake_delay = 0.5f;
	}

    int type2int(type input_type)
    {
        switch (input_type)
        {
            case type.Fire:
                return 0;
            case type.Water:
                return 1;
            case type.Earth:
                return 2;
        }
        return -1;
    }

	/// <summary>
	/// Converts owner index integer into the owner's color.
	/// </summary>
	/// <returns>color representing owner</returns>
	/// <param name="owner_number">owner index number</param>
	static public Color OwnerIntToColor(int owner_number) {
		switch (owner_number)
		{
		case 0:
			return Color.green;
		case 1:
			// purple
			return new Color(0.453f, 0.270f, 0.809f);
		case 2:
			return Color.red;
		case 3:
			return Color.yellow;
		}
		throw new UnityException ("Invalid owner number");
	}
}