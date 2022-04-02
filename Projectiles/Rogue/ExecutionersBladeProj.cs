using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class ExecutionersBladeProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/ExecutionersBlade";

        private void handleStealth(Vector2 position)
        {
            if (Main.myPlayer == projectile.owner)
            {
                Main.PlaySound(SoundID.Item73, projectile.Center);
                for (int i = 0; i < 20; i++)
                {
                    Projectile.NewProjectile(new Vector2(position.X + (-600 + i * 60), position.Y - 800), new Vector2(0f, 2.5f), ModContent.ProjectileType<ExecutionersBladeStealthProj>(), (int)(projectile.damage * 1.2f), projectile.knockBack, projectile.owner);
                }
            }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blade");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 64;
            projectile.height = 64;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.aiStyle = 2;
            projectile.timeLeft = 240;
            aiType = ProjectileID.ThrowingKnife;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0.5f, 0f, 0.65f);
            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 173, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
            CalamityGlobalProjectile.HomeInOnNPC(projectile, true, 250f, 12f, 20f);
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 origin = new Vector2(32f, 32f);
            spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Items/Weapons/Rogue/ExecutionersBladeGlow"), projectile.Center - Main.screenPosition, null, Color.White, projectile.rotation, origin, 1f, SpriteEffects.None, 0f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 180);
            if (projectile.Calamity().stealthStrike)
            {
                handleStealth(target.Center);
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 180);
            if (projectile.Calamity().stealthStrike)
            {
                handleStealth(target.Center);
            }
        }

        public override void Kill(int timeLeft)
        {
            CalamityGlobalProjectile.ExpandHitboxBy(projectile, 128);
            Main.PlaySound(SoundID.Item74, (int)projectile.position.X, (int)projectile.position.Y);
            for (int num621 = 0; num621 < 3; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 173, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 5; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 173, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 173, 0f, 0f, 100, default, 2f);
                Main.dust[num624].velocity *= 2f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
