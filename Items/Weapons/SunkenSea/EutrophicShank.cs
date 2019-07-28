using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.SunkenSea
{
    public class EutrophicShank : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Eutrophic Shank");
			Tooltip.SetDefault("Shoots electric sparks");
		}

		public override void SetDefaults()
		{
			item.useStyle = 3;
			item.useTurn = false;
			item.useAnimation = 14;
			item.useTime = 14;
			item.width = 42;
			item.height = 38;
			item.damage = 24;
			item.melee = true;
			item.knockBack = 4f;
			item.UseSound = SoundID.Item1;
			item.useTurn = true;
			item.autoReuse = true;
			item.value = Item.buyPrice(0, 2, 0, 0);
			item.rare = 2;
			item.shoot = mod.ProjectileType("EutrophicSpark");
			item.shootSpeed = 3f;
		}

	    public override void MeleeEffects(Player player, Rectangle hitbox)
	    {
	        if (Main.rand.Next(3) == 0)
	        {
	        	int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 206, 0f, 0f, 100, default(Color), 1f);
	        }
	    }

		public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
	    {
	    	int num6 = Main.rand.Next(1, 2);
			for (int index = 0; index < num6; ++index)
			{
			    float SpeedX = speedX + (float) Main.rand.Next(-60, 61) * 0.05f;
			    float SpeedY = speedY + (float) Main.rand.Next(-60, 61) * 0.05f;
			    int projectile = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, (int)((double)damage * 0.4), knockBack, player.whoAmI, 0.0f, 0.0f);
			}
			return false;
		}
	}
}
