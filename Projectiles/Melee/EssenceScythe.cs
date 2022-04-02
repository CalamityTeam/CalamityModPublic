using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Healing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class EssenceScythe : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scythe");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 100;
            projectile.height = 78;
            projectile.aiStyle = 18;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.tileCollide = false;
            projectile.alpha = 55;
            projectile.penetrate = 10;
            projectile.timeLeft = 300;
            projectile.ignoreWater = true;
            aiType = ProjectileID.DeathSickle;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.5f / 255f, (255 - projectile.alpha) * 0f / 255f, (255 - projectile.alpha) * 0.65f / 255f);

            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 173, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }

            CalamityGlobalProjectile.HomeInOnNPC(projectile, true, 300f, 12f, 20f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 2);
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 origin = new Vector2(50f, 39f);
            Color color = Color.White;
            if (projectile.timeLeft < 85)
            {
                byte b2 = (byte)(projectile.timeLeft * 3);
                byte a2 = (byte)(100f * ((float)b2 / 255f));
                color = new Color((int)b2, (int)b2, (int)b2, (int)a2);
            }
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(Main.projectileTexture[projectile.type], projectile.Center - Main.screenPosition, null, color, projectile.rotation, origin, 1f, spriteEffects, 0f);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(150, 0, 200, 0);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 2;
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 180);
            if (target.life <= 0)
            {
                if (projectile.owner == Main.myPlayer)
                    CalamityGlobalProjectile.SpawnLifeStealProjectile(projectile, Main.player[projectile.owner], 8, ModContent.ProjectileType<EssenceFlame>(), 1200f, 0f);
            }
        }
    }
}
