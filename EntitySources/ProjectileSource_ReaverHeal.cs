using Terraria;
using Terraria.DataStructures;

namespace CalamityMod.EntitySources
{
    public class ProjectileSource_ReaverHeal : IEntitySource
    {
        public Player player;
        public ProjectileSource_ReaverHeal(Player p) => player = p;
    }
}
