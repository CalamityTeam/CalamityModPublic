using CalamityMod.NPCs.Astral;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Critters
{
    public class TwinklerItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Twinkler");
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
            item.value = Item.buyPrice(0, 0, 40, 0);
            //item.CloneDefaults(2004); //Lightning Bug item
            item.width = 26;
            item.height = 24;
            item.bait = 40;
            item.makeNPC = (short)ModContent.NPCType<Twinkler>();
            item.rare = ItemRarityID.Green;
        }
    }
}
