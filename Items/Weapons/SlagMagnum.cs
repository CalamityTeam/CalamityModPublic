using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons
{
    public class SlagMagnum : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slag Magnum");
        }

        public override void SetDefaults()
        {
            item.damage = 24;
            item.ranged = true;
            item.width = 58;
            item.height = 24;
            item.useTime = 15;
            item.useAnimation = 15;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 4f;
            item.value = 100000;
            item.rare = 5;
            item.UseSound = SoundID.Item41;
            item.autoReuse = true;
            item.shootSpeed = 22f;
            item.shoot = mod.ProjectileType("SlagRound");
            item.useAmmo = 97;
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("SlagRound"), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
            return false;
        }
    }
}