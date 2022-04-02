using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Projectiles.Rogue
{
    public class CraniumSmasherStealth : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Rogue/CraniumSmasherExplosive";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stealthy Cranium Smasher");
        }

        public override void SetDefaults()
        {
            projectile.width = 50;
            projectile.height = 50;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 5;
            projectile.timeLeft = 300;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 15;
            projectile.tileCollide = false;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.ai[0] += 1f;
            if (projectile.ai[0] >= 5f)
            {
                projectile.tileCollide = true;
            }
            projectile.rotation += projectile.velocity.X * 0.02f;
            projectile.velocity.Y = projectile.velocity.Y + 0.085f;
            projectile.velocity.X = projectile.velocity.X * 0.99f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.owner == Main.myPlayer)
            {
                int smash = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<CraniumSMASH>(), (int)(projectile.damage * 0.5), projectile.knockBack, projectile.owner, 0f, 0f);
                Main.projectile[smash].Center = projectile.Center;
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            if (projectile.owner == Main.myPlayer)
            {
                int smash = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<CraniumSMASH>(), (int)(projectile.damage * 0.5), projectile.knockBack, projectile.owner, 0f, 0f);
                Main.projectile[smash].Center = projectile.Center;
            }
        }

        public override void Kill(int timeLeft)
        {
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 192;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Damage();
            Main.PlaySound(SoundID.Item, (int)projectile.Center.X, (int)projectile.Center.Y, 14);
            CalamityUtils.ExplosionGores(projectile.Center, 3);
            for (int num194 = 0; num194 < 25; num194++)
            {
                int num195 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 135, 0f, 0f, 100, default, 2f);
                Main.dust[num195].noGravity = true;
                Main.dust[num195].velocity *= 0f;
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = ModContent.GetTexture("CalamityMod/Projectiles/Rogue/CraniumSmasherGlow");
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, Color.White, projectile.rotation, tex.Size() / 2, projectile.scale, SpriteEffects.None, 0f);
        }
    }
}
