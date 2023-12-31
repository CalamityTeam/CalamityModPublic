﻿using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Sounds;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class PolarisParrotfish : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetStaticDefaults()
        {
                       Item.staff[Item.type] = true; //so it doesn't look weird af when holding it
        }

        public override void SetDefaults()
        {
            Item.damage = 72;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 38;
            Item.height = 34;
            Item.useTime = 21;
            Item.useAnimation = 21;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.25f;
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = CommonCalamitySounds.LaserCannonSound; //pew pew
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<PolarStar>();
            Item.shootSpeed = 15f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.polarisBoostThree) //Homes in and explodes
            {
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<PolarStar>(), damage, knockback, player.whoAmI, 0f, 2f);
                return false;
            }
            else if (modPlayer.polarisBoostTwo) //Splits on enemy or tile hits
            {
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<PolarStar>(), (int)(damage * 1.25), knockback, player.whoAmI, 0f, 1f);
                return false;
            }
            return true;
        }

        public override Vector2? HoldoutOrigin() //so it looks normal when holding
        {
            return new Vector2(10, 10);
        }
    }
}
