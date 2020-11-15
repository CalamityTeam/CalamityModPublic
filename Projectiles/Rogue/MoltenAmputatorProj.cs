using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class MoltenAmputatorProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/MoltenAmputator";

        private int stealthyBlobTimer = 6;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Amputator");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 74;
            projectile.height = 74;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.aiStyle = 3;
            projectile.timeLeft = 180;
            aiType = ProjectileID.WoodenBoomerang;
            projectile.Calamity().rogue = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 2);
            return false;
        }

        private void fireInTheBlob(int blobCount)
        {
            for (int i = 0; i < blobCount; i++)
            {
                Vector2 iAmSpeed = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                while (iAmSpeed.X == 0f && iAmSpeed.Y == 0f)
                {
                    iAmSpeed = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                }
                iAmSpeed.Normalize();
                iAmSpeed *= (float)Main.rand.Next(70, 101) * 0.1f;
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, iAmSpeed.X, iAmSpeed.Y, ModContent.ProjectileType<MoltenBlobThrown>(), (int)(projectile.damage * 0.25), 0f, projectile.owner, 0f, 0f);
            }
        }

        public override void AI()
        {
            if (projectile.Calamity().stealthStrike)
            {
                stealthyBlobTimer--;
                if (stealthyBlobTimer <= 0 && projectile.timeLeft % 2 == 0)
                {
                    fireInTheBlob(1);
                    stealthyBlobTimer = Main.rand.Next(4, 10);
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 4;
            if (projectile.owner == Main.myPlayer)
            {
                fireInTheBlob(projectile.Calamity().stealthStrike ? Main.rand.Next(3, 5) : Main.rand.Next(1, 3));
            }
            Main.PlaySound(SoundID.Item20, projectile.position);
            for (int k = 0; k < 10; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 244, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            if (projectile.owner == Main.myPlayer)
            {
                fireInTheBlob(projectile.Calamity().stealthStrike ? Main.rand.Next(3, 5) : Main.rand.Next(1, 3));
            }
            Main.PlaySound(SoundID.Item20, projectile.position);
            for (int k = 0; k < 10; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 244, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}
