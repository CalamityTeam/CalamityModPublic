using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class BelladonnaPetal : ModProjectile
    {
        public Player Owner => Main.player[Projectile.owner];
        
        public ref float AITimer => ref Projectile.ai[0]; // The timer for the AI to do it's actions.

        public ref float CheckForFiring => ref Projectile.ai[1]; // Check for when it's about to fire, so we can put one-time effects and sounds.

        public ref float GaveRandomAngle => ref Projectile.localAI[0]; // A check to give the projectile a random angle.

        public NPC targetFound; // A variable where the potential target will be written on.
        
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Belladonna Petal");
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 130;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {   
            if (GaveRandomAngle == 0f) // Just gives it a random angle once so the throws look more natural.
            {
                Projectile.rotation = Main.rand.NextFloat(0, MathHelper.TwoPi);
                GaveRandomAngle = 1f;
            }
            Lighting.AddLight(Projectile.Center, 0.5f, 1f, 0.3f); // Gives it a jungl-y green color.

            if (Owner.HasMinionAttackTargetNPC) // If the Owner has selected for a target manually, go for that one.
            {
                NPC manualTarget = Main.npc[Owner.MinionAttackTargetNPC];
                Behaviour(manualTarget);
            }
            else // If not, go for the closest one.
            {
                NPC potentialTarget = Projectile.Center.MinionHoming(1200f, Owner);
                Behaviour(potentialTarget);
            }
            
            AITimer++;
            AITimer = (AITimer > 60f) ? 60f : AITimer; // AITimer can reach only a maximun of 60.
        }

        #region Methods

        public void Behaviour(NPC target)
        {
            if (target != null && AITimer >= 60f) // If the target is found, and 1 second has passed, do effects and go to the target.
            {
                Projectile.alpha = 0;
                for (int i = 0; i < 5; i++)
                {
                    Dust gotTarget = Dust.NewDustPerfect(Projectile.Center, DustID.Grass);
                    gotTarget.velocity = Main.rand.NextVector2Circular(2f, 2f);
                    gotTarget.noGravity = true;
                }
                if (CheckForFiring == 0f) // Check for it so it doesn't update the velocity indefinetly, meaning it would be homing.
                {
                    Projectile.velocity = CalamityUtils.CalculatePredictiveAimToTarget(Projectile.Center, targetFound, 20f);
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                    SoundEngine.PlaySound(SoundID.Grass, Projectile.Center);
                    CheckForFiring = 1f;
                    Projectile.netUpdate= true;
                }
            }
            else if (target != null && AITimer < 60f) // If there's target, but 1 second hasn't passed, rotate to point at the target, while still having gravity.
            {
                Projectile.velocity.Y += 0.2f;
                Projectile.rotation = MathHelper.Lerp(Projectile.rotation, (target.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2, AITimer / 60f);
                // (AITimer / 60f) because 60 is maximum time of the timer.
                targetFound = target;
                // Puts the potentialTarget on this variable that won't update constantly so the projectile doesn't become incredibly homing.
                Projectile.netUpdate = true;
            }
            else // If there's no target when shot (For example when the enemy has been killed), just fall.
            {
                AITimer = 0f; // Restart the timer, just so if a target appears again it looks smoother.
                Projectile.alpha += 2;
                Projectile.velocity.Y += 0.2f;
                Projectile.rotation += 0.05f; // Continues spinning until it dies.
                Projectile.netUpdate = true;
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust projDeath = Dust.NewDustPerfect(Projectile.Center, DustID.Grass);
                projDeath.velocity = Main.rand.NextVector2Circular(2f, 2f);
                projDeath.noGravity = true;
            }
        }

        public override bool? CanDamage() // If the projectile is going to the target, do damage, if not, don't.
        {
            if (targetFound != null && AITimer >= 60f)
            {
                return null;
            }

            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override bool PreDraw(ref Color lightColor) // Code taken from ExampleMod to make the trail.
        {
            Main.instance.LoadProjectile(Projectile.type);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            if (CheckForFiring == 1f)
            {
                Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
                for (int k = 0; k < Projectile.oldPos.Length; k++)
                {
                    Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                    Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
                }
            }
            return true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => target.AddBuff(BuffID.Poisoned, 240);

        #endregion
    }
}
