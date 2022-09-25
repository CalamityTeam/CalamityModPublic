using CalamityMod.NPCs.DraedonLabThings;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Critters
{
    public class NanodroidDisfunctionalItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 5;
            DisplayName.SetDefault("Disfunctional Nanodroid");
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.maxStack = 999;
            Item.consumable = true;
            Item.noUseGraphic = true;
            Item.value = Item.buyPrice(0, 0, 30);
            Item.width = 16;
            Item.height = 10;
            Item.makeNPC = (short)ModContent.NPCType<NanodroidDisfunctional>();
            Item.rare = ModContent.RarityType<DarkOrange>();
        }
    }
}
