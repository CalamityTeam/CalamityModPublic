using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class VoltageStream : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Misc";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public float Time
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public NPC Target
        {
            get => Main.npc[(int)Projectile.ai[1]];
            set => Projectile.ai[1] = value.whoAmI;
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 180;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 40;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.SkyBlue.ToVector3());
            if (!Target.active)
            {
                Projectile.Kill();
                return;
            }
            Projectile.Center = Target.Center;
            if (Time < 90f)
            {
                float completionRatio = Utils.GetLerpValue(0f, 90f, Time, true);
                float offsetRatioOnSprite = (float)Math.Sin(completionRatio * MathHelper.ToRadians(720f)) * 0.5f + 0.5f;
                Vector2 dustInitialPosition = Vector2.Lerp(Target.Top, Target.Bottom, offsetRatioOnSprite);
                for (int i = 0; i < 4; i++)
                {
                    float angularOffset = MathHelper.TwoPi / 4f * i + Time / 90f * MathHelper.ToRadians(1080f);
                    Vector2 dustSpawnPosition = dustInitialPosition + angularOffset.ToRotationVector2() * 4f;
                    dustSpawnPosition.X += 10f * (float)Math.Sin(completionRatio * MathHelper.ToRadians(360f));

                    Dust dust = Dust.NewDustPerfect(dustSpawnPosition, 261);
                    dust.velocity = Vector2.Zero;
                    dust.noGravity = true;

                    dustSpawnPosition = dustInitialPosition + angularOffset.ToRotationVector2() * 4f;
                    dustSpawnPosition.X -= 10f * (float)Math.Sin(completionRatio * MathHelper.ToRadians(360f));
                    dust = Dust.NewDustPerfect(dustSpawnPosition, 261);
                    dust.velocity = Vector2.Zero;
                    dust.noGravity = true;
                }
            }
            else if (Time < 150f)
            {
                for (int i = 0; i < 50; i++)
                {
                    float angle = MathHelper.TwoPi / 50f * i + Utils.GetLerpValue(90f, 150f, Time, true) * MathHelper.ToRadians(1080f);
                    float radius = MathHelper.Lerp(0f, 25f, Utils.GetLerpValue(90f, 150f, Time, true));
                    Dust dust = Dust.NewDustPerfect(Target.Center + angle.ToRotationVector2() * radius, 226);
                    dust.velocity = Vector2.Zero;
                    if (Main.rand.NextBool(6))
                        dust.velocity = Target.SafeDirectionTo(dust.position) * 4.5f;

                    dust.noGravity = true;
                }
            }
            else
            {
                for (int i = 0; i < 120; i++)
                {
                    float angle = MathHelper.TwoPi / 120f * i;
                    Dust dust = Dust.NewDustPerfect(Target.Center + angle.ToRotationVector2() * 25f, 226);
                    dust.velocity = angle.ToRotationVector2() * Main.rand.NextFloat(2f, 9f) * Main.rand.NextBool().ToDirectionInt();
                    dust.velocity = dust.velocity.RotatedBy(dust.velocity.ToRotation() * -0.02f);
                    dust.velocity *= 2.1f;
                    dust.noGravity = true;
                }
                Target.AddBuff(BuffID.Electrified, 180);
                Projectile.Kill();
            }
            Time++;

            if (Projectile.damage <= 0)
                Projectile.Kill();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.75);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Projectile.damage = (int)(Projectile.damage * 0.75);
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.ai[1] == target.whoAmI)
                return null;
            return (bool?)false;
        }
    }
}
