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

namespace CalamityMod.Items.Weapons.AbyssWeapons
{
    public class HerringStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Herring Staff");
            Tooltip.SetDefault("Summons a herring to fight for you");
        }

        public override void SetDefaults()
        {
            item.damage = 20;
            item.mana = 10;
            item.width = 46;
            item.height = 46;
            item.useTime = 36;
            item.useAnimation = 36;
            item.useStyle = 1;
            item.noMelee = true;
            item.knockBack = 1.25f;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = 3;
            item.UseSound = SoundID.Item21;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("Herring");
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