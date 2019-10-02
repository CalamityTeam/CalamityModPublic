using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;
using CalamityMod;
using static CalamityMod.CalPlayer.CalamityPlayer;

namespace CalamityMod.Items.CalamityCustomThrowingDamage
{
	public class ScourgeoftheSeas : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Scourge of the Seas");
			Tooltip.SetDefault("Snaps apart into a venomous cloud upon striking an enemy\n" +
			"Stealth strikes increase the strength of the venom");
		}

		public override void SetDefaults()
		{
			item.width = 82;
			item.damage = 45;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.useAnimation = 20;
			item.useStyle = 1;
			item.useTime = 20;
			item.knockBack = 3.5f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 82;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
			item.shoot = mod.ProjectileType("ScourgeoftheSeasProjectile");
			item.shootSpeed = 8f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).rogue = true;
		}

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.GetCalamityPlayer().StealthStrikeAvailable())
			{
				Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("ScourgeoftheSeasStealth"), damage, knockBack, player.whoAmI, 0f, 0f);
			}
			else
			{
				Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
			}
			return false;
		}
	}
}
