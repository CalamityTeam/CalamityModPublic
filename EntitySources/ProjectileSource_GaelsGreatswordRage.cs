using Terraria;
using Terraria.DataStructures;

namespace CalamityMod.EntitySources
{
    public class ProjectileSource_GaelsGreatswordRage : IEntitySource
    {
        public Player player;
        public ProjectileSource_GaelsGreatswordRage(Player p) => player = p;
    }
}
