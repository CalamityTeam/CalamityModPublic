using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class FrequencyManipulatorProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Misc";
        public override string Texture => "CalamityMod/Items/Weapons/DraedonsArsenal/FrequencyManipulator";

        public float Time
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public const int TotalSpins = 2;
        public const float SpinTime = 30f;
        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 180;
            Projectile.penetrate = 2;
            Projectile.extraUpdates = 1;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.tileCollide = false;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 45;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.Purple.ToVector3());

            Time++;
            Player player = Main.player[Projectile.owner];
            if (Time <= SpinTime)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.TwoPi * TotalSpins / SpinTime);
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
                Projectile.Center = player.itemLocation + Projectile.velocity;
                ManipulatePlayerItemValues(player);
            }
            else if (Time == SpinTime + 1f)
            {
                Projectile.tileCollide = true;
                RingDustEffect(player.itemLocation, 1f, 1f, 5f, 2f);
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX * player.direction) * player.ActiveItem().shootSpeed;
            }
        }

        public void ManipulatePlayerItemValues(Player player)
        {
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemRotation = Projectile.rotation;
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
            if (Main.myPlayer != Projectile.owner)
                return;
            // Explode if the projectile is a stealth strike projectile.
            int totalEnergyParticlesToSpawn = 3;
            if (Projectile.Calamity().stealthStrike)
            {
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
                totalEnergyParticlesToSpawn = 4;
                RingDustEffect(Projectile.Center, 1.6f, 1.4f, 7f, 2.6f);
                Projectile.ExpandHitboxBy(160);
                Projectile.damage /= 2;
                Projectile.Damage();
                Projectile.damage *= 2;
                Projectile.ExpandHitboxBy(34);
            }

            for (int i = 0; i < totalEnergyParticlesToSpawn; i++)
            {
                float offsetAngle = MathHelper.Lerp(-0.35f, 0.35f, i / (float)totalEnergyParticlesToSpawn);
                Vector2 velocity = -Projectile.oldVelocity.RotatedBy(offsetAngle) * 0.66f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + velocity * 1.8f, velocity, ModContent.ProjectileType<FrequencyManipulatorEnergy>(), Projectile.damage, Projectile.knockBack * 0.4f, Projectile.owner);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
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
