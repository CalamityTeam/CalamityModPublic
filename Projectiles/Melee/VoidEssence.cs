using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class VoidEssence : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        private const int NumAnimationFrames = 4;
        private const int AnimationFrameTime = 12;
        private const float TentacleRange = 140f;
        private const float TentacleCooldown = 25f;
        public bool StartFading = false;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = NumAnimationFrames;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.height = 24;
            Projectile.width = 24;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 180;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 80;

            Projectile.penetrate = 4;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 4;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            DrawOffsetX = 1;
            DrawOriginOffsetY = 4;

            // Update animation
            Projectile.frameCounter++;
            if (Projectile.frameCounter > AnimationFrameTime)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= NumAnimationFrames)
                Projectile.frame = 0;

            // Produce light
            Lighting.AddLight(Projectile.Center, 0.9f, 0.9f, 1.0f);

            // Continuously trail dust
            int trailDust = 1;
            for (int i = 0; i < trailDust; ++i)
            {
                int dustID = Main.rand.NextBool(8) ? 66 : 143;

                int idx = Dust.NewDust(Projectile.position - Projectile.velocity, Projectile.width, Projectile.height, dustID);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity += Projectile.velocity * 0.8f;
            }

            // If tentacle is currently on cooldown, reduce the cooldown.
            if (Projectile.ai[0] > 0f)
                Projectile.ai[0] -= 1f;

            // Home in on nearby enemies if homing is enabled
            if (Projectile.ai[1] == 0f)
                HomingAI();

            // Fade-out.
            if (StartFading)
                Projectile.alpha += Nadir.FadeoutSpeed;
        }

        private void HomingAI()
        {
            // Find the closest NPC within range.
            int targetIdx = -1;
            float maxHomingRange = 400f;
            bool hasHomingTarget = false;
            for (int i = 0; i < Main.npc.Length; ++i)
            {
                NPC npc = Main.npc[i];
                if (npc == null || !npc.active)
                    continue;

                // Won't home in through walls and won't chase invulnerable targets.
                if (npc.CanBeChasedBy(Projectile, false) && Collision.CanHit(Projectile.Center, 1, 1, npc.Center, 1, 1))
                {
                    float dist = (Projectile.Center - npc.Center).Length();
                    if (dist < maxHomingRange)
                    {
                        targetIdx = i;
                        maxHomingRange = dist;
                        hasHomingTarget = true;
                    }
                }
            }

            // Home in on said closest NPC.
            if (hasHomingTarget)
            {
                NPC target = Main.npc[targetIdx];
                Vector2 homingVector = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * Nadir.ProjShootSpeed;
                float homingRatio = 35f;
                Projectile.velocity = (Projectile.velocity * homingRatio + homingVector) / (homingRatio + 1f);

                // If the target is close enough and tentacle is off cooldown, summon one.
                // maxHomingRange doubles as the distance to the target.
                if (Projectile.ai[0] <= 0f && maxHomingRange <= TentacleRange)
                {
                    Vector2 projVel = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                    projVel *= 6f;
                    SpawnTentacle(projVel);
                    Projectile.ai[0] = TentacleCooldown;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Rapidly screech to a halt upon touching an enemy and disable homing.
            Projectile.velocity *= 0.4f;
            Projectile.ai[1] = 1f;

            // Start fading after hitting the target.
            StartFading = true;

            // Explode into dust (as if being shredded apart on contact)
            int onHitDust = Main.rand.Next(6, 11);
            for (int i = 0; i < onHitDust; ++i)
            {
                int dustID = Main.rand.NextBool() ? 198 : 199;
                int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustID, 0f, 0f);

                Main.dust[idx].noGravity = true;
                float speed = Main.rand.NextFloat(1.4f, 2.6f);
                Main.dust[idx].velocity *= speed;
                float scale = Main.rand.NextFloat(1.0f, 1.8f);
                Main.dust[idx].scale = scale;
            }
        }

        public override void OnKill(int timeLeft)
        {
            // Create a burst of dust
            int killDust = Main.rand.Next(30, 41);
            for (int i = 0; i < killDust; ++i)
            {
                int dustID = Main.rand.NextBool() ? 198 : 199;
                int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustID, 0f, 0f);

                Main.dust[idx].noGravity = true;
                float speed = Main.rand.NextFloat(2.0f, 3.1f);
                Main.dust[idx].velocity *= speed;
                float scale = Main.rand.NextFloat(1.0f, 1.8f);
                Main.dust[idx].scale = scale;
            }

            // Spawn three tentacles pointing in random directions
            for (int i = 0; i < 3; ++i)
            {
                Vector2 projVel = Vector2.One.RotatedByRandom(MathHelper.TwoPi);
                projVel *= 4f;
                SpawnTentacle(projVel);
            }
        }

        private void SpawnTentacle(Vector2 tentacleVelocity)
        {
            int damage = Projectile.damage;
            float kb = Projectile.knockBack;

            // Randomize tentacle behavior variables
            float ai0 = Main.rand.NextFloat(0.01f, 0.08f);
            ai0 *= Main.rand.NextBool() ? -1f : 1f;
            float ai1 = Main.rand.NextFloat(0.01f, 0.08f);
            ai1 *= Main.rand.NextBool() ? -1f : 1f;

            if (Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, tentacleVelocity, ModContent.ProjectileType<VoidTentacle>(), damage, kb, Projectile.owner, ai0, ai1);
        }
    }
}
