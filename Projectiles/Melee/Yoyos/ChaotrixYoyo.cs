using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Yoyos
{
    public class ChaotrixYoyo : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fault");
            ProjectileID.Sets.YoyosLifeTimeMultiplier[projectile.type] = 14f;
            ProjectileID.Sets.YoyosMaximumRange[projectile.type] = 420f;
            ProjectileID.Sets.YoyosTopSpeed[projectile.type] = 13f;

            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.aiStyle = 99;
            projectile.width = 16;
            projectile.height = 16;
            projectile.scale = 1.15f;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = -1;
            projectile.MaxUpdates = 2;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(5))
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, Main.rand.NextBool(3) ? 16 : 127, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);

            if (Main.rand.NextBool(8))
            {
                int smoke = Gore.NewGore(projectile.position, default, Main.rand.Next(375, 378), 0.5f);
                Main.gore[smoke].behindTiles = true;
            }
            if ((projectile.position - Main.player[projectile.owner].position).Length() > 3200f) //200 blocks
                projectile.Kill();
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 240);
            if (projectile.owner == Main.myPlayer)
            {
                int boom = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<FuckYou>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
                if (boom.WithinBounds(Main.maxProjectiles))
                    Main.projectile[boom].Calamity().forceMelee = true;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
