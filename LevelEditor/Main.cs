﻿using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using MelonLoader;
using MelonLoader.TinyJSON;
using UnityEngine.Tilemaps;
using static UnityEngine.RuleTile.TilingRuleOutput;
using System.Runtime.ConstrainedExecution;
using UnityEngine.SceneManagement;
using System.Reflection;
using System.Globalization;

namespace LevelEditor.Main
{
    public class Main : MelonMod
    {
        public List<Tile> pallete = new List<Tile>();
        public List<List<int>> tiles = new List<List<int>>();
        public Tilemap tileMap; // Grid > Tilemap GameObject
        public Tilemap InvisWalls; // Grid > Tilemap Invisible Wall
        public int width;
        public int height;

        public override void OnApplicationStart()
        {
            SetTilesInSet();
            pallete = LoadPaletteFromFile("Test");
            width = int.Parse(LoadRules("Test")[1]);
            height = int.Parse(LoadRules("Test")[2]);
        }

        public List<string> LoadRules(string folderName)
        {
            string modsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Mods", "Levels", folderName);
            string filePath = Path.Combine(modsFolder, "rules.txt");
            MelonLogger.Msg($"Loading file from path: {filePath}");
            List<string> lines = new List<string>();
            // Check if the file exists
            if (File.Exists(filePath))
            {
                try
                {
                    // Read all lines from the file
                    lines = new List<string>(File.ReadAllLines(filePath));
                    MelonLogger.Msg($"File 'rules.txt' loaded successfully from folder '{folderName}'.");
                }
                catch (Exception ex)
                {
                    MelonLogger.Error($"An error occurred while loading the file: {ex.Message}");
                }
            }
            else
            {
                MelonLogger.Warning($"File 'rules.txt' not found in folder '{folderName}' inside the mods directory.");
            }

            return lines;
        }
        public void SetStartPos(Vector2 pos)
        {
            GameObject pd = GameObject.Find("Death Collider");
            PlayerDeath pds = pd.GetComponent<PlayerDeath>();
            var field = typeof(PlayerDeath).GetField("playerStartPos", BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(pds, pos);
        }

        public List<string> LoadTilePosFile(string folderName)
        {
            // Define the path to the specified folder inside the mods folder
            string modsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Mods", "Levels", folderName);
            string filePath = Path.Combine(modsFolder, "tilepos.txt");
            MelonLogger.Msg($"Loading file from path: {filePath}");
            List<string> lines = new List<string>();

            // Check if the file exists
            if (File.Exists(filePath))
            {
                try
                {
                    // Read all lines from the file
                    lines = new List<string>(File.ReadAllLines(filePath));
                    MelonLogger.Msg($"File 'tilepos.txt' loaded successfully from folder '{folderName}'.");
                }
                catch (Exception ex)
                {
                    MelonLogger.Error($"An error occurred while loading the file: {ex.Message}");
                }
            }
            else
            {
                MelonLogger.Warning($"File 'tilepos.txt' not found in folder '{folderName}' inside the mods directory.");
            }

            return lines;
        }

        public List<Tile> LoadPaletteFromFile(string folderName)
        {
            // Define the path to the Palette folder inside the specified folder in the mods directory
            string modsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Mods", "Levels", folderName, "Palette");

            // Create a list to hold the tiles
            List<Tile> palette = new List<Tile>();

            Tile emptyTile = ScriptableObject.CreateInstance<Tile>();
            emptyTile.sprite = null; // No sprite for the empty tile
            palette.Add(emptyTile);
            // Check if the folder exists
            if (Directory.Exists(modsFolder))
            {
                // Get all image files in the folder (e.g., PNG, JPG)
                string[] imageFiles = Directory.GetFiles(modsFolder, "*.*", SearchOption.TopDirectoryOnly);

                foreach (string filePath in imageFiles)
                {
                    if (filePath.EndsWith(".png") || filePath.EndsWith(".jpg") || filePath.EndsWith(".jpeg"))
                    {
                        // Load the image file into a Texture2D
                        byte[] fileData = File.ReadAllBytes(filePath);
                        Texture2D texture = new Texture2D(2, 2);
                        if (texture.LoadImage(fileData))
                        {
                            // Create a Sprite from the Texture2D
                            Rect rect = new Rect(0, 0, texture.width, texture.height);
                            Sprite sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));

                            // Create a Tile from the Sprite
                            Tile tile = ScriptableObject.CreateInstance<Tile>();
                            tile.sprite = sprite;

                            // Add the tile to the palette list
                            palette.Add(tile);
                        }
                        else
                        {
                            Debug.LogWarning($"Failed to load image from file: {filePath}");
                        }
                    }
                }
            }
            else
            {
                Debug.LogWarning($"Palette folder '{modsFolder}' not found.");
            }
            MelonLogger.Msg(palette.Count);
            return palette;
        }

