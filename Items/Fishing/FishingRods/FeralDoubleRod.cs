using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;
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
			//item.CloneDefaults(2289); //Wooden Fishing Pole
			item.width = 24;
			item.height = 28;
			item.useAnimation = 8;
			item.useTime = 8;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;
			item.fishingPole = 40;
			item.shootSpeed = 16f;
			item.shoot = ModContent.ProjectileType<FeralDoubleBobber>();
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = 7;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int index = 0; index < 2; ++index)
            {
                float SpeedX = speedX + (float)Main.rand.Next(-75, 76) * 0.05f;
                float SpeedY = speedY + (float)Main.rand.Next(-75, 76) * 0.05f;
                Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<FeralDoubleBobber>(), 0, 0f, player.whoAmI, 0.0f, 0.0f);
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<DraedonBar>(), 10);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
