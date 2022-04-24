#nullable enable
using Terraria;
using Terraria.DataStructures;

namespace CalamityMod.EntitySources
{
    public class ProjectileSource_BloodflareSummonSet : IEntitySource
    {
        public Player player;
        public ProjectileSource_BloodflareSummonSet(Player p) => player = p;
        public string? Context => null;
    }
}
