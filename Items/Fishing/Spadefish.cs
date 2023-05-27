using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing
{
    public class Spadefish : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 15;
            Item.knockBack = 2f;
            Item.useTime = 7;
            Item.useAnimation = 20;
            Item.pick = 34;

            Item.DamageType = DamageClass.Melee;
            Item.width = 46;
            Item.height = 44;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = CalamityGlobalItem.Rarity2BuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }
    }
}
