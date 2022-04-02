using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Dyes.HairDye
{
    public class RageHairDye : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rage Hair Dye");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 26;
            item.maxStack = 99;
            item.value = Item.buyPrice(gold: 7, silver: 50);
            item.rare = ItemRarityID.Green;
            item.UseSound = SoundID.Item3;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.useTurn = true;
            item.useAnimation = 17;
            item.useTime = 17;
            item.consumable = true;
        }
    }
}
