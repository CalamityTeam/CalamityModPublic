using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
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
            for (int k = 0; k < 15; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 85, Projectile.oldVelocity.X, Projectile.oldVelocity.Y);
            }
            double cloudAmt = Main.rand.Next(20, 35);
            if (stealth)
            {
                cloudAmt *= 1.5;
                cloudAmt = Math.Round(cloudAmt);
            }
            if (Projectile.owner == Main.myPlayer)
            {
                for (int index = 0; index < cloudAmt; index++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 10f, 200f, 0.01f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<DuststormCloud>(), 0, 0, Projectile.owner, stealth ? 1f : 0f, (float)Main.rand.Next(-45, 1));
                    int hitbox = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<DuststormCloudHitbox>(), 0, Projectile.knockBack * 0.5f, Projectile.owner);
                    if (hitbox.WithinBounds(Main.maxProjectiles) && Projectile.Calamity().stealthStrike) //Inherit stealth flag and less iframes
                        Main.projectile[hitbox].Calamity().stealthStrike = true;
                        Main.projectile[hitbox].idStaticNPCHitCooldown = 10;
                }

            }
        }
    }
}
