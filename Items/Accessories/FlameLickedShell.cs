using System;
using System.Collections.Generic;
using CalamityMod.CalPlayer;
using CalamityMod.Dusts;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Items.Potions.Alcohol;

namespace CalamityMod.Items.Accessories
{
    [LegacyName("FabledTortoiseShell")]
    public class FlameLickedShell : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        internal const int flameLickedParry = 30;
        public override void ModifyTooltips(List<TooltipLine> list) => list.IntegrateHotkey(CalamityKeybinds.AccessoryParryHotKey);
        
        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 42;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.flameLickedShell = true;
        }

        public static void handleParry(Player player)
        {
            var calPlayer = player.Calamity();
            var empowered = calPlayer.flameLickedShellEmpoweredParry;
            
            NPC target = player.Center.ClosestNPCAt(1300f, true, true);
            Vector2 targetPosition = target?.Center ?? Main.MouseWorld;
            float projectileSpeed = 8f;

            float radialOffset = 0.2f;
            float diameter = 80f;
                
            Vector2 projectileVelocity = targetPosition - player.Center;
            projectileVelocity = Vector2.Normalize(projectileVelocity) * projectileSpeed;

            Vector2 velocity = projectileVelocity;
            velocity.Normalize();
            velocity *= diameter;
            int totalProjectiles = 6;
            float offsetAngle = (float)Math.PI * radialOffset;
            int type = ModContent.ProjectileType<FlameLickedHellblast>();
            int damage = (int)player.GetBestClassDamage().ApplyTo(60);
            damage = player.ApplyArmorAccDamageBonusesTo(damage);

            if (player.whoAmI == Main.myPlayer)
            {
                if (empowered)
                {
                    for (int j = 0; j < totalProjectiles; j++)
                    {
                        float radians = j - (totalProjectiles - 1f) / 2f;
                        Vector2 offset = velocity.RotatedBy(offsetAngle * radians);
                        Projectile.NewProjectile(player.GetSource_FromThis(), player.Center + offset, projectileVelocity * 1.5f, type, damage, 2f, Main.myPlayer);
                    }
                }
                totalProjectiles = 12;
                float radians2 = MathHelper.TwoPi / totalProjectiles;
                type = ModContent.ProjectileType<FlameLickedBarrage>();
                damage = (int)player.GetBestClassDamage().ApplyTo(20);
                damage = player.ApplyArmorAccDamageBonusesTo(damage);

                double angleA = radians2 * 0.5;
                double angleB = MathHelper.ToRadians(90f) - angleA;
                float velocityX = (float)(projectileSpeed * Math.Sin(angleA) / Math.Sin(angleB));
                Vector2 spinningPoint = !empowered ? new Vector2(0f, -projectileSpeed) : new Vector2(-velocityX, -projectileSpeed);
                for (int k = 0; k < totalProjectiles; k++)
                {
                    Vector2 vector255 = spinningPoint.RotatedBy(radians2 * k);
                    int proj = Projectile.NewProjectile(player.GetSource_FromAI(), player.Center + Vector2.Normalize(vector255) * 5f, vector255 * (empowered ? 0.99f : 1.15f), type, damage, 1f, Main.myPlayer, empowered ? 0f : 1f);
                    if (empowered && proj.WithinBounds(Main.maxProjectiles))
                        Main.projectile[proj].extraUpdates += 1;
                }
            }

            calPlayer.flameLickedShellEmpoweredParry = false;
        }
        
        public static void HandleParryCountdown(Player player)
        {
            
            player.Calamity().flameLickedShellParry--;

            if (player.Calamity().flameLickedShellParry > 0)
            {
                player.controlJump = false;
                player.controlDown = false;
                player.controlLeft = false;
                player.controlRight = false;
                player.controlUp = false;
                player.controlUseItem = false;
                player.controlUseTile = false;
                player.controlThrow = false;
                player.gravDir = 1f;
                player.velocity = Vector2.Zero;
                player.velocity.Y = -0.1f; //if player velocity is 0, the flight meter gets reset
                player.RemoveAllGrapplingHooks();
            }
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    int theDust = Dust.NewDust(player.position, player.width, player.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, new Color(255, 255, 255), 2f);
                    Main.dust[theDust].noGravity = true;
                }
            }
        }
    }
}
