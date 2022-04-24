#nullable enable
using Terraria;
using Terraria.DataStructures;

namespace CalamityMod.EntitySources
{
    public class ProjectileSource_OmegaBlueTentacles : IEntitySource
    {
        public Player player;
        public ProjectileSource_OmegaBlueTentacles(Player p) => player = p;
        public string? Context => null;
    }
}
