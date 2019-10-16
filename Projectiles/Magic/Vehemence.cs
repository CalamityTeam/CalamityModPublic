﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Projectiles
{
    public class Vehemence : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vehemence");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 26;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.extraUpdates = 1;
            projectile.timeLeft = 300;
            projectile.magic = true;
        }

        public override void AI()
        {
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
            Lighting.AddLight(projectile.Center, 0.45f, 0f, 0.45f);
            for (int num457 = 0; num457 < 2; num457++)
            {
                int num458 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 65, 0f, 0f, 100, default, 2f);
                Main.dust[num458].noGravity = true;
                Main.dust[num458].velocity *= 0.15f;
                Main.dust[num458].velocity += projectile.velocity * 0.1f;
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 74);
            for (int j = 0; j <= 25; j++)
            {
                int num459 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 65, 0f, 0f, 100, default, 1f);
                Main.dust[num459].noGravity = true;
                Main.dust[num459].velocity *= 0.1f;
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            double lifeAmount = (double)target.life;
            double lifeMax = (double)target.lifeMax;
            double damageMult = lifeAmount / lifeMax * 7;
            damage = (int)Math.Pow(damage, damageMult);
            if (damage > 1000000)
            {
                damage = 1000000;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (target.life == target.lifeMax)
            {
                target.AddBuff(BuffID.ShadowFlame, 12000);
                target.AddBuff(BuffID.Ichor, 12000);
                target.AddBuff(BuffID.Frostburn, 12000);
                target.AddBuff(BuffID.OnFire, 12000);
                target.AddBuff(BuffID.Poisoned, 12000);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 2);
            return false;
        }
    }
}
