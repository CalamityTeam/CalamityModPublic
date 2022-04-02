using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
	public class Alluvion : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Alluvion");
            Tooltip.SetDefault("Converts wooden arrows into sharks, torrential and typhoon arrows\n" +
                       "Fires a torrent of six arrows at once");
        }

        public override void SetDefaults()
        {
            item.damage = 165;
            item.ranged = true;
            item.width = 62;
            item.height = 90;
            item.useTime = 15;
            item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4f;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = ItemRarityID.Purple;
            item.UseSound = SoundID.Item5;
            item.autoReuse = true;
            item.shoot = ProjectileID.WoodenArrowFriendly;
            item.shootSpeed = 12f;
            item.useAmmo = AmmoID.Arrow;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
			item.Calamity().canFirePointBlankShots = true;
		}

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 source = player.RotatedRelativePoint(player.MountedCenter);
            float num117 = MathHelper.Pi * 0.1f;
            int totalProjectiles = 6;
            Vector2 velocity = new Vector2(speedX, speedY);
            velocity.Normalize();
            velocity *= 35f;
            bool canHit = Collision.CanHit(source, 0, 0, source + velocity, 0, 0);
            for (int i = 0; i < totalProjectiles; i++)
            {
                float num120 = i - (totalProjectiles - 1f) / 2f;
                Vector2 offset = velocity.RotatedBy(num117 * num120);
                if (!canHit)
                    offset -= velocity;

                if (type == ProjectileID.WoodenArrowFriendly)
                {
					int newType = type;
					switch (i)
					{
						case 0:
						case 5:
							newType = ModContent.ProjectileType<TyphoonArrow>();
							break;
						case 1:
						case 4:
							newType = ModContent.ProjectileType<MiniSharkron>();
							break;
						case 2:
						case 3:
							newType = ModContent.ProjectileType<TorrentialArrow>();
							break;
					}
                    int proj = Projectile.NewProjectile(source.X + offset.X, source.Y + offset.Y, speedX, speedY, newType, damage, knockBack, player.whoAmI);
					if (proj.WithinBounds(Main.maxProjectiles))
					{
						Main.projectile[proj].arrow = true;
						Main.projectile[proj].extraUpdates += 1;
					}
                }
                else
                {
                    int proj = Projectile.NewProjectile(source.X + offset.X, source.Y + offset.Y, speedX, speedY, type, damage, knockBack, player.whoAmI);
                    Main.projectile[proj].noDropItem = true;
                }
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Monsoon>());
            recipe.AddIngredient(ModContent.ItemType<SeaPrism>(), 20);
            recipe.AddIngredient(ModContent.ItemType<DepthCells>(), 10);
            recipe.AddIngredient(ModContent.ItemType<Lumenite>(), 20);
            recipe.AddIngredient(ModContent.ItemType<Tenebris>(), 5);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 8);
            recipe.AddIngredient(ModContent.ItemType<EndothermicEnergy>(), 20);
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
