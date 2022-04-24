#nullable enable
using Terraria;
using Terraria.DataStructures;

namespace CalamityMod.EntitySources
{
    public class ProjectileSource_InfernoPotionBoost : IEntitySource
    {
        public Player player;
        public ProjectileSource_InfernoPotionBoost(Player p) => player = p;
        public string? Context => null;
    }
}
