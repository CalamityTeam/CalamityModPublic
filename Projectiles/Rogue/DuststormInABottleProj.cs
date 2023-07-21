using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Items.Weapons.Rogue;
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

        public override void Kill(int timeLeft)
        {
            bool stealth = Projectile.Calamity().stealthStrike;
            SoundEngine.PlaySound(SoundID.Item107, Projectile.Center);
            double cloudAmt = Main.rand.Next(30, 50);
            if (stealth)
            {
                //DUST STORM
                for (int dustexplode = 0; dustexplode < 360; dustexplode++)
                {
                    Vector2 dustd = new Vector2(DuststormInABottle.DustRadius, DuststormInABottle.DustRadius).RotatedBy(MathHelper.ToRadians(dustexplode));
                    int d = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, Main.rand.NextBool(5) ? 32 : 85, dustd.X, dustd.Y, 50, default, 2f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].position = Projectile.Center;
                    Main.dust[d].velocity *= Main.rand.NextFloat(0.25f, 1f);
                }
                cloudAmt *= DuststormInABottle.StealthCloudAmountMult;
                cloudAmt = Math.Round(cloudAmt);
            }
            if (Projectile.owner == Main.myPlayer)
            {
                for (int index = 0; index < cloudAmt; index++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 10f, 200f, 0.01f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, stealth ? velocity*1.1f : velocity, ModContent.ProjectileType<DuststormCloud>(), 0, 0, Projectile.owner, stealth ? 1f : 0f, (float)Main.rand.Next(-45, 1));
                }
                int hitbox = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<DuststormCloudHitbox>(), Projectile.damage, Projectile.knockBack * 0.5f, Projectile.owner);
                if (hitbox.WithinBounds(Main.maxProjectiles) && Projectile.Calamity().stealthStrike) //Inherit stealth flag and less iframes
                {
                    Main.projectile[hitbox].ai[1] = 1;
                    Main.projectile[hitbox].localNPCHitCooldown = DuststormInABottle.StealthIframes;
                }
            }
        }
    }
}
