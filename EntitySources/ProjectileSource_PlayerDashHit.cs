#nullable enable
using Terraria;
using Terraria.DataStructures;

namespace CalamityMod.EntitySources
{
    // Ozzatron 10JUL2023: couldn't figure out a clever way to remove this one, because it's used for some esoteric function
    public class ProjectileSource_PlayerDashHit : IEntitySource
    {
        public Player player;
        public ProjectileSource_PlayerDashHit(Player p) => player = p;
        public string? Context => null;
    }
}
