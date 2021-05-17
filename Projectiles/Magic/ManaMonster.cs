using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class ManaMonster : ModProjectile
    {
        public ref float Time => ref projectile.ai[0];
        public Player Target => Main.player[projectile.owner];
        public const int NPCAttackTime = 150;
        public const int PlayerAttackRedirectTime = 45;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Monster");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 52;
            projectile.friendly = true;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 300;
            projectile.Opacity = 0f;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 5;
            projectile.magic = true;
        }

        public override void AI()
        {
            if (Time < NPCAttackTime)
            {
                projectile.Opacity = Utils.InverseLerp(0f, 15f, Time, true);
                NPC potentialTarget = projectile.Center.ClosestNPCAt(1600f);
                if (potentialTarget != null)
                    AttackNPCTarget(potentialTarget);
                else if (projectile.velocity.Length() < 22f)
                    projectile.velocity *= 1.02f;
            }
            else
            {
                // Make an alert sound to indicate that the monster has become enraged and will now attack its caster.
                if (Time == NPCAttackTime)
                {
                    int ambientNoiseID = Main.rand.Next(39, 41 + 1);
                    Main.PlaySound(SoundID.Zombie, Target.Center, ambientNoiseID);
                    Main.PlaySound(SoundID.DD2_DrakinShot, Target.Center);
                    CreateTransitionBurstDust();
                }

                if (Time < NPCAttackTime + PlayerAttackRedirectTime)
                {
                    float idealMovementDirection = projectile.AngleTo(Target.Center);
                    float angularTurnSpeed = 0.09f;
                    float newSpeed = MathHelper.Lerp(projectile.velocity.Length(), 14f, 0.1f);
                    projectile.velocity = projectile.velocity.ToRotation().AngleTowards(idealMovementDirection, angularTurnSpeed).ToRotationVector2() * newSpeed;
                }
                else if (projectile.velocity.Length() < 34f)
                    projectile.velocity *= 1.02f;

                // Fade out.
                projectile.Opacity = Utils.InverseLerp(0f, 15f, projectile.timeLeft, true);
            }

            projectile.rotation = projectile.velocity.ToRotation() - MathHelper.PiOver2;
            Time++;
        }

        public void AttackNPCTarget(NPC target)
        {
            if (!projectile.WithinRange(target.Center, 60f))
            {
                float idealMovementDirection = projectile.AngleTo(target.Center);
                float angularTurnSpeed = 0.18f;
                float newSpeed = MathHelper.Lerp(projectile.velocity.Length(), 21f, 0.115f);
                projectile.velocity = projectile.velocity.ToRotation().AngleTowards(idealMovementDirection, angularTurnSpeed).ToRotationVector2() * newSpeed;
            }
        }

        public void CreateTransitionBurstDust()
        {
            if (Main.dedServ)
                return;

            for (int i = 0; i < 75; i++)
            {
                Dust brimstone = Dust.NewDustPerfect(projectile.Center + Main.rand.NextVector2Square(-25f, 25f), (int)CalamityDusts.Brimstone);
                brimstone.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(2f, 3.1f) * MathHelper.Lerp(1f, 2.175f, i / 75f);
                brimstone.velocity = Vector2.Lerp(brimstone.velocity, -Vector2.UnitY * brimstone.velocity.Length(), 0.5f);
                brimstone.scale = MathHelper.Lerp(1f, 1.875f, i / 75f) * Main.rand.NextFloat(0.8f, 1f);
                brimstone.fadeIn = 0.4f;
                brimstone.noGravity = true;
            }
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            // Ensure damage is not absolutely obscene when hitting players.
            damage = (int)(damage * 0.0021);
        }

        public override bool CanDamage() => projectile.Opacity >= 1f;


        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
