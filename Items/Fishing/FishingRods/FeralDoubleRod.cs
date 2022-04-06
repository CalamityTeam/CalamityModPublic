using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;
using Terraria.DataStructures;

namespace CalamityMod.Items.Fishing.FishingRods
{
    public class FeralDoubleRod : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Feral Double Rod");
            Tooltip.SetDefault("Fires two lines at once.\n" +
                "Just as you have tamed the jungle monster, you can now tame the fish in the sea.");
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.useAnimation = 8;
            Item.useTime = 8;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.fishingPole = 40;
            Item.shootSpeed = 16f;
            Item.shoot = ModContent.ProjectileType<FeralDoubleBobber>();
            Item.value = Item.buyPrice(0, 60, 0, 0);
            Item.rare = ItemRarityID.Lime;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int index = 0; index < 2; ++index)
            {
                float SpeedX = velocity.X + Main.rand.NextFloat(-3.75f, 3.75f);
                float SpeedY = velocity.Y + Main.rand.NextFloat(-3.75f, 3.75f);
                Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, type, 0, 0f, player.whoAmI);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DraedonBar>(10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
