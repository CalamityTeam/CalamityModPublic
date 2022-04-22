using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Rogue
{
    public class DuststormInABottleProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/DuststormInABottle";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Duststorm");
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.aiStyle = 2;
            Projectile.timeLeft = 180;
            AIType = ProjectileID.ThrowingKnife;
            Projectile.Calamity().rogue = true;
        }

        public override void Kill(int timeLeft)
        {
            bool stealth = Projectile.Calamity().stealthStrike;
            SoundEngine.PlaySound(SoundID.Item107, Projectile.Center);
            for (int k = 0; k < 15; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 85, Projectile.oldVelocity.X, Projectile.oldVelocity.Y);
            }
            int cloudAmt = Main.rand.Next(20, 31);
            if (stealth)
                cloudAmt *= 2;
            int projType = stealth ? ModContent.ProjectileType<DuststormCloudStealth>() : ModContent.ProjectileType<DuststormCloud>();
            if (Projectile.owner == Main.myPlayer)
            {
                for (int index = 0; index < cloudAmt; index++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 10f, 200f, 0.01f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, projType, Projectile.damage, Projectile.knockBack * 0.5f, Projectile.owner, stealth ? 1f : 0f, (float)Main.rand.Next(-45, 1));
                }
            }
        }
    }
}
