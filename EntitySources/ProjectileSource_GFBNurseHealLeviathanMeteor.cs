#nullable enable
using Terraria;
using Terraria.DataStructures;

namespace CalamityMod.EntitySources
{
    public class ProjectileSource_GFBNurseHealLeviathanMeteor : IEntitySource
    {
        public Player player;
        public ProjectileSource_GFBNurseHealLeviathanMeteor(Player p) => player = p;
        public string? Context => null;
    }
}
