using CalamityMod.Dusts;
using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class BrimstoneSwordExplosion : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 130;
            Projectile.height = 130;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 60;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.75f / 255f, (255 - Projectile.alpha) * 0.05f / 255f, (255 - Projectile.alpha) * 0.05f / 255f);
            if (Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item20, Projectile.position);
                Projectile.localAI[0] += 1f;
            }
            bool xflag = false;
            bool yflag = false;
            if (Projectile.velocity.X < 0f && Projectile.position.X < Projectile.ai[0])
            {
                xflag = true;
            }
            if (Projectile.velocity.X > 0f && Projectile.position.X > Projectile.ai[0])
            {
                xflag = true;
            }
            if (Projectile.velocity.Y < 0f && Projectile.position.Y < Projectile.ai[1])
            {
                yflag = true;
            }
            if (Projectile.velocity.Y > 0f && Projectile.position.Y > Projectile.ai[1])
            {
                yflag = true;
            }
            if (xflag && yflag)
            {
                Projectile.Kill();
            }
            float projTimer = 25f;
            if (Projectile.ai[0] > 180f)
            {
                projTimer -= (Projectile.ai[0] - 180f) / 2f;
            }
            if (projTimer <= 0f)
            {
                projTimer = 0f;
                Projectile.Kill();
            }
            projTimer *= 0.7f;
            Projectile.ai[0] += 4f;
            int timerCounter = 0;
            while ((float)timerCounter < projTimer)
            {
                float rando1 = (float)Main.rand.Next(-10, 11);
                float rando2 = (float)Main.rand.Next(-10, 11);
                float rando3 = (float)Main.rand.Next(3, 9);
                float randoAdjuster = (float)Math.Sqrt((double)(rando1 * rando1 + rando2 * rando2));
                randoAdjuster = rando3 / randoAdjuster;
                rando1 *= randoAdjuster;
                rando2 *= randoAdjuster;
                int brimDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 1.5f);
                Dust dust = Main.dust[brimDust];
                dust.noGravity = true;
                dust.position.X = Projectile.Center.X;
                dust.position.Y = Projectile.Center.Y;
                dust.position.X += (float)Main.rand.Next(-10, 11);
                dust.position.Y += (float)Main.rand.Next(-10, 11);
                dust.velocity.X = rando1;
                dust.velocity.Y = rando2;
                timerCounter++;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 180);
    }
}
