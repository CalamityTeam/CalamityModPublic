#nullable enable
using Terraria;
using Terraria.DataStructures;

namespace CalamityMod.EntitySources
{
    public class ProjectileSource_DemonshadeSet : IEntitySource
    {
        public Player player;
        public ProjectileSource_DemonshadeSet(Player p) => player = p;
        public string? Context => null;
    }
}
