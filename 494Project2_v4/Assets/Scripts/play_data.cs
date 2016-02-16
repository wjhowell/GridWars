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
    public bool game_start;
    public bool message;
	//Audio
	public AudioSource audSource;
	public AudioClip fireball;
	public AudioClip waterSplash;
	public AudioClip grassCut;
	public AudioClip defend;
	public AudioClip claim;
	public AudioClip endTurn;
	public int loser_count = 0;

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
    public int[] tiles_owned = new int[4];
    //public List<coordinate>[] player_property = new List<coordinate>[4];
    //turn moves
    public int moves_remain;
    //map sprites
    public Sprite[] empty;
    public Sprite[] fire;
    public Sprite[] water;
    public Sprite[] earth;

    public List<coordinate>[] player_mine = new List<coordinate>[4];

    public int whosturn;


    // Use this for initialization
    void Start () {
        game_start = false;
        message = false;
        audSource = GetComponent<AudioSource> ();

        moves_remain = 2;
        whosturn = 0;
        instance = this;
        //SetupBlankBoard(14, 10);
        for (int p=0; p < 4; p++)
        {
            tiles_owned[p] = 0;
            for(int q=0; q < 3; q++)
            {
                player_resource[p, q] = 0;
                player_income[p, q] = 0;
            }
        }
        
    }
	
	// Update is called once per frame
	void Update () {
        
		// breaks HUD/play_data code (no unavailabity checking on play_data level)
		//if (Input.GetKeyDown (KeyCode.Q)) {
		//	option_0 ();
		//} else if (Input.GetKeyDown (KeyCode.A)) {
		//	option_1 ();
		//} else if (Input.GetKeyDown (KeyCode.Z)) {
		//	option_2 ();
		//} else if (Input.GetKeyDown (KeyCode.X)) {
		//	next_turn ();
		//}
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


        owner[4, 2] = 0;
        owner[4, 7] = 1;
        owner[9, 7] = 2;
        owner[9, 2] = 3;
        tile_type[4, 2] = type.Empty;
        tile_type[4, 7] = type.Empty;
        tile_type[9, 7] = type.Empty;
        tile_type[9, 2] = type.Empty;
        tiles_owned[0] = 1;
        tiles_owned[1] = 1;
        tiles_owned[2] = 1;
        tiles_owned[3] = 1;
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

    void SetupBlankBoard(int cols, int rows)
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
                nt.GetComponent<Transform>().position = new Vector3((float)x, (float)y + 10.0f + 10.0f * Random.value, 0f);
                nt.GetComponent<Rigidbody>().velocity = new Vector3(0f, -20.0f, 0f);
                nt.col = x;
                nt.row = y;
                nt.owner = -1;
                tiles[x, y] = nt;
            }
        }
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
		audSource.PlayOneShot (endTurn, 1.5f);
        whosturn++;
        if (whosturn == 4)
        {
            whosturn = 0;
        }
        // make sure player is still in game
        while (tiles_owned[whosturn] <= 0)
        {
            whosturn++;
            if (whosturn == 4)
            {
                whosturn = 0;
            }
        }
        moves_remain = 2;

        for (int p = 0; p < 3; p++)
        {
                player_resource[whosturn, p] += player_income[whosturn, p];
        }
        for (int col = 0; col < 14; col++)
        {
            for (int row = 0; row < 10; row++)
            {
                if (owner[col, row] == whosturn && tile_type[col, row]!=type.Empty&& remaining[col, row] !=10)
                {
                    tiles[col, row].DisplayScoreChange(1);
                    remaining[col, row] --;
                    if (remaining[col, row] == 0)
                    {
                        player_income[whosturn, type2int(tile_type[col, row])]--;
                        tile_type[col, row] = type.Empty;
                    }
                }
            }
        }

        random_events();



        UpdateSelectableTiles();
    }

    public void random_events()
    {
        float seed = Random.value;
        #region randomly get/lost resources
        if (seed < 0.04)
        {
            Hud.instance.Panel1.gameObject.SetActive(false);
            Hud.instance.Panel2.gameObject.SetActive(false);
            Hud.instance.Panel3.gameObject.SetActive(true);
            message = true;
            player_resource[whosturn, 0] += 10;
            Hud.instance.instruction_title.text = "+10 resources :)";
            Hud.instance.instruction_text.text = "Congratulations!\nA volcano erupts and you picked up 10 fire.";
            Hud.instance.Instruction_cube.GetComponent<SpriteRenderer>().sprite = fire[0];
            if(whosturn == 0){
            	tiles[4, 2].DisplayScoreChange(10);
            }
            else if(whosturn == 1){
            	tiles[4, 7].DisplayScoreChange(10);
            }
            else if(whosturn == 2){
            	tiles[9, 7].DisplayScoreChange(10);
            }
            else if(whosturn == 3){
            	tiles[9, 2].DisplayScoreChange(10);
            }
            return;
        }
        else if (seed < 0.08)
        {
            Hud.instance.Panel1.gameObject.SetActive(false);
            Hud.instance.Panel2.gameObject.SetActive(false);
            Hud.instance.Panel3.gameObject.SetActive(true);
            message = true;
            player_resource[whosturn, 1] += 10;
            Hud.instance.instruction_title.text = "+10 resources :)";
            Hud.instance.instruction_text.text = "Congratulations!\nIt's raining and you picked up 10 water.";
            Hud.instance.Instruction_cube.GetComponent<SpriteRenderer>().sprite = water[0];
            if(whosturn == 0){
            	tiles[4, 2].DisplayScoreChange(10);
            }
            else if(whosturn == 1){
            	tiles[4, 7].DisplayScoreChange(10);
            }
            else if(whosturn == 2){
            	tiles[9, 7].DisplayScoreChange(10);
            }
            else if(whosturn == 3){
            	tiles[9, 2].DisplayScoreChange(10);
            }
            return;
        }
        else if (seed < 0.12)
        {
            Hud.instance.Panel1.gameObject.SetActive(false);
            Hud.instance.Panel2.gameObject.SetActive(false);
            Hud.instance.Panel3.gameObject.SetActive(true);
            message = true;
            player_resource[whosturn, 2] += 10;
            Hud.instance.instruction_title.text = "+10 resources :)";
            Hud.instance.instruction_text.text = "Congratulations!\nYou just picked up 10 grass in the wild.";
            Hud.instance.Instruction_cube.GetComponent<SpriteRenderer>().sprite = earth[0];
            if(whosturn == 0){
            	tiles[4, 2].DisplayScoreChange(10);
            }
            else if(whosturn == 1){
            	tiles[4, 7].DisplayScoreChange(10);
            }
            else if(whosturn == 2){
            	tiles[9, 7].DisplayScoreChange(10);
            }
            else if(whosturn == 3){
            	tiles[9, 2].DisplayScoreChange(10);
            }
            return;
        }
        else if (seed < 0.16)
        {
            if (player_resource[whosturn, 0] >= 10)
            {
                Hud.instance.Panel1.gameObject.SetActive(false);
                Hud.instance.Panel2.gameObject.SetActive(false);
                Hud.instance.Panel3.gameObject.SetActive(true);
                message = true;
                player_resource[whosturn, 0] -= 10;
            	Hud.instance.instruction_title.text = "-10 resources :(";
                Hud.instance.instruction_text.text = "I'm sorry\nAliens just robbed you for 10 fire.";
                Hud.instance.Instruction_cube.GetComponent<SpriteRenderer>().sprite = fire[0];
                if(whosturn == 0){
	            	tiles[4, 2].DisplayScoreChange(-10);
	            }
	            else if(whosturn == 1){
	            	tiles[4, 7].DisplayScoreChange(-10);
	            }
	            else if(whosturn == 2){
	            	tiles[9, 7].DisplayScoreChange(-10);
	            }
	            else if(whosturn == 3){
	            	tiles[9, 2].DisplayScoreChange(-10);
	            }
                return;
            }
            else {
                return;
            }
            
        }
        else if (seed < 0.20)
        {
            if (player_resource[whosturn, 1] >= 10)
            {
                Hud.instance.Panel1.gameObject.SetActive(false);
                Hud.instance.Panel2.gameObject.SetActive(false);
                Hud.instance.Panel3.gameObject.SetActive(true);
                message = true;
                player_resource[whosturn, 1] -= 10;
            	Hud.instance.instruction_title.text = "-10 resources :(";
                Hud.instance.instruction_text.text = "I'm sorry\nAliens just robbed you for 10 water.";
                Hud.instance.Instruction_cube.GetComponent<SpriteRenderer>().sprite = water[0];
                if(whosturn == 0){
	            	tiles[4, 2].DisplayScoreChange(-10);
	            }
	            else if(whosturn == 1){
	            	tiles[4, 7].DisplayScoreChange(-10);
	            }
	            else if(whosturn == 2){
	            	tiles[9, 7].DisplayScoreChange(-10);
	            }
	            else if(whosturn == 3){
	            	tiles[9, 2].DisplayScoreChange(-10);
	            }
	            return;
            }
            else
            {
                return;
            }
        }
        else if (seed < 0.24)
        {
            if (player_resource[whosturn, 2] >= 10)
            {
                Hud.instance.Panel1.gameObject.SetActive(false);
                Hud.instance.Panel2.gameObject.SetActive(false);
                Hud.instance.Panel3.gameObject.SetActive(true);
                message = true;
                player_resource[whosturn, 2] -= 10;
            	Hud.instance.instruction_title.text = "-10 resources :(";
                Hud.instance.instruction_text.text = "I'm sorry\nAliens just robbed you for 10 grass.";
                Hud.instance.Instruction_cube.GetComponent<SpriteRenderer>().sprite = earth[0];
                if(whosturn == 0){
	            	tiles[4, 2].DisplayScoreChange(-10);
	            }
	            else if(whosturn == 1){
	            	tiles[4, 7].DisplayScoreChange(-10);
	            }
	            else if(whosturn == 2){
	            	tiles[9, 7].DisplayScoreChange(-10);
	            }
	            else if(whosturn == 3){
	            	tiles[9, 2].DisplayScoreChange(-10);
	            }
	            return;
            }
            else
            {
                return;
            }
        }
        #endregion
        #region randomly get tiles
        else if (seed <0.36)
        {
            if (tiles_owned[0]+ tiles_owned[1]+ tiles_owned[2]+ tiles_owned[3]>=120)
            {
                return;
            }
            else
            {
                int map_col = 14;
                int map_row = 10;
                int seed_col = Random.Range(0, map_col);
                int seed_row = Random.Range(0, map_row);
                while (owner[seed_col,seed_row]!=-1)
                {
                    seed_col = Random.Range(0, map_col);
                    seed_row = Random.Range(0, map_row);
                }
                owner[seed_col, seed_row] = whosturn;

                Hud.instance.Panel1.gameObject.SetActive(false);
                Hud.instance.Panel2.gameObject.SetActive(false);
                Hud.instance.Panel3.gameObject.SetActive(true);
                message = true;
            	Hud.instance.instruction_title.text = "+1 tile :)";
                Hud.instance.instruction_text.text = "Congratulations!\n You were gifted a new tile at column " + seed_col.ToString() + ", row " + seed_row.ToString() + "!";
                tiles_owned[whosturn]++;
                switch(tile_type[seed_col, seed_row]){
                    case type.Empty:
                        Hud.instance.Instruction_cube.GetComponent<SpriteRenderer>().sprite = empty[whosturn + 1];
                        break;
                    case type.Fire:
                        Hud.instance.Instruction_cube.GetComponent<SpriteRenderer>().sprite = fire[whosturn + 1];
                        player_income[whosturn, 0]++;
                        player_resource[whosturn, 0]++;
                        remaining[seed_col, seed_row]--;
                        Hud.instance.instruction_text.text += "\nFire income +1!";
                        tiles[seed_col, seed_row].DisplayScoreChange(1);
                        break;
                    case type.Water:
                        Hud.instance.Instruction_cube.GetComponent<SpriteRenderer>().sprite = water[whosturn + 1];
                        player_income[whosturn, 1]++;
                        player_resource[whosturn, 1]++;
                        remaining[seed_col, seed_row]--;
                        Hud.instance.instruction_text.text += "\nWater income +1!";
                        tiles[seed_col, seed_row].DisplayScoreChange(1);
                        break;
                    case type.Earth:
                        Hud.instance.Instruction_cube.GetComponent<SpriteRenderer>().sprite = earth[whosturn + 1];
                        player_income[whosturn, 2]++;
                        player_resource[whosturn, 2]++;
                        remaining[seed_col, seed_row]--;
                        Hud.instance.instruction_text.text += "\nGrass income +1!";
                        tiles[seed_col, seed_row].DisplayScoreChange(1);
                        break;

                }
                return;
            }
            

        }
        #endregion
        else
        {
            return;
        }
    }

    public void option_0()
    {
        moves_remain--;
        if (owner[current_select_col, current_select_row] == -1) //long-term claim
        {
			audSource.pitch = Random.Range (lowPitchRange, highPitchRange);
			audSource.PlayOneShot (claim, 1.5f);

            owner[current_select_col, current_select_row] = whosturn;
            ++tiles_owned[whosturn];
            tiles[current_select_col, current_select_row].DisplayScoreChange(1);
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
        else if (owner[current_select_col, current_select_row] == whosturn) //Fire Defense
        {
            DoDefense(type.Fire);
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
        if (owner[current_select_col, current_select_row] == -1) //one-time claim
        {
			audSource.pitch = Random.Range (lowPitchRange, highPitchRange);
			audSource.PlayOneShot (claim, 1.5f);

            owner[current_select_col, current_select_row] = whosturn;  
            remaining[current_select_col, current_select_row] = 0;
			int type_index = type2int(tile_type [current_select_col, current_select_row]);
            if (type_index != -1)
            {
                tiles[current_select_col, current_select_row].DisplayScoreChange(5);
                player_resource[whosturn, type_index] += 5;
            }
            tile_type[current_select_col, current_select_row] = type.Empty;
            ++tiles_owned[whosturn];
        }
        else if (owner[current_select_col, current_select_row] == whosturn)//Water Defense
        {
            DoDefense(type.Water);
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
            DoDefense(type.Earth);
        }
        else //Earth attack
        {
			DoAttack (current_select_col, current_select_row, type.Earth, whosturn);
        }
        UpdateSelectableTiles();
    }

    void DoDefense(type element)
    {
    	audSource.pitch = 1.0f + defense[current_select_col, current_select_row] / 10f;
	    audSource.PlayOneShot(defend, 1.0f);

	    if(defense[current_select_col, current_select_row] < 5){
            tiles[current_select_col, current_select_row].DisplayScoreChange(1);
	        defense_type[current_select_col, current_select_row] = element;
	        defense[current_select_col, current_select_row]++;
	        player_resource[whosturn, type2int(element)] -= 1;
	    }
	    else{
	    	tiles[current_select_col, current_select_row].DisplayScoreChange(0);
	        player_resource[whosturn, type2int(element)] -= 1;
	    }
    }

    public void Continue()
    {
        if (!game_start)
        {
            SetupBoard(14, 10);
            game_start = true;
            GameObject.Find("Start").SetActive(false);
        }
        Hud.instance.Panel1.gameObject.SetActive(true);
        Hud.instance.Panel2.gameObject.SetActive(true);
        Hud.instance.Panel3.gameObject.SetActive(false);
        message = false;
    }

    void DoAttack(int def_col, int def_row, type element, int current_turn) {
		//Audio
		audSource.pitch = Random.Range (lowPitchRange, highPitchRange);
        type def_type = defense_type[def_col, def_row];
        int damage = 0;
        switch (element) {
		case type.Fire:
			audSource.PlayOneShot (fireball, 1.0f);
                switch (def_type)
                {
                    case type.Fire:
                        damage = 1;
                        break;
                    case type.Water:
                        damage = 0;
                        break;
                    case type.Earth:
                        damage = 2;
                        break;
                    default:
                        damage = 1;
                        break;
                }
                break;
		case type.Water:
			audSource.PlayOneShot (waterSplash, 1.0f);
                switch (def_type)
                {
                    case type.Fire:
                        damage = 2;
                        break;
                    case type.Water:
                        damage = 1;
                        break;
                    case type.Earth:
                        damage = 0;
                        break;
                    default:
                        damage = 1;
                        break;
                }
                break;
		case type.Earth:
			audSource.PlayOneShot (grassCut, 1.0f);
                switch (def_type)
                {
                    case type.Fire:
                        damage = 0;
                        break;
                    case type.Water:
                        damage = 2;
                        break;
                    case type.Earth:
                        damage = 1;
                        break;
                    default:
                        damage = 1;
                        break;
                }
                break;
		default:
                damage = 1;
                break;
		}

        defense[def_col, def_row] -= damage;
        tiles[def_col, def_row].DisplayScoreChange(-damage);
        if (player_resource [whosturn, type2int(element)] > 0) {
			player_resource [whosturn, type2int(element)] -= 1;
		} else {
			// tell player, insufficent resoures
			// better yet, grey out the box
			++moves_remain;
		}

		bool someone_lost = false;
		bool someone_won = false;
		int loser = owner[def_col, def_row];

		// barrier down, player takes immediately
		if (defense[def_col, def_row] < 0)
		{
			defense_type[def_col, def_row] = type.Empty;
			defense[def_col, def_row] = 0;
            --tiles_owned[owner[def_col, def_row]];

            if(tiles_owned[loser] <= 0){
            	someone_lost = true;
            	++loser_count;
            	if(loser_count == 3){
            		someone_won = true;
            	}
            }


            
            player_resource[whosturn, 2] -= 10;
        	Hud.instance.instruction_title.text = "-10 resources :(";
            Hud.instance.instruction_text.text = "I'm sorry\nAliens just robbed you for 10 grass.";
            Hud.instance.Instruction_cube.GetComponent<SpriteRenderer>().sprite = earth[0];




            owner[def_col, def_row] = current_turn;
            ++tiles_owned[current_turn];

        }

		// make the tile shake :)
		tiles[def_col, def_row].shake_delay = 0.5f;

		if(someone_lost){
			if(!someone_won){
				Hud.instance.Panel1.gameObject.SetActive(false);
	            Hud.instance.Panel2.gameObject.SetActive(false);
	            Hud.instance.Panel3.gameObject.SetActive(true);
	            message = true;
	            Hud.instance.instruction_title.text = "You lost!";
	            Hud.instance.instruction_text.text = "I'm sorry, player " + (loser + 1).ToString() + ", but you have lost the GridWars.";
	            Hud.instance.Instruction_cube.GetComponent<SpriteRenderer>().sprite = empty[loser + 1];
	        }
	        else{
	        	Hud.instance.Panel1.gameObject.SetActive(false);
	            Hud.instance.Panel2.gameObject.SetActive(false);
	            Hud.instance.Panel3.gameObject.SetActive(true);
	            message = true;
	            Hud.instance.instruction_title.text = "You won!";
	            Hud.instance.instruction_text.text = "Congratulations, player " + (whosturn + 1).ToString() + ", you have won the GridWars!";
	            Hud.instance.Instruction_cube.GetComponent<SpriteRenderer>().sprite = empty[whosturn + 1];
	        }
		}

	}

	public void DoSuperAttack(int col, int row) {
		List<tile> targets = new List<tile>();
		if (!HasTileAt (col, row))
			throw new UnityException ("There is no target at: " + col + ", " + row + "!");
		else
			targets.Add (tiles [col, row]);
		if (HasTileAt(col + 1, row))
			targets.Add (tiles [col + 1, row]);
		if (HasTileAt(col - 1, row))
			targets.Add (tiles [col - 1, row]);
		if (HasTileAt(col, row + 1))
			targets.Add (tiles [col, row + 1]);
		if (HasTileAt(col, row - 1))
			targets.Add (tiles [col, row - 1]);

		// sync issues between play_data and tile collection may arrise!
		foreach (tile t in targets) {
			owner[t.col, t.row] = t.owner = whosturn;
			tile_type[t.col, t.row] = defense_type[t.col, t.row] = t.defense_type = t.tile_type = type.Empty;
			defense[t.col, t.row] = t.defense = 0;
			t.shake_delay = 0.5f;
		}
	}

	bool HasTileAt(int col, int row) {
		return (col >= 0 && col < tiles.GetLength (0) && row >= 0 && row < tiles.GetLength (1));
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

	static public Color OwnerIntToColor(int owner_number) {
		return Color.black;
	}
}
