#nullable enable
using Terraria;
using Terraria.DataStructures;

namespace CalamityMod.EntitySources
{
    public class ProjectileSource_LevelUp : IEntitySource
    {
        public Player player;
        public ProjectileSource_LevelUp(Player p) => player = p;
        public string? Context => null;
    }
}
