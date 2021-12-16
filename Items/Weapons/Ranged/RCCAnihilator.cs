using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using System.Collections.Generic;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.Projectiles.Ranged;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class RCCAnihilator : ModItem
    {
        public const int MaxBolts = 6;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rending Greatbow of the Six Northen Winds: Anihilator of Right Click Channel Restrictions");
            Tooltip.SetDefault("Hold RIGHT click to load up to six precision bolts\n" +
                "The more precision bolts are loaded, the harder they hit");
        }

        public override void SetDefaults()
        {
            item.damage = 10000000;
            item.ranged = true;
            item.width = 48;
            item.height = 96;
            item.useTime = 60;
            item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4.25f;
            item.value = CalamityGlobalItem.Rarity10BuyPrice;
            item.rare = ItemRarityID.Red;
            item.UseSound = SoundID.Item5;
            item.autoReuse = true;
            item.shoot = ProjectileID.PurificationPowder;
            item.shootSpeed = 15f;
            item.useAmmo = AmmoID.Arrow;            
        }

        public override string Texture => "CalamityMod/Items/Weapons/Ranged/ClockworkBow";
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            item.noUseGraphic = true;
            item.UseSound = SoundID.Item20;
            item.channel = true;
            return player.ownedProjectileCounts[ModContent.ProjectileType<RCCHoldout>()] <= 0;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse == 2)
            {
                Vector2 shootVelocity = new Vector2(speedX, speedY);
                Vector2 shootDirection = shootVelocity.SafeNormalize(Vector2.UnitX * player.direction);
                // Charge-up. Done via a holdout projectile.
                Projectile.NewProjectile(position, shootDirection, ModContent.ProjectileType<RCCHoldout>(), damage, knockBack, player.whoAmI);
                return false;
            }
            else //This is just a test thing 
            {
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<SeasSearingBubble>(), damage, knockBack, player.whoAmI, 1f, 0f);
                return false;
            }
        }
    }
}
