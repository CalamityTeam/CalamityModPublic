using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class EssenceFire : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/FireProj";

        public static int Lifetime => 105;
        public static int Fadetime => 90;
        public ref float Time => ref Projectile.ai[0];

        public bool spawnParticles = true;
        public bool spawnDusts = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.MaxUpdates = 4;
            Projectile.timeLeft = Lifetime;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 3;
        }

        public override void AI()
        {
            Time++;
            if (Time > Fadetime)
                Projectile.velocity *= 0.95f;

            if ( Time > 6 && Time < Fadetime && Main.rand.NextBool(16))
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center + Main.rand.NextVector2Circular(60f, 60f) * Utils.Remap(Time, 0f, Lifetime, 0.5f, 1f), 4, 4, 295, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100);
                if (Main.rand.NextBool(5))
                {
                    dust.noGravity = true;
                    dust.scale *= 2f;
                    dust.velocity *= 0.8f;
                }
                dust.velocity *= 1.1f;
                dust.velocity += Projectile.velocity * Utils.Remap(Time, 0f, Fadetime * 0.75f, 1f, 0.1f) * Utils.Remap(Time, 0f, Fadetime * 0.1f, 0.1f, 1f);
            }
            if (spawnParticles && Time > 6 && Main.rand.NextBool(7))
            {
                SparkParticle spark = new SparkParticle(Projectile.Center, Projectile.velocity * Main.rand.NextFloat(0.4f, 0.7f), false, Main.rand.Next(7, 11), Main.rand.NextFloat(0.7f, 1.8f), Main.rand.NextBool(4) ? Color.Magenta : Color.DodgerBlue);
                GeneralParticleHandler.SpawnParticle(spark);
            }
            if (Time == 5)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(3) ? 295 : 181, Projectile.velocity);
                dust.scale = Main.rand.NextFloat(0.9f, 1.7f);
                dust.velocity = Projectile.velocity.RotatedByRandom(1) * Main.rand.NextFloat(0.3f, 0.7f);
                dust.noGravity = true;
            }
        }

        // Keeping the flames in place when hitting a block
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (spawnDusts)
            {
                for (int i = 0; i <= 8; i++)
                {
                    Vector2 position = Projectile.Center + new Vector2(Main.rand.NextFloat(-13, 13), Main.rand.NextFloat(-13, 13));
                    Vector2 velocity = -oldVelocity.RotatedByRandom(1) * Main.rand.NextFloat(0.2f, 2.2f);
                    float scale = Main.rand.NextFloat(2f, 3.2f);

                    Dust dust = Dust.NewDustPerfect(position + velocity, Main.rand.NextBool(3) ? 307 : 295, velocity, 0, default, scale);
                    dust.noGravity = true;
                    dust.scale -= 0.4f;
                }
                spawnDusts = false;
            }
            Projectile.velocity = oldVelocity * 0.95f;
			Projectile.position -= Projectile.velocity;
            Time += 2;
            Projectile.timeLeft -= 2;
            spawnParticles = false;
            return false;
        }

        // Expanding hitbox
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            int size = (int)Utils.Remap(Time, 0f, Fadetime, 10f, 40f);

            // Shrinks again after fading
            if (Time > Fadetime)
                size = (int)Utils.Remap(Time, Fadetime, Lifetime, 40f, 0f);
            hitbox.Inflate(size, size);
        }

        // Anti-wall hacking checks
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => projHitbox.Intersects(targetHitbox) && Collision.CanHit(Projectile.Center, 0, 0, targetHitbox.Center.ToVector2(), 0, 0) ? null : false;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 360);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D fire = ModContent.Request<Texture2D>(Texture).Value;

            // The conga line of colors to sift through
            Color color1 = new Color(160, 100, 255, 200);
            Color color2 = new Color(160, 50, 255, 70);
            Color color3 = new Color(200, 100, 255, 100);
            Color color4 = new Color(150, 30, 200, 100);
            float length = ((Time > Fadetime - 10f) ? 0.1f : 0.15f);
            float vOffset = Math.Min(Time, 20f);
            float timeRatio = Utils.GetLerpValue(0f, Lifetime, Time);
            float fireSize = Utils.Remap(timeRatio, 0.2f, 0.5f, 0.25f, 1f);

            if (timeRatio >= 1f)
                return false;

            for (float j = 1f; j >= 0f; j -= length)
            {
                // Color
                Color fireColor = ((timeRatio < 0.1f) ? Color.Lerp(Color.Transparent, color1, Utils.GetLerpValue(0f, 0.1f, timeRatio)) :
                ((timeRatio < 0.2f) ? Color.Lerp(color1, color2, Utils.GetLerpValue(0.1f, 0.2f, timeRatio)) :
                ((timeRatio < 0.35f) ? color2 :
                ((timeRatio < 0.7f) ? Color.Lerp(color2, color3, Utils.GetLerpValue(0.35f, 0.7f, timeRatio)) :
                ((timeRatio < 0.85f) ? Color.Lerp(color3, color4, Utils.GetLerpValue(0.7f, 0.85f, timeRatio)) :
                Color.Lerp(color4, Color.Transparent, Utils.GetLerpValue(0.85f, 1f, timeRatio)))))));
                fireColor *= (1f - j) * Utils.GetLerpValue(0f, 0.2f, timeRatio, true);

                // Positions and rotations
                Vector2 firePos = Projectile.Center - Main.screenPosition - Projectile.velocity * vOffset * j;
                float mainRot = -j * MathHelper.PiOver2 - Main.GlobalTimeWrappedHourly * (j + 1f) * 2f / length;
                float trailRot = MathHelper.PiOver4 - mainRot;

                // Draw one backtrail
                Vector2 trailOffset = Projectile.velocity * vOffset * length * 0.5f;
                Main.EntitySpriteDraw(fire, firePos - trailOffset, null, fireColor * 0.25f, trailRot, fire.Size() * 0.5f, fireSize, SpriteEffects.None);

                // Draw the main fire
                Main.EntitySpriteDraw(fire, firePos, null, fireColor, mainRot, fire.Size() * 0.5f, fireSize, SpriteEffects.None);
            }
            return false;
        }
    }
}
