using CalamityMod.NPCs.NormalNPCs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Critters
{
    public class PiggyItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 5;
            DisplayName.SetDefault("Piggy");
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
            Item.width = 26;
            Item.height = 24;
            Item.makeNPC = (short)ModContent.NPCType<Piggy>();

            Item.value = Item.sellPrice(gold: 10);
            Item.rare = ItemRarityID.Blue;
            Item.Calamity().donorItem = true;
        }
    }
}
