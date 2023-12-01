using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class BrimstoneFire : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public override string Texture => "CalamityMod/Projectiles/FireProj";

        public static int Lifetime => 90;
        public static int Fadetime => 80;
        public ref float Time => ref Projectile.ai[0];
        public int MistType = -1;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.MaxUpdates = 3;
            Projectile.timeLeft = Lifetime;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            Time++;
            if (Time < Fadetime && Main.rand.NextBool(6))
            {
                Vector2 cinderPos = Projectile.Center + Main.rand.NextVector2Circular(60f, 60f) * Utils.Remap(Time, 0f, Lifetime, 0.5f, 1f);
                float cinderSize = Utils.GetLerpValue(6f, 12f, Time, true);
                Dust cinder = Dust.NewDustDirect(cinderPos, 4, 4, ModContent.DustType<BrimstoneFlame>(), Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f);
                if (Main.rand.NextBool(3))
                {
                    cinder.scale *= 2f;
                    cinder.velocity *= 2f;
                }
                cinder.noGravity = true;
                cinder.scale *= cinderSize * 1.2f;
                cinder.velocity += Projectile.velocity * Utils.Remap(Time, 0f, Fadetime * 0.75f, 1f, 0.1f) * Utils.Remap(Time, 0f, Fadetime * 0.1f, 0.1f, 1f);
            }

            if (MistType == -1)
                MistType = Main.rand.Next(3);

            Lighting.AddLight(Projectile.Center, 0.75f, 0.15f, 0.15f);
        }

        // Keeping the flames in place when hitting a block
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = oldVelocity * 0.95f;
			Projectile.position -= Projectile.velocity;
            Time++;
            Projectile.timeLeft--;
            return false;
        }

        // Expanding hitbox
        // A bit more generous than Havoc's Breath fire because enemy projectiles
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            int size = (int)Utils.Remap(Time, 0f, Fadetime, 8f, 32f);

            // Shrinks again after fading
            if (Time > Fadetime)
                size = (int)Utils.Remap(Time, Fadetime, Lifetime, 32f, 0f);
            hitbox.Inflate(size, size);
        }

        // Anti-wall hacking checks
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => projHitbox.Intersects(targetHitbox) && Collision.CanHit(Projectile.Center, 0, 0, targetHitbox.Center.ToVector2(), 0, 0) ? null : false;

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 150);

            // Cook you up (still scales with player size in case it's manipulated)
            int smokeCount = 4 + (int)MathHelper.Clamp(target.width * 0.1f, 0f, 20f);
            for (int i = 0; i < smokeCount; i++)
            {
                Vector2 smokePos = target.Center + Main.rand.NextVector2Circular(target.width * 0.5f, target.height * 0.5f);
                Vector2 smokeVel = Vector2.UnitY * Main.rand.NextFloat(-2.4f, -0.8f) * MathHelper.Clamp(target.height * 0.1f, 1f, 10f);
                Particle smoke = new MediumMistParticle(smokePos, smokeVel, new Color(255, 50, 50), Color.DimGray, Main.rand.NextFloat(1f, 2f), 245 - Main.rand.Next(50), 0.1f);
                GeneralParticleHandler.SpawnParticle(smoke);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D fire = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D mist = ModContent.Request<Texture2D>("CalamityMod/Particles/MediumMist").Value;

            // The conga line of colors to sift through
            Color color1 = new Color(255, 110, 100, 200);
            Color color2 = new Color(255, 50, 50, 70);
            Color color3 = new Color(255, 100, 100, 100);
            Color color4 = new Color(200, 35, 30, 100);
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
                Color innerColor = Color.Lerp(fireColor, Color.Black, 0.3f);

                // Positions and rotations
                Vector2 firePos = Projectile.Center - Main.screenPosition - Projectile.velocity * vOffset * j;
                float mainRot = -j * MathHelper.PiOver2 - Main.GlobalTimeWrappedHourly * (j + 1f) * 2f / length;
                float trailRot = MathHelper.PiOver4 - mainRot;

                // Draw one backtrail
                Vector2 trailOffset = Projectile.velocity * vOffset * length * 0.5f;
                Main.EntitySpriteDraw(fire, firePos - trailOffset, null, innerColor * 0.25f, trailRot, fire.Size() * 0.5f, fireSize, SpriteEffects.None);

                // Draw the main fire
                Main.EntitySpriteDraw(fire, firePos, null, innerColor, mainRot, fire.Size() * 0.5f, fireSize, SpriteEffects.None);

                // Draw the masking smoke
                Main.spriteBatch.SetBlendState(BlendState.Additive);
                Rectangle frame = mist.Frame(1, 3, 0, MistType);
                Main.EntitySpriteDraw(mist, firePos, frame, Color.Lerp(fireColor, Color.White, 0.3f), mainRot, frame.Size() * 0.5f, fireSize, SpriteEffects.None);
                Main.EntitySpriteDraw(mist, firePos, frame, fireColor, mainRot, frame.Size() * 0.5f, fireSize * 3f, SpriteEffects.None);
                Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            }
            return false;
        }
    }
}
