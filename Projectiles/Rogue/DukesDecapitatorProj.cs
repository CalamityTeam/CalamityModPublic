using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Projectiles.Rogue
{
    public class DukesDecapitatorProj : ModProjectile
    {
        float rotationAmount = 1.5f;
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/DukesDecapitator";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Decapitator");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 600;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 20;
            projectile.tileCollide = false;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            CalamityPlayer modPlayer = Main.player[projectile.owner].Calamity();
            if (projectile.velocity.X != 0 || projectile.velocity.Y != 0)
            {
                projectile.velocity *= 0.99f;
            }
            projectile.ai[0] += 1f;
            if (projectile.ai[0] == 5f)
                projectile.tileCollide = true;

            if ((projectile.ai[0] % 15f) == 0f && rotationAmount > 0)
            {
                rotationAmount -= 0.05f;
                if (projectile.Calamity().stealthStrike && projectile.owner == Main.myPlayer)
                {
                    float velocityX = Main.rand.NextFloat(-0.8f, 0.8f);
                    float velocityY = Main.rand.NextFloat(-0.8f, -0.8f);
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, velocityX, velocityY, ModContent.ProjectileType<DukesDecapitatorBubble>(), (int)(projectile.damage * 0.8), projectile.knockBack, projectile.owner);
                }
            }
            if (rotationAmount <= 0f)
                projectile.Kill();

            projectile.rotation += rotationAmount;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            projectile.velocity = Vector2.Zero;
            rotationAmount -= 0.05f;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 49, projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f, 0, new Color(255, 255, 255), 0.75f);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
