using System;
using CalamityMod.Buffs.Summon;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class BelladonnaSpirit : BaseMinionProjectile
    {
        public override int AssociatedProjectileTypeID => ModContent.ProjectileType<BelladonnaSpirit>();
        public override int AssociatedBuffTypeID => ModContent.BuffType<BelladonnaSpiritBuff>();
        public override ref bool AssociatedMinionBool => ref ModdedOwner.belladonaSpirit;
        public override bool PreHardmodeMinionTileVision => true;
        public override int AnimationFrames => 5;
        public ref float ShootingTimer => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 28;
            Projectile.height = 48;
        }

        public override void MinionAI()
        {
            FollowPlayer();
            if (Target != null)
                AttackTarget();

            Projectile.MinionAntiClump();
            Projectile.spriteDirection = Target == null ? MathF.Sign(Projectile.velocity.X) : MathF.Sign(Target.Center.X - Projectile.Center.X);

            Projectile.netUpdate = true;
            if (Projectile.netSpam >= 10)
                Projectile.netSpam = 9;
        }

        #region AI Methods

        public void FollowPlayer()
        {
            // If the minion starts to get far, force the minion to go to you.
            if (Projectile.WithinRange(Owner.Center, EnemyDistanceDetection) && !Projectile.WithinRange(Owner.Center, 300f))
                Projectile.velocity = (Owner.Center - Projectile.Center) / 30f;

            // The minion will change directions to you if it's going away from you, meaning it'll just hover around you.
            else if (!Projectile.WithinRange(Owner.Center, 160f))
                Projectile.velocity = (Projectile.velocity * 37f + Projectile.SafeDirectionTo(Owner.Center) * 17f) / 40f;

            // Teleport to the owner if sufficiently far away.
            if (!Projectile.WithinRange(Owner.Center, EnemyDistanceDetection))
            {
                Projectile.position = Owner.Center;
                Projectile.velocity *= 0.3f;
            }
        }

        public void AttackTarget()
        {
            // The minion will slowly go up until it throws the petal.
            // This essentially makes the minion stay above you and trigger the "Turn back to player", it'll do this continuously, giving the effect of bouncing.
            // Yes, it's very wacky. You can make it better if you wish.
            Projectile.velocity.Y -= MathHelper.Lerp(0, 0.005f, ShootingTimer % BelladonnaSpiritStaff.FireRate);

            ShootingTimer++;
            if (ShootingTimer == BelladonnaSpiritStaff.FireRate && Main.myPlayer == Projectile.owner) // Every 75 frames, throws a petal.
            {
                // Throws the petal upwards with a random force and inherits the minion's speed.
                Vector2 petalShootVelocity = (-Vector2.UnitY * Main.rand.NextFloat(7f, 8.5f)) +
                    Vector2.UnitX * Projectile.velocity.X +
                    Vector2.UnitY * Projectile.velocity.Y * 0.35f;

                Projectile petal = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    petalShootVelocity,
                    ModContent.ProjectileType<BelladonnaPetal>(),
                    Projectile.damage,
                    Projectile.knockBack,
                    Projectile.owner);
                petal.rotation = Main.rand.NextFloat(0, MathHelper.TwoPi);

                // Resets the timer.
                ShootingTimer = 0f;
            }
        }

        #endregion

        public override void OnSpawn(IEntitySource source)
        {
            if (!Main.dedServ)
            {
                int dustAmount = 45;
                for (int dustIndex = 0; dustIndex < dustAmount; dustIndex++)
                {
                    float angle = MathHelper.TwoPi / 45f * dustIndex;
                    Vector2 velocity = angle.ToRotationVector2() * Main.rand.NextFloat(3f, 4.5f);
                    Dust spawnDust = Dust.NewDustPerfect(Projectile.Center, 39, velocity);
                    spawnDust.noGravity = true;
                    spawnDust.scale = velocity.Length() * 0.1f;
                    spawnDust.velocity *= 0.3f;
                }
            }
        }

        public override bool? CanDamage() => false;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 position = Projectile.Center - Main.screenPosition;
            Rectangle sourceRectangle = texture.Frame(verticalFrames: AnimationFrames, frameY: Projectile.frame);
            Vector2 origin = sourceRectangle.Size() * 0.5f;
            SpriteEffects effects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Main.EntitySpriteDraw(texture, position, sourceRectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, effects);

            return false;
        }
    }
}
