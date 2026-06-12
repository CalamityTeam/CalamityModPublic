using System;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class PhalanxSurgeLance : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Misc";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public Vector2 startVel = Vector2.Zero;
        public int time = 0;
        public bool didDash => Projectile.ai[2] == 5;
        public override void SetDefaults()
        {
            Projectile.width = 35;
            Projectile.height = 35;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 10;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Effects.ArsenalEffects.ArsenalLaserColor.ToVector3());
            Player Owner = Main.player[Projectile.owner];
            float targetDist = Vector2.Distance(Owner.Center, Projectile.Center);

            if (time == 0)
            {
                startVel = Projectile.velocity;
                for (int i = 0; i <= 15; i++)
                {
                    Dust chargefull = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(40, 40), Effects.ArsenalEffects.ArsenalLaserDust);
                    chargefull.velocity = (Projectile.velocity * 10).RotatedByRandom(0.3f) * Main.rand.NextFloat(0.2f, 2f);
                    chargefull.scale = Main.rand.NextFloat(0.4f, 0.8f);
                    chargefull.noGravity = true;
                    chargefull.color = Effects.ArsenalEffects.ArsenalLaserColor;
                }
                for (int i = 0; i < 20; i++)
                {
                    Vector2 place = Projectile.Center + startVel * 20 + Main.rand.NextVector2Circular(5, 5);
                    int dir = Main.rand.NextBool() ? 1 : -1;
                    Particle chargeParticles = new CustomSpark(place, -Projectile.velocity.RotatedBy(0.42f * dir).RotatedByRandom(0.1f) * Main.rand.NextFloat(12, 35), "CalamityMod/Particles/BloomLineSoftEdge", false, 7, 0.025f, Effects.ArsenalEffects.ArsenalLaserColor, new Vector2(1f, 0.7f), shrinkSpeed: 1.3f);
                    GeneralParticleHandler.SpawnParticle(chargeParticles);

                    if (i % 2 == 0)
                    {
                        Dust dust = Dust.NewDustPerfect(place, Effects.ArsenalEffects.ArsenalLaserDust, -Projectile.velocity.RotatedBy(0.42f * dir).RotatedByRandom(0.1f) * Main.rand.NextFloat(4, 20), 0, default, Main.rand.NextFloat(0.7f, 1.2f));
                        dust.noGravity = true;
                        dust.color = Effects.ArsenalEffects.ArsenalLaserColor;
                        dust.alpha = 100;
                    }
                }
            }
            Projectile.rotation = startVel.ToRotation() + MathHelper.PiOver2;
            Projectile.velocity = Vector2.Zero;
            Projectile.Center = Owner.Center + startVel * 30;
            time++;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            bool onKill = (target.life <= 0 && target.realLife == -1);
            Player Owner = Main.player[Projectile.owner];
            float distMult = Utils.GetLerpValue(400, 10, Utils.Distance(target.Center, Projectile.Center), true);
            Vector2 launchVel = startVel;
            target.MoveNPC(launchVel, 10 + 35 * distMult, true, Owner);

            if (Projectile.numHits == 0)
            {
                if (!onKill || !didDash)
                {
                    for (int i = -5; i <= 5; i++)
                    {
                        if (i == 0)
                            i++;
                        Particle spark2 = new CustomSpark(target.Center, startVel * i * 2, "CalamityMod/Particles/BloomLineSoftEdge", false, 12, 0.085f - Math.Abs(i) * 0.012f, Effects.ArsenalEffects.ArsenalLaserColor * 1f, new Vector2((6 - Math.Abs(i)) * 0.4f, 1), true, true, 0, false, false, 1.2f, glowOpacity: 0.6f);
                        GeneralParticleHandler.SpawnParticle(spark2);
                    }
                    if (Projectile.timeLeft > 10)
                        Projectile.timeLeft = 10;
                    for (int x = 0; x < Main.maxProjectiles; x++)
                    {
                        Projectile projectile = Main.projectile[x];
                        if (projectile.active && projectile.type == ModContent.ProjectileType<PhalanxSurgeHoldout>() && projectile.owner == Projectile.owner)
                        {
                            projectile.localAI[0] = distMult; // Tell the holdout that you hit a target and how much recoil
                        }
                    }
                }
                else if (didDash)
                {
                    int extention = 25;
                    for (int x = 0; x < Main.maxProjectiles; x++)
                    {
                        Projectile projectile = Main.projectile[x];
                        if (projectile.active && projectile.type == ModContent.ProjectileType<PhalanxSurgeHoldout>() && projectile.owner == Projectile.owner && projectile.ai[0] > -extention)
                        {
                            projectile.ai[0] -= extention;
                        }
                    }
                    if (Projectile.timeLeft < extention)
                        Projectile.timeLeft = extention;
                }

                SoundStyle sound = new("CalamityMod/Sounds/Item/MeldShoot");
                SoundEngine.PlaySound(sound with { Volume = 1f }, Projectile.Center);
            }
            for (int i = 0; i < MathHelper.Clamp(25 - Projectile.numHits * 3, 1, 10); i++)
            {
                Vector2 velocity = startVel.RotatedByRandom(0.2f) * Main.rand.NextFloat(8f, 20f);

                Dust dust = Dust.NewDustPerfect(Projectile.Center, Effects.ArsenalEffects.ArsenalLaserDust, velocity, 0, default, Main.rand.NextFloat(0.9f, 1.8f));
                dust.noGravity = true;
                dust.color = Effects.ArsenalEffects.ArsenalLaserColor;
                dust.alpha = 100;
                dust.fadeIn = -3;
            }
            if (onKill)
                Projectile.numHits--;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float minMult = 0.4f;
            int hitsToMinMult = 10;
            float damageMult = Utils.Remap(Projectile.numHits, 0, hitsToMinMult, 1, minMult, true);
            modifiers.SourceDamage *= damageMult;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Player Owner = Main.player[Projectile.owner];
            if (startVel == Vector2.Zero)
                return false;
            // If the target is touching the beam's hitbox (which is a small rectangle vaguely overlapping the host crystal), that's good enough.
            if (projHitbox.Intersects(targetHitbox))
                return true;
            // Otherwise, perform an AABB line collision check to check the whole beam.
            float _ = float.NaN;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.Center, Projectile.Center + startVel * 145, Projectile.width * Projectile.scale, ref _);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (time < 1)
                return false;
            Texture2D pointTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomLineFade").Value;
            Texture2D bloomTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;
            float fade = Utils.GetLerpValue(0, 5, Projectile.timeLeft, true);
            Vector2 distVel = startVel * 10;
            for (int i = 0; i < 6; i++)
                Main.EntitySpriteDraw(pointTexture, Projectile.Center - Main.screenPosition + (distVel * (3 + i)), null, Effects.ArsenalEffects.ArsenalLaserColor with { A = 0 } * fade, Projectile.rotation, pointTexture.Size() * 0.5f, new Vector2(0.5f * (1 - i * 0.08f) * fade, 0.8f * (1 + i * 0.07f)) * 0.11f, SpriteEffects.None);
            for (int i = 0; i < 6; i++)
                Main.EntitySpriteDraw(pointTexture, Projectile.Center - Main.screenPosition + (distVel * (3 + i)), null, Color.White with { A = 0 } * 0.8f * fade, Projectile.rotation, pointTexture.Size() * 0.5f, new Vector2(0.5f * (1 - i * 0.08f) * fade, 0.8f * (1 + i * 0.07f)) * 0.07f, SpriteEffects.None);

            for (int i = 0; i < 2; i++)
                Main.EntitySpriteDraw(bloomTexture, Projectile.Center - Main.screenPosition, null, Effects.ArsenalEffects.ArsenalLaserColor with { A = 0 } * fade, Projectile.rotation, bloomTexture.Size() * 0.5f, new Vector2(0.6f, 1) * 0.8f, SpriteEffects.None);
            Main.EntitySpriteDraw(bloomTexture, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 } * fade, Projectile.rotation, bloomTexture.Size() * 0.5f, new Vector2(0.5f, 1) * 0.65f, SpriteEffects.None);

            return false;
        }
    }
}
