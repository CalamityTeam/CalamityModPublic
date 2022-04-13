using CalamityMod.NPCs.Astral;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

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
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.maxStack = 999;
            Item.consumable = true;
            Item.noUseGraphic = true;
            Item.value = Item.buyPrice(0, 0, 40, 0);
            //item.CloneDefaults(2004); //Lightning Bug item
            Item.width = 26;
            Item.height = 24;
            Item.bait = 40;
            Item.makeNPC = (short)ModContent.NPCType<Twinkler>();
            Item.rare = ItemRarityID.Green;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;
        }
    }
}
