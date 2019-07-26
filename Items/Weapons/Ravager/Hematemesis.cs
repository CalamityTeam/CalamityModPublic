using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.Ravager
{
	public class Hematemesis : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Hematemesis");
			Tooltip.SetDefault("Casts a barrage of blood geysers from below you");
			Item.staff[item.type] = true;
		}

	    public override void SetDefaults()
	    {
	        item.damage = 125;
	        item.magic = true;
	        item.mana = 14;
            item.rare = 8;
	        item.width = 48;
	        item.height = 54;
	        item.useTime = 20;
	        item.useAnimation = 20;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 3.75f;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.UseSound = SoundID.Item21;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("BloodBlast");
	        item.shootSpeed = 10f;
	    }

	    public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
	    {
            for (int x = 0; x < 10; x++)
            {
                Projectile.NewProjectile(player.position.X + (float)Main.rand.Next(-150, 150), player.position.Y + 600f, 0f, -10f, type, damage, knockBack, Main.myPlayer, 0f, 0f);
            }
            return false;
		}
	}
}
