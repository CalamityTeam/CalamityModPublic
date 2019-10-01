using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.CalamityCustomThrowingDamage
{
    public class UtensilPoker : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Utensil Poker");
			Tooltip.SetDefault("Fires random utensils in bursts of three\n" +
				"Grants Well Fed on enemy hits\n" +
				"Stealth strikes launch an additional butcher knife");
        }

        public override void SetDefaults()
        {
            item.width = 44;
            item.height = 66;
            item.damage = 750;
			item.GetGlobalItem<CalamityGlobalItem>(mod).rogue = true;
            item.knockBack = 8f;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useStyle = 1;
            item.useTime = 15;
			item.reuseDelay = 15;
            item.useAnimation = 45;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 12;
            item.shoot = mod.ProjectileType("Fork");
            item.shootSpeed = 16f;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			if (player.GetCalamityPlayer().StealthStrikeAvailable())
			{
				Projectile.NewProjectile(position.X, position.Y, (int) ((double)speedX * 1.2), (int) ((double)speedY * 1.2), mod.ProjectileType("ButcherKnife"),  (int) ((double)damage * 3), knockBack, Main.myPlayer);
				if (Main.rand.NextBool(3))
				{
					Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("Fork"), (int) ((double)damage * 1.1), knockBack * 2f, Main.myPlayer);
				}
				else if (Main.rand.NextBool(2))
				{
					Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("Knife"), (int)((double)damage * 1.2), knockBack, Main.myPlayer);
				}
				else
				{
					Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("CarvingFork"), damage, knockBack, Main.myPlayer);
				}
			}
			else if (Main.rand.NextBool(3))
			{
				Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("Fork"), (int) ((double)damage * 1.1), knockBack * 2f, Main.myPlayer);
			}
			else if (Main.rand.NextBool(2))
			{
				Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("Knife"), (int)((double)damage * 1.2), knockBack, Main.myPlayer);
			}
			else
			{
				Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("CarvingFork"), damage, knockBack, Main.myPlayer);
			}
			return false;
		}
    }
}
