using CalamityMod.NPCs.SunkenSea;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Critters
{
    public class BabyGhostBellItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 5;
            DisplayName.SetDefault("Baby Ghost Bell");
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
            Item.value = Item.buyPrice(0, 0, 30, 0);
            Item.width = 26;
            Item.height = 24;
            Item.bait = 20;
            Item.makeNPC = (short)ModContent.NPCType<BabyGhostBell>();
            Item.rare = ItemRarityID.Green;
        }
    }
}
