#nullable enable
using Terraria;
using Terraria.DataStructures;

namespace CalamityMod.EntitySources
{
    public class ProjectileSource_PlaguebringerSetBoost : IEntitySource
    {
        public Player player;
        public ProjectileSource_PlaguebringerSetBoost(Player p) => player = p;
        public string? Context => null;
    }
}
