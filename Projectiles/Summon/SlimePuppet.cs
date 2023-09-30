using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Buffs.DamageOverTime;

namespace CalamityMod.Projectiles.Summon
{
    public class SlimePuppet : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public Player Owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 1;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.minionSlots = 0;
            Projectile.timeLeft = 300;
            Projectile.penetrate = 1;
            Projectile.minion = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            // Prevent clumping of other slime puppets near this one.
            Projectile.MinionAntiClump();

            NPC potentialTarget = Projectile.Center.MinionHoming(800f, Owner);

            if (potentialTarget is null)
            {
                // Movement above the player, depending on the index of the projectile in the Main.projectile array.
                float destinationAngularOffset = (float)Math.Sin(Projectile.identity / 6f % 6f * MathHelper.Pi) * MathHelper.Pi / 10f;
                Vector2 destination = Owner.Center - Vector2.UnitY.RotatedBy(destinationAngularOffset) * 160f;

                if (Projectile.DistanceSQ(destination) > 18f * 18f)
                    Projectile.velocity = (Projectile.velocity * 20f + Projectile.SafeDirectionTo(destination) * 9f) / 21f;
            }
            else
            {
                float nudgedVelocityDirection = Projectile.velocity.ToRotation().AngleTowards(Projectile.AngleTo(potentialTarget.Center), 0.078f);
                Projectile.velocity = nudgedVelocityDirection.ToRotationVector2() * MathHelper.Lerp(Projectile.velocity.Length(), 16f, 0.25f);
                if (Projectile.WithinRange(potentialTarget.Center, 240f))
                {
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(potentialTarget.Center) * 16f, 0.1f);
                    Projectile.Center = Projectile.Center.MoveTowards(potentialTarget.Center, 6f);
                }
            }
            if (Projectile.ai[0] > 0f)
            {
                Projectile.rotation += MathHelper.ToRadians(Projectile.velocity.Length() / 4f) * Math.Sign(Projectile.velocity.X);
                Projectile.ai[0]--;
            }
            else if (potentialTarget != null)
                Projectile.rotation = Projectile.velocity.ToRotation();

            if (Vector2.Dot(Projectile.oldVelocity.SafeNormalize(Vector2.Zero), Projectile.velocity.SafeNormalize(Vector2.Zero)) < 0.87)
                Projectile.ai[0] = 50f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Slimed, 300);

        public override void OnKill(int timeLeft)
        {
            Projectile.ExpandHitboxBy(100);
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();
            SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.Center);

            if (!Main.dedServ)
            {
                for (int i = 0; i < 20; i++)
                {
                    Vector2 spawnOffset = Main.rand.NextVector2Unit() * Main.rand.NextFloat(4f, 36f);
                    Dust slime = Dust.NewDustPerfect(Projectile.Center + spawnOffset, 243);
                    slime.velocity = spawnOffset.RotatedBy(MathHelper.PiOver2 * Main.rand.NextBool().ToDirectionInt()) * 0.16f;
                    slime.scale = 1.2f;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor);
            return false;
        }
    }
}
