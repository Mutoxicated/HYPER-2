using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{   
    [SerializeField] private PublicPools pps;
    [SerializeField] private Resources resources;
    [SerializeField] private World world;
    [SerializeField] private Player player;

    public void Awake() {
        pps.DStart();
        resources.DStart();
        player.DStart(this);
        world.DStart(player);
    }

    public World World {
        get { return world; }
    }

    public GameObject Tower {
        get { return world.Tower; }
    }

    public GameObject PlayerGo {
        get { return player.gameObject; }
    }

    public PublicPools Pools {
        get { return pps; }
    }
}
