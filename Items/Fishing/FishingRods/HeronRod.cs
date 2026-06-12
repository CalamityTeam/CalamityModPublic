using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing.FishingRods
{
    public class HeronRod : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Fishing";

        public static float FishingPowerBiomeMult = 1.1f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(FishingPowerBiomeMult.ToString());

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.useAnimation = 8;
            Item.useTime = 8;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.fishingPole = 25;
            Item.shootSpeed = 14.5f;
            Item.shoot = ModContent.ProjectileType<HeronBobber>();
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.rare = ItemRarityID.Orange;
        }

        public override void ModifyFishingLine(Projectile bobber, ref Vector2 lineOriginOffset, ref Color lineColor)
        {
            lineOriginOffset = new Vector2(47f, -33f);
            lineColor = new Color(101, 149, 154, 100);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AerialiteBar>(7).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
