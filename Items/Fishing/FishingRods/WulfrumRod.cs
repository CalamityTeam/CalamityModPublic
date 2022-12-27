using CalamityMod.Projectiles.Typeless;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;

namespace CalamityMod.Items.Fishing.FishingRods
{
    public class WulfrumRod : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Wulfrum Fishing Pole");
            Tooltip.SetDefault("This barely works, but it's better than nothing");
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.useAnimation = 8;
            Item.useTime = 8;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.rare = ItemRarityID.Blue;
            Item.fishingPole = 10;
            Item.shootSpeed = 10f;
            Item.shoot = ModContent.ProjectileType<WulfrumBobber>();
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<WulfrumMetalScrap>(6).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
