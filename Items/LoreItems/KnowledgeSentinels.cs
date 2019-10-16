using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class KnowledgeSentinels : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Sentinels of the Devourer");
            Tooltip.SetDefault("Signus. The Void. The Weaver.\n" +
                "Each represent one of the Devourer’s largest spheres of influence.\n" +
                "Dispatching them has most likely invoked its anger and marked you as a target for destruction.");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = 10;
            item.consumable = false;
            item.Calamity().postMoonLordRarity = 13;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }
    }
}
