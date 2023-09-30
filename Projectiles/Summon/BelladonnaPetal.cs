using CalamityMod.Items.Weapons.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class BelladonnaPetal : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public Player Owner => Main.player[Projectile.owner];

        // The timer for the AI to do it's actions.
        public ref float AITimer => ref Projectile.ai[0];

        // Check for when it's about to fire, so we can put one-time effects and sounds.
        public ref float CheckForFiring => ref Projectile.ai[1];

        // A variable where the potential target will be written on.
        public NPC targetFound;
        
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.timeLeft = 130;

            Projectile.width = Projectile.height = 14;

            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            // Detects a target at a given distance.
            NPC potentialTarget = Projectile.Center.MinionHoming(BelladonnaSpiritStaff.EnemyDistanceDetection, Owner);

            // It's behaviour depending on the target.
            Behaviour(potentialTarget);

            if (AITimer < BelladonnaSpiritStaff.PetalTimeBeforeTargetting)
                AITimer++;

            // Gives it a jungl-y green color.
            Lighting.AddLight(Projectile.Center, 0.5f, 1f, 0.3f);

            Projectile.netUpdate = true;
        }

        #region Methods

        public void Behaviour(NPC target)
        {
            // If the target is found, and 1 second has passed, go to the target.
            if (target != null && AITimer >= BelladonnaSpiritStaff.PetalTimeBeforeTargetting)
            {
                // A trail made of dust.
                for (int dustIndex = 0; dustIndex < 5; dustIndex++)
                {
                    float velModifier = 0.25f;
                    Dust.NewDust(Projectile.Center,
                        Projectile.width,
                        Projectile.height,
                        DustID.Grass,
                        -Projectile.velocity.X * velModifier,
                        -Projectile.velocity.Y * velModifier,
                        0, default, 0.5f);
                }

                // Check for it so it doesn't update the velocity indefinetly, meaning it would be homing.
                if (CheckForFiring == 0f) 
                {
                    Projectile.velocity = CalamityUtils.CalculatePredictiveAimToTarget(Projectile.Center, targetFound, BelladonnaSpiritStaff.PetalVelocity);

                    // The projectile will look towards where it's going.
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

                    SoundEngine.PlaySound(SoundID.Grass, Projectile.Center);
                    CheckForFiring = 1f;
                    Projectile.netUpdate= true;
                }

                // In case the shot was about to fade out, un-fade.
                Projectile.alpha = 0;
            }

            // If there's target, but 1 second hasn't passed, rotate to point at the target, while still having gravity.
            else if (target != null && AITimer < BelladonnaSpiritStaff.PetalTimeBeforeTargetting)
            {
                
                // "(AITimer / It's maxuimum value)" so it's a fraction between 0 and 1.
                Projectile.rotation = MathHelper.Lerp(Projectile.rotation,
                    (target.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2,
                    AITimer / BelladonnaSpiritStaff.PetalTimeBeforeTargetting);

                // Gravity.
                Projectile.velocity.Y += BelladonnaSpiritStaff.PetalGravityStrenght;

                // Puts the potentialTarget on this variable that won't update constantly so the projectile doesn't become incredibly homing.
                targetFound = target;
                
                Projectile.netUpdate = true;
            }

            // If there's no target when shot (For example when the enemy has been killed), just fall.
            else
            {
                // Restart the timer, just so if a target appears again it looks smoother.
                AITimer = 0f;

                // Starts to fade out.
                Projectile.alpha += 2;

                // Keeps falling.
                Projectile.velocity.Y += BelladonnaSpiritStaff.PetalGravityStrenght;

                // Continues spinning until it dies.
                Projectile.rotation += 0.05f;

                Projectile.netUpdate = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int dustIndex = 0; dustIndex < 5; dustIndex++)
                Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.Grass);
        }

        // If the projectile is going to the target, do damage, if not, don't.
        public override bool? CanDamage()
        {
            if (targetFound != null && AITimer >= BelladonnaSpiritStaff.PetalTimeBeforeTargetting)
                return null;
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override bool PreDraw(ref Color lightColor)
        {
            if (CheckForFiring == 1f)
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Poisoned, 240);

        #endregion
    }
}
