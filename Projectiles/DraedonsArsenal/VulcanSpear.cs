using System;
using CalamityMod.Dusts;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class VulcanSpear : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Misc";
        public ref float time => ref Projectile.ai[0];

        public NPC targeted;
        public bool stuckInGround = false;
        public bool canDamage => (targeted == null && !stuckInGround);
        public bool canStick = true;
        public int stuckTimer = 420;
        public Vector2 placementCenter;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 35;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 1200;
            Projectile.extraUpdates = 30;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override void AI()
        {
            Player Owner = Main.player[Projectile.owner];
            float targetDist = Vector2.Distance(Owner.Center, Projectile.Center);

            Lighting.AddLight(Projectile.Center, Effects.ArsenalEffects.ArsenalGaussColor.ToVector3() * 0.5f);
            if (targetDist < 1400 && canDamage)
            {
                if (time % 2 == 0)
                {
                    Particle trail = new CustomSpark(Projectile.Center - Projectile.velocity.SafeNormalize(Vector2.UnitX) * 14, Projectile.velocity * 0.2f, "CalamityMod/Particles/BloomCircle", false, 12, 0.065f, Effects.ArsenalEffects.ArsenalGaussColor, new Vector2(1f, 2.5f), true, true, glowCenterScale: 0.7f, shrinkSpeed: 0.4f);
                    GeneralParticleHandler.SpawnParticle(trail);
                }
            }

            if (!stuckInGround && targeted == null && Collision.SolidCollision(Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitX) * 15, 6, 6))
            {
                Projectile.extraUpdates = 0;
                stuckInGround = true;
                SoundStyle hitTile = new("CalamityMod/Sounds/NPCHit/ExoHit3");
                SoundEngine.PlaySound(hitTile with { Volume = 0.8f, Pitch = Main.rand.NextFloat(0.1f, 0.2f) }, Projectile.Center);
                //Get stuck in the ground and fade
            }
            if (targeted != null && (!targeted.active || targeted.life <= 0))
                targeted = null;
            if (targeted != null)
            {
                Projectile.Center = targeted.Center - placementCenter;
            }
            else if (Projectile.numHits > 0)
            {
                Projectile.timeLeft = 1;
            }
            if (stuckInGround || targeted != null)
            {
                foreach (Projectile p in Main.ActiveProjectiles)
                {
                    float dist = Utils.Distance(Projectile.Center, p.Center);
                    if (p.type == ModContent.ProjectileType<VulcanProjectile>() && dist <= 900 && p.ai[0] > 120)
                    {
                        if (p.velocity.Length() < 6)
                            p.velocity += p.Center.DirectionTo(Projectile.Center) * Utils.GetLerpValue(1500, 50, dist) * 0.9f;
                        else
                            p.velocity *= 0.9f;
                        if (p.ai[0] % 2 == 0)
                            p.timeLeft += p.MaxUpdates;
                    }
                }
            }
            if (!canDamage && Projectile.timeLeft > 300)
                Projectile.timeLeft = 300;
            time++;
            if (Projectile.timeLeft == 35)
            {
                SoundStyle sound2 = new("CalamityMod/Sounds/Item/VulcanRampUp");
                for (int i = 0; i < 2; i++)
                    SoundEngine.PlaySound(sound2 with { Volume = 0.9f, MaxInstances = 5, Pitch = -0.4f }, Projectile.Center);
            }
        }
        public override void OnKill(int timeLeft)
        {
            SoundStyle die = new("CalamityMod/Sounds/NPCHit/ExoHit2");
            SoundEngine.PlaySound(die with { Volume = 0.8f, Pitch = Main.rand.NextFloat(0.4f, 0.5f), MaxInstances = 5 }, Projectile.Center);
            SoundStyle sound2 = new("CalamityMod/Sounds/Item/NidhoggFire");
            for (int i = 0; i < 2; i++)
                SoundEngine.PlaySound(sound2 with { Volume = 0.8f, MaxInstances = 5, Pitch = -0.1f }, Projectile.Center);
            foreach (Projectile p in Main.ActiveProjectiles)
            {
                float dist = Utils.Distance(Projectile.Center, p.Center);
                if (p.type == ModContent.ProjectileType<VulcanProjectile>() && dist <= 500)
                {
                    NPC closeTarget = p.Center.ClosestNPCAt(800);
                    if (closeTarget != null)
                        p.velocity = p.Center.DirectionTo(closeTarget.Center) * 7;
                    else
                        p.velocity = Projectile.Center.DirectionTo(p.Center) * 7;
                    p.extraUpdates = 8;
                    p.ai[0] = 0;
                    p.penetrate = 5;
                    if (p.ai[1] < 4) // Pretty much just so gfb can't oneshot everything
                    {
                        p.damage = (int)(p.damage * 2f);
                        p.ai[1]++;
                    }
                    p.timeLeft = Main.rand.Next(100, 120 + 1);
                }
            }
            Particle bolt = new CustomPulse(Projectile.Center, Vector2.Zero, Effects.ArsenalEffects.ArsenalGaussColor, "CalamityMod/Particles/GlowSquareParticleThick", Vector2.One, MathHelper.PiOver4, 0.1f, 0.7f, 15);
            GeneralParticleHandler.SpawnParticle(bolt);
            Particle bolt2 = new CustomPulse(Projectile.Center, Vector2.Zero, Effects.ArsenalEffects.ArsenalGaussColor, "CalamityMod/Particles/GlowSquareParticleBig", Vector2.One, 0, 0.3f, 1.15f, 10);
            GeneralParticleHandler.SpawnParticle(bolt2);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Player Owner = Main.player[Projectile.owner];
            impactDust();

            Vector2 launchVel = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            float launchPower = 12;
            target.MoveNPC(launchVel, launchPower, true, Owner);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.life <= 0 && target.realLife == -1)
            {
                // Don't stick into dead enemies
                Projectile.numHits--;
            }
            else if (targeted == null && !stuckInGround)
            {
                //Stick
                Projectile.extraUpdates = 0;
                SoundStyle hitTile = new("CalamityMod/Sounds/NPCHit/ExoHit1");
                SoundEngine.PlaySound(hitTile with { Volume = 0.8f, Pitch = Main.rand.NextFloat(0.1f, 0.2f) }, Projectile.Center);
                targeted = target;
                placementCenter = targeted.Center - Projectile.Center;
            }
        }
        public override bool? CanDamage() => canDamage ? null : false;
        public override bool ShouldUpdatePosition()
        {
            return canDamage;
        }
        public void impactDust()
        {
            for (int i = 0; i < 7; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SquashDust>());
                dust.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(0.15f) * Main.rand.NextFloat(12f, 19f);
                dust.scale = Main.rand.NextFloat(0.8f, 1.35f);
                dust.noGravity = true;
                dust.color = Effects.ArsenalEffects.ArsenalGaussColor;
                dust.noLightEmittence = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D proj = ModContent.Request<Texture2D>("CalamityMod/Projectiles/DraedonsArsenal/VulcanSpear").Value;
            Texture2D glow = ModContent.Request<Texture2D>("CalamityMod/Projectiles/DraedonsArsenal/VulcanSpearGlow").Value;

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 vel = Projectile.rotation.ToRotationVector2();

            Color drawColor = Effects.ArsenalEffects.ArsenalGaussColor with { A = 0 };
            float drawRotation = Projectile.velocity.ToRotation() + (Projectile.direction == -1 ? MathHelper.Pi : 0f);
            Vector2 rotationPoint = proj.Size() * 0.5f;
            SpriteEffects flipSprite = Projectile.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Main.EntitySpriteDraw(proj, drawPosition, null, lightColor, drawRotation, rotationPoint, Projectile.scale, flipSprite);
            Main.EntitySpriteDraw(glow, drawPosition, null, Color.White, drawRotation, rotationPoint, Projectile.scale, flipSprite);

            int draws = 16;
            for (int i = 0; i < draws; i++)
            {
                float fadeIn = (float)Math.Pow(Utils.GetLerpValue(300, 1, Projectile.timeLeft, true), 3);
                Vector2 offset = (MathHelper.TwoPi / draws * i).ToRotationVector2() * 3 * fadeIn;
                Main.EntitySpriteDraw(proj, Projectile.Center - Main.screenPosition + offset + Main.rand.NextVector2Circular(3, 3) * fadeIn, null, Effects.ArsenalEffects.ArsenalGaussColor with { A = 0 } * fadeIn * 0.5f, drawRotation, proj.Size() * 0.5f, Projectile.scale, flipSprite);
                Main.EntitySpriteDraw(proj, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 } * fadeIn, drawRotation, proj.Size() * 0.5f, Projectile.scale, flipSprite);
            }

            return false;
        }
    }
}
