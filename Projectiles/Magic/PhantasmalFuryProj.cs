using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class PhantasmalFuryProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public int time = 0;
        public float dustRotation = 0;
        public bool launched = false;
        public NPC targeted;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 5;
            Projectile.timeLeft = 900;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 16 * Projectile.MaxUpdates;
            Projectile.ArmorPenetration = 15;
        }

        public override void AI()
        {
            Player Owner = Main.player[Projectile.owner];
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Type])
                Projectile.frame = 0;

            dustRotation += 0.12f;
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.5f);

            // 175, 100 ALPHA

            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();

            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);

            if (time >= 500)
            {
                if (time == 500)
                {
                    Projectile.penetrate = 1;
                    launched = true;
                }
                if (targeted == null || targeted.life <= 0)
                    targeted = Projectile.Center.ClosestNPCAt(950);
                CalamityUtils.HomeInOnSelectedNPC(Projectile, targeted, true, 0.15f, 6, 0.98f, accelerate: true);

                if (time < 550 && targeted == null)
                {
                    // 14NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
                    if (Projectile.velocity.Length() < 6)
                        Projectile.velocity += (Owner.Calamity().mouseWorld - Projectile.Center).SafeNormalize(Vector2.UnitX) * 0.35f;
                    else
                        Projectile.velocity *= 0.9f;
                }
            }
            else if (time > 15)
            {
                Vector2 circle = Owner.Center + new Vector2(0, -30).RotatedBy(time * 0.05f);
                Vector2 moveToEnemy = (circle - Projectile.Center).SafeNormalize(Vector2.UnitX);
                if (Projectile.velocity.Length() < 8)
                    Projectile.velocity += moveToEnemy * Main.rand.NextFloat(0.2f, 0.4f);
                else
                    Projectile.velocity *= 0.85f;
            }

            if (time > 5)
            {
                Vector2 dustPos = Projectile.Center + (MathHelper.Pi + dustRotation + MathHelper.PiOver2).ToRotationVector2() * 10f * Projectile.scale;
                Dust dust = Dust.NewDustPerfect(dustPos, DustID.SpectreStaff, (MathHelper.Pi + dustRotation * Math.Sign(Projectile.velocity.Length())).ToRotationVector2() * 2);
                dust.noGravity = false;
                dust.scale = Main.rand.NextFloat(0.75f, 1.2f);
                dust.alpha = Main.rand.Next(100, 170 + 1);
                dust.velocity = dust.velocity.RotatedByRandom(0f);
            }
            time++;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage *= (launched ? 1f : 0.3f);

            Player Owner = Main.player[Projectile.owner];

            Vector2 launchVel = (Owner.Center - target.Center).SafeNormalize(Vector2.UnitY) * -10 * (launched ? 0.5f : 1);
            target.MoveNPC(launchVel, 10 * (launched ? 0.5f : 1), true, Owner);
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (targeted != null)
                return (target == targeted ? null : false);
            else
                return null;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i <= 3; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.SpectreStaff, (Projectile.velocity * 6).RotatedByRandom(0.4f) * Main.rand.NextFloat(0.1f, 0.8f), 100, default, Main.rand.NextFloat(1.2f, 1.8f));
                dust.noGravity = true;
                Dust chargefull = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(4) ? 278 : 267);
                chargefull.velocity = Projectile.velocity.RotatedByRandom(0.25f) * Main.rand.NextFloat(1f, 4);
                chargefull.scale = Main.rand.NextFloat(0.5f, 0.9f);
                chargefull.noGravity = true;
                chargefull.color = Color.Lerp(Color.White, Color.Aqua, 0.3f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], Color.White, 1);

            return false;
        }
    }
}
