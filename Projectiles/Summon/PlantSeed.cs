﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class PlantSeed : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Seed");
            Main.projFrames[Projectile.type] = 2;
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.light = 0.5f;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 2;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = 1;
            AIType = ProjectileID.BulletHighVelocity;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 8)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= 2)
            {
                Projectile.frame = 0;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 2);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Poisoned, 60);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Poisoned, 60);
        }
    }
}
