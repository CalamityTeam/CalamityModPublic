#nullable enable
using Terraria;
using Terraria.DataStructures;

namespace CalamityMod.EntitySources
{
    public class ProjectileSource_TarragonSummonAura : IEntitySource
    {
        public Player player;
        public ProjectileSource_TarragonSummonAura(Player p) => player = p;
        public string? Context => null;
    }
}
