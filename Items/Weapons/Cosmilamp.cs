using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
    public class Cosmilamp : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cosmilamp");
			Tooltip.SetDefault("Summons a cosmic lantern to fight for you\n" +
                "Takes up 2 minion slots");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 180;
	        item.mana = 10;
	        item.width = 42;
	        item.height = 60;
	        item.useTime = 36;
	        item.useAnimation = 36;
	        item.useStyle = 1;
	        item.noMelee = true;
	        item.knockBack = 4f;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item44;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("Cosmilamp");
	        item.shootSpeed = 10f;
	        item.summon = true;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 13;
		}

	    public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
	    {
			float num72 = item.shootSpeed;
	    	Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
	    	float num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
			float num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
			if (player.gravDir == -1f)
			{
				num79 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector2.Y;
			}
			float num80 = (float)Math.Sqrt((double)(num78 * num78 + num79 * num79));
			float num81 = num80;
			if ((float.IsNaN(num78) && float.IsNaN(num79)) || (num78 == 0f && num79 == 0f))
			{
				num78 = (float)player.direction;
				num79 = 0f;
				num80 = num72;
			}
			else
			{
				num80 = num72 / num80;
			}
	    	num78 = 0f;
			num79 = 0f;
			vector2.X = (float)Main.mouseX + Main.screenPosition.X;
			vector2.Y = (float)Main.mouseY + Main.screenPosition.Y;
			Projectile.NewProjectile(vector2.X, vector2.Y, num78, num79, mod.ProjectileType("Cosmilamp"), damage, knockBack, player.whoAmI, 0f, 0f);
			return false;
	    }
	}
}
