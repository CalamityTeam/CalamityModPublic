using Terraria;
using Terraria.DataStructures;

namespace CalamityMod.EntitySources
{
    public class ProjectileSource_Death : IEntitySource
    {
        public Player player;
        public ProjectileSource_Death(Player p) => player = p;
    }
}
