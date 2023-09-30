using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Particles;
namespace CalamityMod.Projectiles.Rogue
{
    public class DuststormInABottleProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/DuststormInABottle";

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.aiStyle = ProjAIStyleID.ThrownProjectile;
            Projectile.timeLeft = 180;
            AIType = ProjectileID.ThrowingKnife;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void OnKill(int timeLeft)
        {
            bool stealth = Projectile.Calamity().stealthStrike;
            SoundEngine.PlaySound(SoundID.Item107, Projectile.Center);
            double cloudAmt = Main.rand.Next(40, 50);
            if (stealth)
            {
                //DUST STORM
                for (int dustexplode = 0; dustexplode < 180; dustexplode++)
                {
                    Vector2 dustd = new Vector2(DuststormInABottle.DustRadius, DuststormInABottle.DustRadius).RotatedBy(MathHelper.ToRadians(dustexplode*2));

                    Particle dust = new SandyDustParticle(Projectile.Center, dustd*Main.rand.NextFloat(0.25f, 1f), Color.Beige, Main.rand.NextFloat(0.7f, 1.2f), Main.rand.Next(30, 50));
                    GeneralParticleHandler.SpawnParticle(dust);

                    int d = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, Main.rand.NextBool(5) ? 32 : 85, dustd.X, dustd.Y, 50, default, 2f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= Main.rand.NextFloat(0.25f, 1f);
                    Main.dust[d].scale *= Main.rand.NextFloat(0.5f, 0.8f);
                }
                cloudAmt *= 2.75;
                cloudAmt = Math.Round(cloudAmt);
            } else
            {
                //DUST STORM but smaller
                for (int dustexplode = 0; dustexplode < 120; dustexplode++)
                {
                    Vector2 dustd = new Vector2(DuststormInABottle.DustRadius, DuststormInABottle.DustRadius - 2).RotatedBy(MathHelper.ToRadians(dustexplode * 3));

                    Particle dust = new SandyDustParticle(Projectile.Center, dustd * Main.rand.NextFloat(0.25f, 1f), Color.Beige, Main.rand.NextFloat(0.7f, 1.2f), Main.rand.Next(30, 50));
                    GeneralParticleHandler.SpawnParticle(dust);
                }
            }
            if (Projectile.owner == Main.myPlayer)
            {
                for (int index = 0; index < cloudAmt; index++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 10f, 200f, 0.01f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, stealth ? velocity*1.2f : velocity, ModContent.ProjectileType<DuststormCloud>(), 0, 0, Projectile.owner, stealth ? 1f : 0f, (float)Main.rand.Next(-45, 1));
                }
                int hitbox = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<DuststormCloudHitbox>(), Projectile.damage, Projectile.knockBack * 0.5f, Projectile.owner);
                if (hitbox.WithinBounds(Main.maxProjectiles) && Projectile.Calamity().stealthStrike) //Inherit stealth flag and less iframes
                {
                    Main.projectile[hitbox].ai[1] = 1;
                    Main.projectile[hitbox].idStaticNPCHitCooldown = DuststormInABottle.StealthIframes;
                }
            }
        }
    }
}
