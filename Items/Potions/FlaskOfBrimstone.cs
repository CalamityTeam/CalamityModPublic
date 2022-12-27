using CalamityMod.Buffs.Potions;
using CalamityMod.Items.Fishing.BrimstoneCragCatches;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions
{
    [LegacyName("CalamitasBrew")]
    public class FlaskOfBrimstone : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 20;
            DisplayName.SetDefault("Flask of Brimstone");
            Tooltip.SetDefault("Melee, Whip, and Rogue attacks inflict Brimstone Flames");
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 34;
            Item.useTurn = true;
            Item.maxStack = 30;
            Item.rare = ItemRarityID.LightRed;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.UseSound = SoundID.Item3;
            Item.consumable = true;
            Item.buffType = ModContent.BuffType<AbyssalWeapon>();
            Item.buffTime = CalamityUtils.SecondsToFrames(1200f);
            Item.value = Item.buyPrice(0, 2, 0, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.BottledWater).
                AddIngredient<AshesofCalamity>().
                AddTile(TileID.ImbuingStation).
                Register();
        }
    }
}
