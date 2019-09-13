using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
    public class BurntSienna : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Burnt Sienna");
			Tooltip.SetDefault("Causes enemies to erupt into healing projectiles on death");
		}

		public override void SetDefaults()
		{
			item.width = 42;
			item.damage = 35;
			item.melee = true;
			item.useAnimation = 21;
			item.useTime = 21;
			item.useTurn = true;
			item.useStyle = 1;
			item.knockBack = 5.5f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 54;
			item.value = Item.buyPrice(0, 4, 0, 0);
			item.rare = 3;
			item.shootSpeed = 5f;
		}

		public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
		{
			if (target.life <= 0)
			{
				float randomSpeedX = (float)Main.rand.Next(3);
				float randomSpeedY = (float)Main.rand.Next(3, 5);
				Projectile.NewProjectile(target.Center.X, target.Center.Y, -randomSpeedX, -randomSpeedY, mod.ProjectileType("BurntSienna"), 0, 0f, player.whoAmI);
				Projectile.NewProjectile(target.Center.X, target.Center.Y, randomSpeedX, -randomSpeedY, mod.ProjectileType("BurntSienna"), 0, 0f, player.whoAmI);
				Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, -randomSpeedY, mod.ProjectileType("BurntSienna"), 0, 0f, player.whoAmI);
			}
		}

		public override void MeleeEffects(Player player, Rectangle hitbox)
		{
			if (Main.rand.Next(5) == 0)
			{
				int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 246);
			}
		}
	}
}
