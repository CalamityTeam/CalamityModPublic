#nullable enable
using Terraria;
using Terraria.DataStructures;

namespace CalamityMod.EntitySources
{
    public class ProjectileSource_TeslaPotion : IEntitySource
    {
        public Player player;
        public ProjectileSource_TeslaPotion(Player p) => player = p;
        public string? Context => null;
    }
}
