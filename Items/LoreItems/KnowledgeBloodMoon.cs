using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class KnowledgeBloodMoon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Red Moon");
            Tooltip.SetDefault("We long ago feared the light of the red moon.\n" +
                "Many went mad, others died, but a scant few became blessed with a wealth of cosmic understanding.");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = 9;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }
    }
}
