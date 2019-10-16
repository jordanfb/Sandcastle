﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitScreenRects : MonoBehaviour
{

    public Camera defaultCamera;
    public GameObject cameraPrefab;
    public List<Transform> players = new List<Transform>();
    // we can add and remove from this and it'll create scripts to follow that

    public List<Camera> cameras = new List<Camera>();
    
    public void AddPlayer(Transform player)
    {
        if (players.Contains(player))
        {
            return; // don't add it it's already there
        }
        players.Add(player);
        UpdatePlayerCameras();
    }

    public bool RemovePlayer(Transform player)
    {
        bool found = players.Remove(player);
        UpdatePlayerCameras();
        return found;
    }

    public void UpdatePlayerCameras()
    {
        // call this when we add or remove a player to refresh the cameras
        if (players.Count == 0)
        {
            // have a global fly camera? Just have a default camera in scene that we activate
            defaultCamera.gameObject.SetActive(true);
            defaultCamera.rect = new Rect(0, 0, 1, 1);
        }
        else
        {
            defaultCamera.gameObject.SetActive(false);
            // activate other cameras
            for (int i = 0; i < players.Count; i++)
            {
                Camera c;
                if (cameras.Count <= i)
                {
                    // create a camera for us!
                    c = Instantiate(cameraPrefab).GetComponent<Camera>();
                    cameras.Add(c);
                }
                else
                {
                    c = cameras[i];
                    c.gameObject.SetActive(true);
                }
                // now make c follow whoever it's following
                c.gameObject.GetComponent<PlayerFollowScript>().toFollow = players[i].transform;
                // now calculate the rect
                Rect cRect = new Rect(0, 0, 1, 1);
                if (players.Count > 1 && players.Count < 5)
                {
                    if (i % 2 == 0)
                    {
                        // left cameras
                        cRect.width = .5f;
                    }
                    else if (i % 2 == 1)
                    {
                        // right cameras
                        cRect.x = .5f;
                        cRect.width = .5f;
                    }
                    if (players.Count > 2)
                    {
                        // then move them vertically
                        cRect.height = .5f;
                        if (i < 2)
                        {
                            cRect.y = .5f;
                        }
                    }
                } else if (players.Count > 4)
                {
                    Debug.Log("Error unable to handle the current number of players");
                }
                c.rect = cRect;
            }

            for (int i = players.Count; i < cameras.Count; i++)
            {
                cameras[i].gameObject.SetActive(false); // disable it
            }

            // special fly camera!
            if (players.Count == 3)
            {
                defaultCamera.gameObject.SetActive(true);
                defaultCamera.rect = new Rect(.5f, 0, .5f, .5f);
            }
        }
    }
}