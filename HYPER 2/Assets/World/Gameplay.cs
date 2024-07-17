using UnityEngine;

public class Gameplay : MonoBehaviour
{
    private static Gameplay self;

    public static float linearT = 1f;
    public const int unlock3rdPath = 10;
    public const int unlock4thPath = 20;

    [SerializeField] private Interval ufoSpawnInterval;

    private Manager manager;
    private World world;
    private Player player;

    public void IntervalUFOSpawn() {
        var range = world.WG.LayerRange(world.WG.LastLayer);

        world.InstantiateEnemy(Resources.Enemy.UFO, world.WG.Tiles[range[Seed.IntRange(0, range.Count-1)]].transform.position+Vector3.up*10f, (r, e) => {
            e.CStart(manager, r);
        });
    }

    public void DStart(Player player, Manager manager) {
        self = this;
        this.manager = manager;
        world = manager.World;

        if (Save.rData.Lvl >= Gameplay.unlock3rdPath) {
            world.WG.CreateNextPath();
        }
        if (Save.rData.Lvl >= Gameplay.unlock4thPath) {
            world.WG.CreateNextPath();
        }
        if (Save.rData.Lvl >= 2) {
            ufoSpawnInterval.Enable();
        }

        player.AddLvlUpListener(lvl => {
            switch (lvl) {
                case unlock3rdPath:
                    world.WG.CreateNextPath();
                    break;
                case unlock4thPath:
                    world.WG.CreateNextPath();
                    break;
                case 2:
                    ufoSpawnInterval.Enable();
                    break;
            }
        });
    }
}
