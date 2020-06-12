using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;
namespace CalamityMod.Items.Fishing.FishingRods
{
	public class TheDevourerofCods : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Devourer of Cods");
            Tooltip.SetDefault("Fires ten lines at once. Line never snaps and can fish from lava.\n" +
				"The devourer was once just an Eater of Shoals.");
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
			item.fishingPole = 75;
			item.shootSpeed = 20f;
			item.shoot = ModContent.ProjectileType<DevourerofCodsBobber>();
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = 10;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int index = 0; index < 10; ++index)
            {
                float SpeedX = speedX + (float)Main.rand.Next(-75, 76) * 0.05f;
                float SpeedY = speedY + (float)Main.rand.Next(-75, 76) * 0.05f;
                int line = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, 0, 0f, player.whoAmI, 0.0f, 0.0f);
                // Protection against projectile cap
				if (Main.rand.NextBool() && line < Main.maxProjectiles) // randomizing line color
					Main.projectile[line].Calamity().lineColor = 1;
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 12);
            recipe.AddRecipeGroup("NForEE", 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
