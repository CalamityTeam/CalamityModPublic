using System;
using CalamityMod.Enums;
using CalamityMod.Particles;
using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Achievements;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class RespiteblockHoldout : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public Player Owner => Main.player[Projectile.owner];
        public ref float Time => ref Projectile.ai[0];

        public override void SetStaticDefaults() => Main.projFrames[Type] = 4;

        public override void SetDefaults()
        {
            Projectile.width = 108;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ownerHitCheck = true;
            // No reason to ID-static the chainsaw -- multiple players can true melee simultaneously!
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 4;
            Projectile.noEnchantmentVisuals = true;
            Projectile.ArmorPenetration = 15;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            SpriteEffects direction = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;

            Main.EntitySpriteDraw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, direction, 0);
            return false;
        }

        public override void AI()
        {
            // Recalculate damage every frame for balance reasons, as this is a long-lasting holdout.
            // This is important because you could start using it while benefitting from Auric Tesla standstill bonus, for example.
            Projectile.damage = Owner.ActiveItem() is null ? 0 : Owner.GetWeaponDamage(Owner.ActiveItem());

            PlayChainsawSounds();

            // Determines the owner's position whilst incorporating their fullRotation field.
            // It uses vector transformation on a Z rotation matrix based on said rotation under the hood.
            // This is essentially just the pure mathematical definition of the RotatedBy method.
            Vector2 playerRotatedPosition = Owner.RotatedRelativePoint(Owner.MountedCenter);
            if (Main.myPlayer == Projectile.owner)
            {
                if (!Owner.CantUseHoldout() && Owner.active && !Owner.dead)
                    HandleChannelMovement(playerRotatedPosition);
                else
                    Projectile.Kill();
            }

            Lighting.AddLight(Projectile.Center, Color.White.ToVector3());

            for (int i = 0; i <= 3; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + (Projectile.velocity.RotatedBy(MathHelper.ToRadians(45f * Projectile.direction)) * 10) + Main.rand.NextVector2Circular(13, 13), Main.rand.NextBool() ? 89 : 86, (Projectile.velocity * 30).RotatedByRandom(0.3f) * Main.rand.NextFloat(0.05f, 0.9f), 0, default, Main.rand.NextFloat(0.4f, 0.95f));
                dust.noGravity = true;
                dust.noLight = true;
                dust.noLightEmittence = true;
                dust.alpha = 90;
            }
            if (Time % 4 == 0)
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + (Projectile.velocity.RotatedBy(MathHelper.ToRadians(45f * Projectile.direction)) * 10) + Main.rand.NextVector2Circular(13, 13) + Projectile.velocity * Main.rand.Next(10, 20 + 1), Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.01f, -0.25f) * Projectile.direction) * 4, ModContent.ProjectileType<RespiteblockBlood>(), (int)(Projectile.damage * 0.4f), Projectile.knockBack, Projectile.owner, 0);
            }

            DetermineVisuals(playerRotatedPosition);
            ManipulatePlayerValues();

            // Prevent the projectile from dying normally. However, if anything for whatever reason
            // goes wrong it will immediately be destroyed on the next frame.
            Projectile.timeLeft = 2;

            Time++;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<BurningBlood>(), 500);

            Player player = Main.player[Projectile.owner];
            Vector2 spawnPos = Projectile.Center + (Projectile.velocity.RotatedBy(MathHelper.ToRadians(45f * Projectile.direction)) * 10) + Main.rand.NextVector2Circular(13, 13);
            for (int i = 0; i <= 3; i++)
            {
                spawnPos = Projectile.Center + (Projectile.velocity.RotatedBy(MathHelper.ToRadians(45f * Projectile.direction)) * 10) + Main.rand.NextVector2Circular(13, 13);
                Particle blood = new PointParticle(spawnPos, (Projectile.velocity * 30).RotatedByRandom(0.3f) * Main.rand.NextFloat(0.2f, 0.9f) + new Vector2(0, Main.rand.NextFloat(-7, -2 + 1)), true, 25, Main.rand.NextFloat(0.7f, 1.2f), (Main.rand.NextBool() ? Color.Green : Color.Purple) * 0.5f, false);
                GeneralParticleHandler.SpawnParticle(blood);

                Dust dust = Dust.NewDustPerfect(spawnPos, 263, (Projectile.velocity * 50).RotatedByRandom(0.3f) * Main.rand.NextFloat(0.05f, 0.9f), 0, default, Main.rand.NextFloat(0.6f, 0.95f));
                dust.noGravity = true;
                dust.color = Color.White;
            }

            SoundStyle fire = new("CalamityMod/Sounds/Item/WulfrumKnifeTileHit", 2);
            SoundEngine.PlaySound(fire with { Volume = 0.55f, Pitch = 0.3f }, Projectile.Center);

            if (player.moonLeech || player.lifeSteal <= 0f || target.lifeMax <= 5)
                return;

            int heal = Main.rand.NextBool(4) ? 2 : 1;
            player.lifeSteal -= heal;
            player.HealPlayer(heal);
        }

        public void PlayChainsawSounds()
        {
            if (Projectile.soundDelay <= 0)
            {
                SoundEngine.PlaySound(SoundID.Item22 with { Pitch = 0.3f}, Projectile.Center);
                Projectile.soundDelay = 6;
            }
        }
        public void DetermineVisuals(Vector2 playerRotatedPosition)
        {
            float directionAngle = Projectile.velocity.ToRotation();
            Projectile.rotation = directionAngle;

            int oldDirection = Projectile.spriteDirection;
            if (oldDirection == -1)
                Projectile.rotation += MathHelper.Pi;

            Projectile.direction = Projectile.spriteDirection = (Math.Cos(directionAngle) > 0).ToDirectionInt();

            // If the direction differs from what it originaly was, undo the previous 180 degree turn.
            // If this is not done, the chainsaw will have 1 frame of rotational "jitter" when the direction changes based on the
            // original angle. This effect looks very strange in-game.
            if (Projectile.spriteDirection != oldDirection)
                Projectile.rotation -= MathHelper.Pi;

            // Positioning close to the player's arm.
            Projectile.position = playerRotatedPosition - Projectile.Size * 0.5f + (directionAngle.ToRotationVector2().RotatedBy(MathHelper.ToRadians(-45f * Projectile.direction)) * 10f) + directionAngle.ToRotationVector2() * 30;

            // Update the position a tiny bit every frame at random to make it look like the saw is vibrating.
            // It is reset on the next frame.
            Projectile.position += Main.rand.NextVector2Circular(1.4f, 1.4f);

            Projectile.frameCounter += 33;
            if (Projectile.frameCounter >= 32)
            {
                Projectile.frame = (Projectile.frame + 1) % 4;
                Projectile.frameCounter = 0;
            }
        }

        public void HandleChannelMovement(Vector2 playerRotatedPosition)
        {
            Vector2 idealAimDirection = (Main.MouseWorld - playerRotatedPosition).SafeNormalize(Vector2.UnitX * Owner.direction);

            float angularAimVelocity = 0.15f;
            float directionAngularDisparity = Projectile.velocity.AngleBetween(idealAimDirection) / MathHelper.Pi;

            // Increase the turn speed if close to the ideal direction, since successive linear interpolations
            // are asymptotic.
            angularAimVelocity += MathHelper.Lerp(0f, 0.25f, Utils.GetLerpValue(0.28f, 0.08f, directionAngularDisparity, true));

            if (directionAngularDisparity > 0.02f)
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, idealAimDirection, angularAimVelocity);
            else
                Projectile.velocity = idealAimDirection;

            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX * Owner.direction);
        }

        public void ManipulatePlayerValues()
        {
            Owner.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.ChangeDir(Projectile.direction);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // Collision is done as a line to bypass the fact that hitboxes cannot rotate and that
            // this projectile is notably flat in terms of sprite shape.
            float _ = 0f;
            float width = Projectile.scale * 36f;
            Vector2 start = Projectile.Center;
            Vector2 end = Projectile.Center + Projectile.velocity * 60f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, width, ref _);
        }
    }
}
