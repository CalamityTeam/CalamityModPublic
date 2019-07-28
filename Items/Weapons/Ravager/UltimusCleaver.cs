using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ravager
{
    public class UltimusCleaver : ModItem
	{

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ultimus Cleaver");
			Tooltip.SetDefault("Launches damaging sparks when the player walks on the ground with this weapon out");
		}

		public override void SetDefaults()
		{
			item.damage = 300;
			item.melee = true;
			item.rare = 8;
			item.width = 82;
			item.height = 102;
			item.useTime = 20;
			item.useAnimation = 20;
			item.useStyle = 5;
			item.knockBack = 5f;
			item.value = Item.buyPrice(0, 80, 0, 0);
			item.autoReuse = true;
			item.shoot = mod.ProjectileType("UltimusCleaverDust");
			item.shootSpeed = 10f;
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-10, -10);
		}

		public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			if (Collision.SolidCollision(position, player.width, player.height) && player.velocity.X != 0)
			{
				for (int i = 0; i < 5; i++)
				{
					float posX;
					float velocityX;
					if (player.direction == 1)
					{
						posX = (float)Main.rand.Next(10, 60);
						velocityX = (float)Main.rand.Next(2, 10);
					}
					else
					{
						posX = (float)Main.rand.Next(-60, -10);
						velocityX = (float)Main.rand.Next(-10, -2);
					}
					Projectile.NewProjectile((player.Center.X + posX), (player.Center.Y + 20), velocityX, (float)Main.rand.Next(-7, -3), mod.ProjectileType("UltimusCleaverDust"), (int)((float)item.damage * 0.4f * player.meleeDamage), 0f, Main.myPlayer);
				}
			}
			return false;
		}
	}
}
