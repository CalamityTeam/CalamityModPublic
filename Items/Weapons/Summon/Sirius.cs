using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Items.Placeables.Ores;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class Sirius : ModItem
    {
		int siriusSlots;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sirius");
            Tooltip.SetDefault("Summons the brightest star in the night sky to shine upon your foes\n" +
                               "Consumes all of the remaining minion slots on use\n" +
                               "Increased power based on the number of minion slots used");
        }

        public override void SetDefaults()
        {
            item.width = 62;
            item.height = 62;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.UseSound = SoundID.Item44;

            item.summon = true;
            item.mana = 60;
            item.damage = 275;
            item.knockBack = 3f;
            item.useTime = item.useAnimation = 10;
            item.shoot = ModContent.ProjectileType<SiriusMinion>();
            item.shootSpeed = 10f;

            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

		public override void HoldItem(Player player)
        {
			double minionCount = 0;
			for (int j = 0; j < Main.projectile.Length; j++)
			{
                Projectile projectile = Main.projectile[j];
				if (projectile.active && projectile.owner == player.whoAmI && projectile.minion && projectile.type != ModContent.ProjectileType<SiriusMinion>() && projectile.type != ModContent.ProjectileType<RadiantResolutionAura>())
				{
					minionCount += projectile.minionSlots;
				}
			}
			siriusSlots = (int)((double)player.maxMinions - minionCount);
		}

        public override bool CanUseItem(Player player)
		{
			return siriusSlots >= 1;
		}

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int x = 0; x < Main.projectile.Length; x++)
            {
                Projectile projectile2 = Main.projectile[x];
                if (projectile2.active && projectile2.owner == player.whoAmI && projectile2.type == ModContent.ProjectileType<SiriusMinion>() && projectile2.type != ModContent.ProjectileType<RadiantResolutionAura>())
                {
                    projectile2.Kill();
                }
            }
            position = Main.MouseWorld;
            speedX = 0;
            speedY = 0;
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, siriusSlots);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Lumenite>(), 5);
            recipe.AddIngredient(ModContent.ItemType<RuinousSoul>(), 2);
            recipe.AddIngredient(ModContent.ItemType<ExodiumClusterOre>(), 12);
            recipe.AddIngredient(ModContent.ItemType<SunGodStaff>());
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
