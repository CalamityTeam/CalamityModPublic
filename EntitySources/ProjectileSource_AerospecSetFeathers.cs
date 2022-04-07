using Terraria;
using Terraria.DataStructures;

namespace CalamityMod.EntitySources
{
    public class ProjectileSource_AerospecSetFeathers : IEntitySource
    {
        public Player player;
        public ProjectileSource_AerospecSetFeathers(Player p) => player = p;
    }
}
