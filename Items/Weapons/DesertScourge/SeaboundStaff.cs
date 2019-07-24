using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.GameContent;
using Terraria.IO;
using Terraria.ObjectData;
using Terraria.Utilities;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.DesertScourge
{
	public class SeaboundStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Seabound Staff");
			Tooltip.SetDefault("Summons a brittle star to fight for you");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 11;
	        item.mana = 10;
	        item.width = 32;
	        item.height = 32;
	        item.useTime = 36;
	        item.useAnimation = 36;
	        item.scale = 0.75f;
	        item.useStyle = 1;
	        item.noMelee = true;
	        item.knockBack = 2f;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 2;
	        item.UseSound = SoundID.Item44;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("BrittleStar");
	        item.shootSpeed = 10f;
	        item.summon = true;
	    }

		public override bool AltFunctionUse(Player player)
		{
			return true;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			if (player.altFunctionUse != 2)
			{
				position = Main.MouseWorld;
				speedX = 0;
				speedY = 0;
				Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI);
			}
			return false;
		}

		public override bool UseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				player.MinionNPCTargetAim();
			}
			return base.UseItem(player);
		}
	}
}