using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
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
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.aiStyle = 2;
            projectile.timeLeft = 180;
            aiType = ProjectileID.ThrowingKnife;
            projectile.Calamity().rogue = true;
        }

        public override void Kill(int timeLeft)
        {
            bool stealth = projectile.Calamity().stealthStrike;
            Main.PlaySound(SoundID.Item107, projectile.Center);
            for (int k = 0; k < 15; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 85, projectile.oldVelocity.X, projectile.oldVelocity.Y);
            }
            int cloudAmt = Main.rand.Next(20, 31);
            if (stealth)
                cloudAmt *= 2;
            int projType = stealth ? ModContent.ProjectileType<DuststormCloudStealth>() : ModContent.ProjectileType<DuststormCloud>();
            if (projectile.owner == Main.myPlayer)
            {
                for (int index = 0; index < cloudAmt; index++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 10f, 200f, 0.01f);
                    Projectile.NewProjectile(projectile.Center, velocity, projType, projectile.damage, projectile.knockBack * 0.5f, projectile.owner, stealth ? 1f : 0f, (float)Main.rand.Next(-45, 1));
                }
            }
        }
    }
}