        public void SetTilesInSet()
        {
            List<string> temp = LoadTilePosFile("Test");
            if (temp == null || temp.Count == 0)
            {
                MelonLogger.Error("Failed to load lines from the file or the file is empty.");
                return;
            }

            List<List<int>> fullTemp = new List<List<int>>();

            foreach (string item in temp)
            {
                // Split the string by commas and trim any whitespace
                List<int> splitList = new List<int>();
                foreach (string str in item.Split(','))
                {
                    if (int.TryParse(str.Trim(), out int result))
                    {
                        splitList.Add(result);
                    }
                }

                // Add the split list to the 2D list
                fullTemp.Add(splitList);
            }

            tiles = fullTemp;
        }
        public void ClearAllTilesManually(string name)
        {
            MelonLogger.Msg("ClearAllTilesManually method called.");

            // Try to find the Tilemap GameObject
            GameObject tilemapGameObject = GameObject.Find(name);

            if (tilemapGameObject == null)
            {
                MelonLogger.Error("Tilemap GameObject not found.");
                return;
            }

            // Try to get the Tilemap component
            tileMap = tilemapGameObject.GetComponent<Tilemap>();

            if (tileMap == null)
            {
                MelonLogger.Error("Tilemap component not found on Tilemap GameObject.");
                return;
            }

            // Iterate over all tile positions and set them to null
            BoundsInt bounds = tileMap.cellBounds;
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    for (int z = bounds.zMin; z < bounds.zMax; z++)
                    {
                        tileMap.SetTile(new Vector3Int(x, y, z), null);
                    }
                }
            }

            tileMap.RefreshAllTiles();
            MelonLogger.Msg("All tiles cleared manually and Tilemap refreshed.");
        }

        public void SetAll()
        {
            MelonLogger.Msg("SetAll method called.");
            SetStartPos(new Vector2(float.Parse(LoadRules("Test")[3]), float.Parse(LoadRules("Test")[4])));
            // Try to find the Tilemap GameObject
            GameObject tilemapGameObject = GameObject.Find("Tilemap Gameobject");
            GameObject coverT = GameObject.Find("Ground Cover");

            if (tilemapGameObject == null)
            {
                MelonLogger.Error("Tilemap GameObject not found.");
                return;
            }
            ClearAllTilesManually("Tilemap Gameobject");
            ClearAllTilesManually("Ground Cover");
            ClearAllTilesManually("Ground Cover Metal");

            // Try to get the Tilemap component
            tileMap = tilemapGameObject.GetComponent<Tilemap>();
            Tilemap gct = coverT.GetComponent<Tilemap>();

            if (tileMap == null)
            {
                MelonLogger.Error("Tilemap component not found on Tilemap GameObject.");
                return;
            }

            if (tiles == null || tiles.Count == 0)
            {
                MelonLogger.Error("Tiles list is empty or not initialized.");
                return;
            }

            while (tilemapGameObject.transform.childCount > 0)
            {
                GameObject.DestroyImmediate(tilemapGameObject.transform.GetChild(0).gameObject);
            }
            PrepMD();
            GroundCoverController gcc = coverT.GetComponent<GroundCoverController>();
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (i < tiles.Count && j < tiles[i].Count)
                    {
                        if (tiles[i][j] != 0)
                        {
                            tileMap.SetTile(new Vector3Int(j, i, 0), pallete[tiles[i][j]]);
                            gct.SetTile(new Vector3Int(j, i, 0), gcc.groundTile);
                            GameObject temp = new GameObject();
                            temp.transform.parent = tileMap.transform;
                            temp.transform.localScale = new Vector3(0.5f, 1, 0.5f);
                            temp.tag = "Metal";
                            temp.layer = 8;
                            temp.transform.position = new Vector3Int(j, i, 0);
                        }
                    }
                    else
                    {
                        MelonLogger.Error($"Index out of range: i={i}, j={j}");
                    }
                }
            }
            tileMap.gameObject.transform.position = new Vector2(-0.5f, -0.5f);
            /*tileMap.gameObject.GetComponent<TilemapCollider2D>().enabled = true;
            GameObject cgc = GameObject.Find("Ground Cover");
            if (cgc != null)
            {
                GroundCoverController groundCoverController = cgc.GetComponent<GroundCoverController>();
                if (groundCoverController != null)
                {
                    // Access and invoke the CoverAllGround method using reflection
                    MethodInfo coverAllGroundMethod = typeof(GroundCoverController).GetMethod("CoverAllGround", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (coverAllGroundMethod != null)
                    {
                        coverAllGroundMethod.Invoke(groundCoverController, null);
                        MelonLogger.Msg("CoverAllGround method invoked.");
                    }
                    else
                    {
                        MelonLogger.Warning("Method 'CoverAllGround' not found.");
                    }
                }
                else
                {
                    MelonLogger.Warning("GroundCoverController component not found.");
                }
                MelonLogger.Msg(GetTileCount(cgc.GetComponent<Tilemap>()));
            }
            else
            {
                MelonLogger.Warning("Ground cover object is null.");
            }
            GameObject cgcm = GameObject.Find("Ground Cover Metal");
            if (cgcm != null)
            {
                GroundCoverController groundCoverController2 = cgcm.GetComponent<GroundCoverController>();
                if (groundCoverController2 != null)
                {
                    // Access and invoke the CoverAllGround method using reflection
                    MethodInfo coverAllGroundMethod = typeof(GroundCoverController).GetMethod("CoverAllGround", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (coverAllGroundMethod != null)
                    {
                        coverAllGroundMethod.Invoke(groundCoverController2, null);
                        MelonLogger.Msg("CoverAllGround method invoked.");
                    }
                    else
                    {
                        MelonLogger.Warning("Method 'CoverAllGround' not found.");
                    }
                }
                else
                {
                    MelonLogger.Warning("GroundCoverController component not found.");
                }
                MelonLogger.Msg(GetTileCount(cgcm.GetComponent<Tilemap>()));
            }
            else
            {
                MelonLogger.Warning("Ground cover object is null.");
            }
            cgc.GetComponent<TilemapCollider2D>().enabled = false;
            cgc.GetComponent<TilemapCollider2D>().enabled = true;
            cgc.GetComponent<CompositeCollider2D>().enabled = false;
            cgc.GetComponent <CompositeCollider2D>().enabled = true;
            cgcm.GetComponent<TilemapCollider2D>().enabled = false;
            cgcm.GetComponent<TilemapCollider2D>().enabled = true;
            cgcm.GetComponent<CompositeCollider2D>().enabled = false;
            cgcm.GetComponent<CompositeCollider2D>().enabled = true;*/
            GameObject p = GameObject.Find("Player B3");
            Debug.Log("After player objkect");
            Rigidbody2D prb = p.GetComponent<Rigidbody2D>();
            Debug.Log("After player rb");
            prb.gravityScale = 2;
            Debug.Log(prb.gravityScale);
        }
        public int GetTileCount(Tilemap tilemap)
        {
            int tileCount = 0;
            if (tilemap != null)
            {
                // Get the bounds of the tilemap
                BoundsInt bounds = tilemap.cellBounds;

                // Iterate through all positions within the bounds
                foreach (var pos in bounds.allPositionsWithin)
                {
                    if (tilemap.HasTile(pos))
                    {
                        tileCount++;
                    }
                }
            }
            else
            {
                Debug.LogWarning("Tilemap is null.");
            }
            return tileCount;
        }
        public void PrepMD()
        {
            GameObject tiwd = GameObject.Find("Tilemap Invisible Wall Death");
            tiwd.SetActive(false);
            GameObject spm = GameObject.Find("Sky Particles Moon");
            spm.SetActive(false);
            GameObject b = GameObject.Find("Bolt");
            b.SetActive(false);
            GameObject b1 = GameObject.Find("Bolt (1)");
            b1.SetActive(false);
            GameObject b2 = GameObject.Find("Bolt (2)");
            b2.SetActive(false);
            GameObject b3 = GameObject.Find("Bolt (3)");
            b3.SetActive(false);
            GameObject b4 = GameObject.Find("Bolt (4)");
            b4.SetActive(false);
            GameObject psa = GameObject.Find("Prop Sign Arrow");
            psa.SetActive(false);
            GameObject cc = GameObject.Find("Cloud Cannon");
            cc.SetActive(false);
            GameObject cc1 = GameObject.Find("Cloud Cannon (1)");
            cc1.SetActive(false);
            GameObject cc2 = GameObject.Find("Cloud Cannon (2)");
            cc2.SetActive(false);
            GameObject m = GameObject.Find("Metals");
            m.SetActive(false);
            GameObject c = GameObject.Find("Checkpoint");
            c.SetActive(false);
            GameObject c1 = GameObject.Find("Checkpoint (1)");
            c1.SetActive(false);
            GameObject c2 = GameObject.Find("Checkpoint (2)");
            c2.SetActive(false);
            GameObject c3 = GameObject.Find("Checkpoint (3)");
            c3.SetActive(false);
            Debug.Log("Before player");
            GameObject p = GameObject.Find("Player B3");
            Debug.Log("After player objkect");
            Rigidbody2D prb = p.GetComponent<Rigidbody2D>();
            Debug.Log("After player rb");
            prb.gravityScale = 2;
            Debug.Log(prb == null);
            Debug.Log(p == null);
            Debug.Log(prb.gravityScale);
            Debug.Log("After set GScale");
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                SceneManager.LoadScene("Moonlight District");
                GameObject p = GameObject.Find("Player B3");
                Debug.Log("After player objkect");
                Rigidbody2D prb = p.GetComponent<Rigidbody2D>();
                Debug.Log("After player rb");
                prb.gravityScale = 2;
                Debug.Log(prb.gravityScale);
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                MelonLogger.Msg("Key T pressed");
                GameObject p = GameObject.Find("Player B3");
                Debug.Log("After player objkect");
                Rigidbody2D prb = p.GetComponent<Rigidbody2D>();
                Debug.Log("After player rb");
                prb.gravityScale = 2;
                Debug.Log(prb.gravityScale);
                SetAll();
                GameObject pd = GameObject.Find("Death Collider");
                PlayerDeath pds = pd.GetComponent<PlayerDeath>();
                pds.DieSilent();
            }
        }
    }
}
