#nullable enable
using Terraria;
using Terraria.DataStructures;

namespace CalamityMod.EntitySources
{
    public class ProjectileSource_HydrothermalSmoke : IEntitySource
    {
        public Player player;
        public ProjectileSource_HydrothermalSmoke(Player p) => player = p;
        public string? Context => null;
    }
}
