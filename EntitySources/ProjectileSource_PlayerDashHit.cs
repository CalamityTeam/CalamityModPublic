#nullable enable
using Terraria;
using Terraria.DataStructures;

namespace CalamityMod.EntitySources
{
    public class ProjectileSource_PlayerDashHit : IEntitySource
    {
        public Player player;
        public ProjectileSource_PlayerDashHit(Player p) => player = p;
        public string? Context => null;
    }
}
