using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class FrequencyManipulatorProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/DraedonsArsenal/FrequencyManipulator";

        public float Time
        {
            get => projectile.ai[0];
            set => projectile.ai[0] = value;
        }
        public const int TotalSpins = 2;
        public const float SpinTime = 30f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frequency Manipulator");
        }

        public override void SetDefaults()
        {
            projectile.width = 34;
            projectile.height = 34;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 180;
            projectile.penetrate = 4;
            projectile.extraUpdates = 1;
            projectile.Calamity().rogue = true;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, Color.Purple.ToVector3());

            Time++;
            Player player = Main.player[projectile.owner];
            if (Time <= SpinTime)
            {
                projectile.velocity = projectile.velocity.RotatedBy(MathHelper.TwoPi * TotalSpins / SpinTime);
                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver4;
                projectile.Center = player.itemLocation + projectile.velocity;
                ManipulatePlayerItemValues(player);
            }
            else if (Time == SpinTime + 1f)
            {
                projectile.tileCollide = true;
                RingDustEffect(player.itemLocation, 1f, 1f, 5f, 2f);
                projectile.velocity = projectile.velocity.SafeNormalize(Vector2.UnitX * player.direction) * player.ActiveItem().shootSpeed;
            }
        }

        public void ManipulatePlayerItemValues(Player player)
        {
            player.direction = projectile.direction;
            player.heldProj = projectile.whoAmI;
            player.itemRotation = projectile.rotation;
            player.itemTime = player.itemAnimation;
        }

        public void RingDustEffect(Vector2 spawnPosition, float scale, float fadeIn, float ringSpeed, float secondaryRingSpeedMultiplier)
        {
            if (Main.dedServ)
                return;
            for (int i = 0; i < 36; i++)
            {
                float angle = MathHelper.TwoPi * i / 36f;
                Dust dust = Dust.NewDustPerfect(spawnPosition, 234);
                dust.scale = scale;
                dust.fadeIn = scale;
                dust.velocity = angle.ToRotationVector2() * ringSpeed;

                dust = Dust.NewDustPerfect(spawnPosition, 173);
                dust.scale = scale;
                dust.fadeIn = scale;
                dust.velocity = angle.ToRotationVector2() * ringSpeed * secondaryRingSpeedMultiplier;
            }
        }

        public void OnHitEffects()
        {
            if (Main.myPlayer != projectile.owner)
                return;
            // Explode if the projectile is a stealth strike projectile.
            int totalEnergyParticlesToSpawn = 4;
            if (projectile.Calamity().stealthStrike)
            {
                Main.PlaySound(SoundID.Item14, projectile.Center);
                totalEnergyParticlesToSpawn = 4;
                RingDustEffect(projectile.Center, 1.6f, 1.4f, 7f, 2.6f);
                CalamityGlobalProjectile.ExpandHitboxBy(projectile, 160);
                projectile.damage /= 2;
                projectile.Damage();
                projectile.damage *= 2;
                CalamityGlobalProjectile.ExpandHitboxBy(projectile, 34);
            }

            for (int i = 0; i < totalEnergyParticlesToSpawn; i++)
            {
                float offsetAngle = MathHelper.Lerp(-0.35f, 0.35f, i / (float)totalEnergyParticlesToSpawn);
                Vector2 velocity = -projectile.oldVelocity.RotatedBy(offsetAngle) * 0.66f;
                Projectile.NewProjectile(projectile.Center + velocity * 1.8f, velocity, ModContent.ProjectileType<FrequencyManipulatorEnergy>(), projectile.damage, projectile.knockBack * 0.4f, projectile.owner);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            OnHitEffects();
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            OnHitEffects();
            return true;
        }
    }
}
