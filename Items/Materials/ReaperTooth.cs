using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class ReaperTooth : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 5;
            DisplayName.SetDefault("Reaper Tooth");
            Tooltip.SetDefault("Sharp enough to cut diamonds");
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.maxStack = 999;
            Item.value = Item.buyPrice(0, 7, 0, 0);
            Item.rare = ModContent.RarityType<PureGreen>();
        }
    }
}
