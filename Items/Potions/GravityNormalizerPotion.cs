using CalamityMod.Buffs.Potions;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Fishing.AstralCatches;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions
{
    public class GravityNormalizerPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gravity Normalizer Potion");
            Tooltip.SetDefault("Disables the low gravity of space and grants immunity to the distorted debuff");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.useTurn = true;
            Item.maxStack = 30;
            Item.rare = ItemRarityID.Lime;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.UseSound = SoundID.Item3;
            Item.consumable = true;
            Item.buffType = ModContent.BuffType<GravityNormalizerBuff>();
            Item.buffTime = CalamityUtils.SecondsToFrames(480f);
            Item.value = Item.buyPrice(0, 2, 0, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.GravitationPotion).AddIngredient(ModContent.ItemType<AstralJelly>()).AddIngredient(ModContent.ItemType<AldebaranAlewife>()).AddTile(TileID.AlchemyTable).Register();
            CreateRecipe(1).AddIngredient(ItemID.BottledWater).AddIngredient(ModContent.ItemType<BloodOrb>(), 10).AddIngredient(ModContent.ItemType<AstralJelly>()).AddTile(TileID.AlchemyTable).Register();
        }
    }
}
