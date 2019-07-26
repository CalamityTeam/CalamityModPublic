using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.Polterghast
{
	public class FatesReveal : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Fate's Reveal");
			Tooltip.SetDefault("Spawns ghostly fireballs that follow the player");
			Item.staff[item.type] = true;
		}

	    public override void SetDefaults()
	    {
	        item.damage = 60;
	        item.magic = true;
	        item.mana = 20;
	        item.width = 68;
	        item.height = 72;
	        item.useTime = 16;
	        item.useAnimation = 16;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 5.5f;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item20;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("FatesReveal");
	        item.shootSpeed = 1f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 13;
		}

	    public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
	    {
            Vector2 vector = player.RotatedRelativePoint(player.MountedCenter, true);
            float num78 = (float)Main.mouseX + Main.screenPosition.X - vector.X;
            float num79 = (float)Main.mouseY + Main.screenPosition.Y - vector.Y;
            if (player.gravDir == -1f)
            {
                num79 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector.Y;
            }
            float num80 = (float)Math.Sqrt((double)(num78 * num78 + num79 * num79));
            float num81 = num80;
            if ((float.IsNaN(num78) && float.IsNaN(num79)) || (num78 == 0f && num79 == 0f))
            {
                num78 = (float)player.direction;
                num79 = 0f;
                num80 = item.shootSpeed;
            }
            else
            {
                num80 = item.shootSpeed / num80;
            }
            vector += new Vector2(num78, num79);
            int num107 = 5;
			for (int num108 = 0; num108 < num107; num108++)
			{
				vector.X = vector.X + (float)Main.rand.Next(-100, 101);
				vector.Y += (float)(Main.rand.Next(-25, 26) * num108);
                float spawnX = vector.X;
                float spawnY = vector.Y;
				Projectile.NewProjectile(spawnX, spawnY, 0f, 0f, type, damage, knockBack, player.whoAmI, 0f, (float)Main.rand.Next(3));
			}
	    	return false;
		}
	}
}
