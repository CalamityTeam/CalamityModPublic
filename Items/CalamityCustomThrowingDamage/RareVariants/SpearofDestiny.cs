using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.CalamityCustomThrowingDamage.RareVariants
{
	public class SpearofDestiny : CalamityDamageItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Spear of Destiny");
		}

		public override void SafeSetDefaults()
		{
			item.width = 52;
			item.damage = 25;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.useAnimation = 20;
			item.useStyle = 1;
			item.useTime = 20;
			item.knockBack = 2f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 52;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
			item.shoot = mod.ProjectileType("IchorSpear");
			item.shootSpeed = 20f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).rogue = true;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 22;
		}

		public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			int numProj = 2;
			float rotation = MathHelper.ToRadians(3);
			for (int i = 0; i < numProj + 1; i++)
			{
				Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
				Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, (i == 1 ? type : mod.ProjectileType("IchorSpear2")), damage, knockBack, player.whoAmI, 0f, 0f);
			}
			return false;
		}
	}
}
