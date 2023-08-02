using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;

namespace CalamityMod.Projectiles.Ranged
{
    public class PlagueTaintedProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 2;
            AIType = ProjectileID.Bullet;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.1f, 0.4f, 0f);
            Vector2 pos = Projectile.Center;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 1f)
            {
                if (Main.rand.NextBool(3))
                {
                    int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 89, 0f, 0f, 0, default, 0.8f);
                    Main.dust[dust].alpha = Projectile.alpha;
                    Main.dust[dust].velocity *= 0f;
                    Main.dust[dust].noGravity = true;
                }
            }
            else
            {
                // Create dust sprays above and below the barrel.
                for (int h = 0; h < 2; h++)
                {
                    bool top = h == 0;
                    int dustPerSpray = 5;
                    for (int i = 0; i < dustPerSpray; i++)
                    {
                        int dustID = 89;
                        float dustSpeed = i * 2f;
                        float angle = top ? -0.12f : 0.12f;
                        Vector2 dustVel = new Vector2(dustSpeed, 0f).RotatedBy(Projectile.velocity.ToRotation());
                        dustVel = dustVel.RotatedBy(angle);

                        // Pick a size for the smoke particle.
                        float scale = 1.2f - (i * 0.2f);

                        // Actually spawn the smoke.
                        int idx = Dust.NewDust(pos, 1, 1, dustID, dustVel.X, dustVel.Y, 0, default, scale);
                        Main.dust[idx].noGravity = true;
                        Main.dust[idx].position = pos;
                    }
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.White;
            CalamityUtils.DrawAfterimagesFromEdge(Projectile, 0, lightColor);
            return false;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float maxDamageScalingDistance = 800f;
            float maxDamageMultiplier = 1.5f;
            float distanceFromOwner = target.Distance(Main.player[Projectile.owner].Center);
            if (distanceFromOwner < maxDamageScalingDistance)
            {
                float amount = MathHelper.Clamp((maxDamageScalingDistance - distanceFromOwner) / maxDamageScalingDistance, 0f, 1f);
                modifiers.SourceDamage *= MathHelper.Lerp(1f, maxDamageMultiplier, amount);
                modifiers.Knockback *= MathHelper.Lerp(1f, maxDamageMultiplier, amount);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 120);

            // Spawn a drone on crit, with a cooldown...I guess.
            if (Projectile.owner == Main.myPlayer)
            {
                if (hit.Crit && Main.player[Projectile.owner].Calamity().plagueTaintedSMGDroneCooldown == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item61, Projectile.Center);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Main.player[Projectile.owner].Center, Main.rand.NextVector2CircularEdge(3f, 3f), ModContent.ProjectileType<PlagueTaintedDrone>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    Main.player[Projectile.owner].Calamity().plagueTaintedSMGDroneCooldown = 90;
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int k = 0; k < 5; k++)
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 89, Projectile.oldVelocity.X * 0.2f, Projectile.oldVelocity.Y * 0.2f, 0, default, 0.6f);
        }
    }
}
