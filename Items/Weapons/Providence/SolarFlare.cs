using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.Providence
{
    public class SolarFlare : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Solar Flare");
            Tooltip.SetDefault("Emits large holy explosions on enemy hits");
        }

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.TheEyeOfCthulhu);
            item.damage = 60;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = 5;
            item.channel = true;
            item.melee = true;
            item.knockBack = 7.5f;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("SolarFlareYoyo");
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 12;
		}

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, (int)((double)damage), knockBack, player.whoAmI, 0.0f, 0.0f);
            return false;
        }
    }
}