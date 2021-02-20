using CalamityMod.NPCs.AcidRain;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Critters
{
    public class BabyFlakHermit : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Baby Flak Crab");
        }

        public override void SetDefaults()
        {
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.autoReuse = true;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.maxStack = 999;
            item.consumable = true;
            item.noUseGraphic = true;
            item.value = Item.buyPrice(0, 0, 30, 0);
            //item.CloneDefaults(2004); //Lightning Bug item
            item.width = 26;
            item.height = 24;
            item.makeNPC = (short)ModContent.NPCType<FlakBaby>();
            item.rare = ItemRarityID.LightPurple;
        }
    }
}
