using CalamityMod.Buffs.Potions;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions
{
    [LegacyName("HolyWrathPotion")]
    public class FlaskOfHolyFlames : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 20;
            DisplayName.SetDefault("Flask of Holy Flames");
            Tooltip.SetDefault("Melee, Whip, and Rogue attacks inflict Holy Flames");
        }

        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 36;
            Item.useTurn = true;
            Item.maxStack = 30;
            Item.rare = ItemRarityID.Purple;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.UseSound = SoundID.Item3;
            Item.consumable = true;
            Item.buffType = ModContent.BuffType<WeaponImbueHolyFlames>();
            Item.buffTime = CalamityUtils.SecondsToFrames(1200f);
            Item.value = Item.buyPrice(0, 2, 0, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.BottledWater).
                AddIngredient<UnholyEssence>().
                AddTile(TileID.ImbuingStation).
                Register();
        }
    }
}
