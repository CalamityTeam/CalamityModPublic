using CalamityMod.Projectiles.Ranged;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class PrideHuntersPlanarRipper : ModItem
    {
		private int counter = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prideful Hunter's Planar Ripper");
            Tooltip.SetDefault("Converts musket balls into lightning bolts\n" +
			"Lightning bolts travel extremely fast and explode on enemy kills\n" +
			"Every fourth lightning bolt fired will deal 35 percent more damage.\n" +
			"Additionally, lightning bolt crits grant a stacking speed boost to the player\n" +
			"This stacks up to 20 percent bonus movement speed and acceleration\n" +
			"The boost will reset if the player holds a different item");
        }

        public override void SetDefaults()
        {
            item.damage = 82;
            item.ranged = true;
            item.width = 68;
            item.height = 32;
            item.useTime = 5;
            item.useAnimation = 5;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 1f;
			item.value = CalamityGlobalItem.Rarity11BuyPrice;
			item.rare = ItemRarityID.Purple;
            item.UseSound = SoundID.Item11;
            item.autoReuse = true;
            item.shoot = ProjectileID.Bullet;
            item.shootSpeed = 15f;
            item.useAmmo = AmmoID.Bullet;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-12, -6);
        }

        public override bool ConsumeAmmo(Player player)
        {
            if (Main.rand.Next(0, 100) < 33)
                return false;
            return true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (type == ProjectileID.Bullet)
            {
				counter++;
				float damageMult = 1f;
				if (counter == 4)
					damageMult = 1.35f;

                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<PlanarRipperBolt>(), (int)(damage * damageMult), knockBack, player.whoAmI);
				if (counter >= 4)
                counter = 0;
				return false;
            }
			return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddIngredient(ItemID.FragmentVortex, 10);
            recipe.AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 6);
            recipe.AddIngredient(ModContent.ItemType<CoreofCalamity>(), 3);
            recipe.AddIngredient(ModContent.ItemType<Stardust>(), 25);
            recipe.AddIngredient(ModContent.ItemType<P90>());
            recipe.AddIngredient(ItemID.Uzi);
            recipe.AddIngredient(ModContent.ItemType<Aeries>());
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
