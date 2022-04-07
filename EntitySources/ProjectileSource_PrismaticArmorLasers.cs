using Terraria;
using Terraria.DataStructures;

namespace CalamityMod.EntitySources
{
    public class ProjectileSource_PrismaticArmorLasers : IEntitySource
    {
        public Player player;
        public ProjectileSource_PrismaticArmorLasers(Player p) => player = p;
    }
}
