using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    [SerializeField] private World world;
    [SerializeField] private Player player;

    public void Start() {
        world.DStart(player);
    }

    public World World {
        get { return world; }
    }

    public GameObject Tower {
        get { return world.Tower; }
    }

}
