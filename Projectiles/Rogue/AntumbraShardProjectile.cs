using System;
using CalamityMod.Dusts;
using CalamityMod.Enums;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class AntumbraShardProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/ShardofAntumbra";
        public ref float time => ref Projectile.ai[0];

        // Vanilla sticky code is jank, So I did my own (for better or worse)
        public NPC chosenTarget;
        public bool stuckInTarget = false;
        public bool stuckInGround = false;
        public bool canDamage = true;
        public bool canStick = true;
        public int stuckTimer = 180;
        public Vector2 placementCenter;
        float placementDistance;
        Vector2 placementVelocity;
        public Vector2 storedVelocity;
        public bool collideWithTiles = true;
        public Vector2 startingVel;

        public bool jitter = false;
        public Vector2 portalSpot;
        public Vector2 shadowPlacement;
        public int clones = 6; // This is the number of clones spawned by the stealth strike
        NPC closestTarget;
        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 2;
            Projectile.timeLeft = 600;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Player Owner = Main.player[Projectile.owner];
            float targetDist = Vector2.Distance(Owner.Center, Projectile.Center);
            if (closestTarget != null && closestTarget.life <= 0)
                closestTarget = null;
            if (Projectile.ai[2] > 0 && time == 0)
            {
                closestTarget = Projectile.Center.ClosestNPCAt(2000);
                // 15NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
                storedVelocity = ((closestTarget == null ? Owner.Calamity().mouseWorld : closestTarget.Center) - Projectile.Center).SafeNormalize(Vector2.UnitX);
                Projectile.alpha = 255;
                Projectile.velocity = Vector2.Zero;
                stuckInTarget = true;
                collideWithTiles = false;
                canDamage = false;
                portalSpot = Projectile.Center;
            }
            if (time == 0)
            {
                startingVel = Projectile.velocity;
            }
            float fading = stuckInTarget ? Utils.GetLerpValue(0, 180, stuckTimer) : 0.5f;
            if (stuckInTarget && time % ((int)(10 * fading) + 1) == 0)
            {
                shadowPlacement = Main.rand.NextVector2Circular(30, 30);
                jitter = !jitter;
            }

            if (stuckInGround)
            {
                Projectile.rotation = storedVelocity.ToRotation() + MathHelper.ToRadians(45f * (storedVelocity.X > 0 ? 1 : -1));
            }
            if (!stuckInTarget && !stuckInGround)
            {
                storedVelocity = Projectile.velocity;
                Projectile.rotation = (storedVelocity).ToRotation() + MathHelper.ToRadians(45f * (storedVelocity.X > 0 ? 1 : -1));
                if (Projectile.spriteDirection == -1)
                {
                    Projectile.rotation -= MathHelper.PiOver2;
                }
                if (time > 5 && !stuckInTarget)
                {
                    if (Main.rand.NextBool(4) && canStick)
                    {
                        Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(6) ? 278 : 263, -Projectile.velocity);
                        dust.scale = dust.type == 278 ? Main.rand.NextFloat(0.3f, 0.6f) : Main.rand.NextFloat(0.6f, 1.4f);
                        dust.velocity = -Projectile.velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.1f, 0.7f);
                        dust.noGravity = true;
                        dust.color = Color.LightGreen;
                    }
                    if ((Projectile.Calamity().stealthStrike || Projectile.ai[1] == 1) && targetDist < 1400)
                    {
                        float randVel = Main.rand.NextFloat(0.3f, 1.2f);
                        int lifetime = Projectile.Calamity().stealthStrike ? 24 : 8;
                        Particle spark = new AltSparkParticle(Projectile.Center, -Projectile.velocity * randVel, false, lifetime, 1.2f, Color.Black);
                        GeneralParticleHandler.SpawnParticle(spark);
                        Particle spark2 = new SparkParticle(Projectile.Center, -Projectile.velocity * randVel, false, lifetime, 0.9f, Color.LightGreen);
                        GeneralParticleHandler.SpawnParticle(spark2, false, GeneralDrawLayer.AfterEverything);
                        if (Main.rand.NextBool() && !Projectile.Calamity().stealthStrike)
                        {
                            Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB);
                            dust.noGravity = true;
                            dust.velocity = Projectile.velocity * randVel;
                            dust.scale = Main.rand.NextFloat(0.6f, 0.8f);
                            dust.color = Color.LightGreen;
                        }
                    }
                    if (targetDist < 1400f && time % 3 == 0 && !canStick && Projectile.ai[1] == 0)
                    {
                        Vector2 placement = Projectile.Center + Main.rand.NextVector2Circular(15, 15);
                        float speed = Main.rand.NextFloat(0.2f, 0.7f);
                        Particle spark = new AltSparkParticle(placement, -Projectile.velocity * speed, false, 11, 0.8f, Color.Black);
                        GeneralParticleHandler.SpawnParticle(spark);
                        Particle spark2 = new SparkParticle(placement, -Projectile.velocity * speed, false, 11, 0.5f, Color.LightGreen);
                        GeneralParticleHandler.SpawnParticle(spark2, false, GeneralDrawLayer.AfterEverything);
                    }
                    if (closestTarget != null && Projectile.numHits < 1 && closestTarget.CanBeChasedBy(Projectile))
                    {
                        CalamityUtils.HomeInOnSelectedNPC(Projectile, Projectile.Center.ClosestNPCAt(2000), true, 0.95f, 16, 0.96f);
                    }
                }
            }
            else if (stuckInTarget)
            {
                Projectile.rotation = (storedVelocity).SafeNormalize(Vector2.UnitX).ToRotation() + MathHelper.ToRadians(45f * (storedVelocity.X > 0 ? 1 : -1));

                if (Projectile.ai[2] == 0)
                {
                    placementCenter = chosenTarget.Center + placementVelocity * placementDistance + storedVelocity * 2;

                    Projectile.Center = placementCenter;

                    if (Main.rand.NextBool(4))
                    {
                        int dustStyle = ModContent.DustType<VoidDustInverted>();
                        Dust dust = Dust.NewDustPerfect(portalSpot, dustStyle, Projectile.velocity);
                        dust.scale = Main.rand.NextFloat(0.6f, 1.1f);
                        dust.velocity = new Vector2(12, 12).RotatedByRandom(100) * Main.rand.NextFloat(0.2f, 1f) * Utils.GetLerpValue(180, 0, stuckTimer);
                        dust.noGravity = true;
                        dust.color = Color.LightGreen;
                    }

                    if (stuckTimer <= 20)
                        Projectile.scale *= 0.97f;
                }
                if (closestTarget == null)
                    closestTarget = Projectile.Center.ClosestNPCAt(2000);

                if (Projectile.Calamity().stealthStrike && time % 20 == 0 && clones > 0)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Main.rand.NextVector2CircularEdge(450, 450) * Main.rand.NextFloat(0.7f, 1.3f), Vector2.Zero, ModContent.ProjectileType<AntumbraShardProjectile>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, 1, 5);
                    clones--;
                }

                stuckTimer--;
                if (Projectile.ai[2] == 0)
                {
                    if (chosenTarget.life <= 0 || chosenTarget == null)
                    {
                        if (clones > 0 && Projectile.Calamity().stealthStrike)
                        {
                            for (int i = 0; i < clones; i++)
                            {
                                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Main.rand.NextVector2CircularEdge(450, 450) * Main.rand.NextFloat(0.7f, 1.3f), Vector2.Zero, ModContent.ProjectileType<AntumbraShardProjectile>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, 1, 5);
                            }
                            clones = 0;
                        }
                        stuckTimer = 0;
                    }
                }
                if (stuckTimer <= 0)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                        Projectile.localNPCImmunity[i] = 0;

                    SoundStyle sound = new("CalamityMod/Sounds/Item/MeldExplosion");
                    SoundEngine.PlaySound(sound with { Volume = 0.3f, Pitch = 0.8f }, Projectile.Center);

                    Projectile.ai[2] = 0;
                    Projectile.numHits = 0;
                    Projectile.scale = 1;
                    jitter = false;
                    canDamage = true;
                    canStick = false;
                    stuckInTarget = false;
                    Projectile.extraUpdates = 4;

                    Projectile.Center = portalSpot;

                    // 15NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
                    if (closestTarget == null)
                        Projectile.velocity = (Projectile.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitX) * -16;
                    else
                        Projectile.velocity = (Projectile.Center - closestTarget.Center).SafeNormalize(Vector2.UnitX) * -16;

                    for (int i = 0; i <= 12; i++)
                    {
                        float variance = Main.rand.NextFloat(-0.6f, 0.6f);
                        int dustStyle = ModContent.DustType<VoidDustInverted>();
                        Dust dust2 = Dust.NewDustPerfect(Projectile.Center, dustStyle, Projectile.velocity);
                        dust2.scale = Main.rand.NextFloat(1.5f, 1.7f) - Math.Abs(variance);
                        dust2.velocity = Projectile.velocity.RotatedBy(variance) * Main.rand.NextFloat(0.3f, 1f) * (1 - Math.Abs(variance));
                        dust2.noGravity = true;
                        dust2.color = Color.LightGreen;
                    }

                    Particle bolt2 = new CustomPulse(Projectile.Center, Vector2.Zero, Color.LightGreen * 0.85f, "CalamityMod/Particles/BloomRing", Vector2.One, Main.rand.NextFloat(-10f, 10f), 0f, 1f, 18);
                    GeneralParticleHandler.SpawnParticle(bolt2, false, GeneralDrawLayer.AfterEverything);

                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45f * (storedVelocity.X > 0 ? 1 : -1));
                }

            }

            time++;

            if (collideWithTiles && Collision.SolidCollision(Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitX) * 68, 4, 4)) // Vanilla tile collision messes with velocity so we use this instead
            {
                canDamage = false;
                if (Projectile.timeLeft > 180)
                    Projectile.timeLeft = 180;
                stuckInGround = true;

                storedVelocity = Projectile.velocity;
                Projectile.velocity = Vector2.Zero;
                SoundStyle sound = new("CalamityMod/Sounds/NPCHit/RavagerRockPillarHit1");
                SoundEngine.PlaySound(sound with { Volume = 0.25f, Pitch = Main.rand.NextFloat(-0.3f, -0.6f) }, Projectile.Center);
                Projectile.rotation = storedVelocity.ToRotation() + MathHelper.ToRadians(45f * (storedVelocity.X > 0 ? 1 : -1));
                collideWithTiles = false;
                for (int k = 0; k < 8; k++)
                {
                    Vector2 partPos = Projectile.Center + storedVelocity.SafeNormalize(Vector2.UnitX) * 68;
                    Dust dust = Dust.NewDustPerfect(partPos, ModContent.DustType<VoidDustInverted>(), new Vector2(4, 4).RotatedByRandom(100) * Main.rand.NextFloat(0.2f, 1f), 0, default, Main.rand.NextFloat(1.4f, 1.75f));
                    dust.noGravity = true;
                    dust.color = Color.LightGreen;
                }

            }

            if (Projectile.ai[2] == 0)
                Projectile.alpha = (int)(Utils.Remap(Projectile.timeLeft, 0, 60, 255, 0));
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.numHits > 0)
                Projectile.damage = (int)(Projectile.damage * 0.8f);
            if (Projectile.damage < 1)
                Projectile.damage = 1;

            if (!stuckInTarget && canStick)
            {
                if (Projectile.timeLeft < 600)
                    Projectile.timeLeft = 600;
                collideWithTiles = false;
                canDamage = false;
                placementDistance = -Vector2.Distance(target.Center, Projectile.Center);
                placementVelocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitX);
                placementCenter = placementVelocity * (placementDistance * 0.01f);
                chosenTarget = target;
                stuckInTarget = true;
                storedVelocity = Projectile.velocity;
                Projectile.velocity = Vector2.Zero;
                portalSpot = Projectile.Center + Main.rand.NextVector2CircularEdge(350, 350) * Main.rand.NextFloat(0.8f, 1.1f);
                SoundStyle sound = new("CalamityMod/Sounds/Item/WulfrumKnifeTileHit2");
                SoundEngine.PlaySound(sound with { Volume = 0.5f, Pitch = Main.rand.NextFloat(-0.3f, -0.4f) }, Projectile.Center);
                for (int i = 0; i <= 11; i++)
                {
                    int dustStyle = Main.rand.NextBool() ? 66 : 263;
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + storedVelocity.SafeNormalize(Vector2.UnitX) * 68 + Main.rand.NextVector2Circular(12, 12), dustStyle, Projectile.velocity);
                    dust.scale = Main.rand.NextFloat(0.7f, 1.1f);
                    dust.velocity = storedVelocity.RotatedByRandom(0.2f) * Main.rand.NextFloat(0.8f, 2.1f);
                    dust.noGravity = true;
                    dust.color = Color.LimeGreen;
                }
            }

            if (!canStick)
            {
                SoundStyle sound = new("CalamityMod/Sounds/Item/MeldSlice");
                SoundEngine.PlaySound(sound with { Volume = 0.7f, Pitch = 0.4f }, Projectile.Center);
                SoundStyle sound2 = new("CalamityMod/Sounds/Item/ExobladeBeamSlash");
                SoundEngine.PlaySound(sound2 with { Volume = 0.15f, Pitch = 0.7f }, Projectile.Center);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Player Owner = Main.player[Projectile.owner];
            if (time <= 4)
                return false;
            Asset<Texture2D> tex = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> tex2 = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Rogue/ShardofAntumbraGhost");
            Asset<Texture2D> portal = ModContent.Request<Texture2D>("CalamityMod/Particles/Light");
            float alpha = Utils.GetLerpValue(255, 0, Projectile.alpha);
            float fading = stuckInTarget ? Utils.GetLerpValue(0, 180, stuckTimer) : 0.5f;
            float portalFading = Utils.GetLerpValue(75, 0, stuckTimer, true);

            Vector2 generalDrawPos = Projectile.Center - Main.screenPosition;
            Vector2 portalDrawPos = portalSpot - Main.screenPosition;

            if (jitter && stuckInTarget)
            {
                Main.EntitySpriteDraw(tex2.Value, generalDrawPos + (shadowPlacement * (1 - fading)), null, Color.Black * alpha, Projectile.rotation, tex2.Size() * 0.5f, Projectile.scale, storedVelocity.X > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically);
                Main.EntitySpriteDraw(tex2.Value, generalDrawPos + (-shadowPlacement * (1 - fading)), null, Color.Black * alpha, Projectile.rotation, tex2.Size() * 0.5f, Projectile.scale, storedVelocity.X > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically);

                Main.EntitySpriteDraw(portal.Value, portalDrawPos, null, Color.Black * (1 - fading * 0.3f), 0, portal.Size() * 0.5f, 1.8f * portalFading, SpriteEffects.None);
                Main.EntitySpriteDraw(portal.Value, portalDrawPos, null, Color.LightGreen with { A = 0 } * (1 - fading), 0, portal.Size() * 0.5f, 1.1f * portalFading, SpriteEffects.None);

                // 15NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
                Vector2 expectedVel = ((closestTarget == null ? Owner.Calamity().mouseWorld : closestTarget.Center) - portalSpot).SafeNormalize(Vector2.UnitX);
                if (true)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Main.EntitySpriteDraw(tex2.Value, portalDrawPos, null, Color.LightGreen with { A = 0 } * portalFading * 0.7f, expectedVel.ToRotation() + MathHelper.ToRadians(45f * (expectedVel.X > 0 ? 1 : -1)), tex2.Size() * 0.5f, (1 - (i * 0.05f)) * (1 - fading), expectedVel.X > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically);
                    }
                }
            }
            else if (stuckInTarget)
                Main.EntitySpriteDraw(portal.Value, portalDrawPos, null, Color.Black * (1 - fading * 0.3f), 0, portal.Size() * 0.5f, 1.5f * portalFading, SpriteEffects.None);

            if (!canStick || stuckInTarget)
            {
                for (int i = 0; i < 7; i++)
                {
                    Color auraColor = Color.LightGreen * (1 - fading);
                    Vector2 rotationalDrawOffset = (MathHelper.TwoPi * i / 7f).ToRotationVector2() * 3;
                    Main.EntitySpriteDraw(tex2.Value, Projectile.Center - Main.screenPosition + rotationalDrawOffset, null, (auraColor * alpha) with { A = 0 }, Projectile.rotation, tex2.Size() * 0.5f, Projectile.scale, storedVelocity.X > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically);
                }
            }

            Main.EntitySpriteDraw(tex.Value, generalDrawPos, null, lightColor * alpha * (stuckInTarget ? fading : 1), Projectile.rotation, tex.Size() * 0.5f, Projectile.scale, storedVelocity.X > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically);
            if (stuckInTarget)
            {
                Main.EntitySpriteDraw(tex2.Value, generalDrawPos, null, Color.LightGreen with { A = 0 } * alpha * (1 - fading) * 0.7f, Projectile.rotation, tex2.Size() * 0.5f, Projectile.scale, storedVelocity.X > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically);
                for (int i = 0; i < 3; i++)
                {
                    Main.EntitySpriteDraw(portal.Value, portalDrawPos, null, Color.LightGreen with { A = 0 } * (1 - fading * 0.3f) * 0.7f, 0, portal.Size() * 0.5f, (0.8f - (i * 0.15f)) * portalFading, SpriteEffects.None);
                }
            }
            return false;
        }
        public override bool? CanDamage() => canDamage ? null : false;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitX) * 68, Projectile.ai[1] == 1 ? 60 : 20, targetHitbox);
    }
}
