using CalamityMod.Dusts;
using CalamityMod.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class DarkEnergyBall : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dark Energy");
            Main.projFrames[projectile.type] = 6;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.Calamity().canBreakPlayerDefense = true;
            projectile.width = 80;
            projectile.height = 80;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 600;
            projectile.penetrate = -1;
            projectile.Opacity = 0f;
            cooldownSlot = 1;
        }

        public override void AI()
        {
            if (CalamityGlobalNPC.voidBoss < 0 || !Main.npc[CalamityGlobalNPC.voidBoss].active)
            {
                projectile.active = false;
                projectile.netUpdate = true;
                return;
            }

            if (Vector2.Distance(projectile.Center, Main.npc[CalamityGlobalNPC.voidBoss].Center) < 80f)
                projectile.Kill();

            if (projectile.velocity.Length() < 10f)
                projectile.velocity *= 1.05f;

            if (projectile.timeLeft < 30)
                projectile.Opacity = MathHelper.Clamp(projectile.timeLeft / 30f, 0f, 1f);
            else
                projectile.Opacity = MathHelper.Clamp(1f - ((projectile.timeLeft - 570) / 30f), 0f, 1f);

            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 5)
                projectile.frame = 0;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Color color = Color.Lerp(Color.White, Color.Fuchsia, 0.5f) * projectile.Opacity;
            Texture2D texture = Main.projectileTexture[projectile.type];
            int height = texture.Height / Main.projFrames[projectile.type];
            int drawStart = height * projectile.frame;
            Vector2 origin = projectile.Size / 2;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Projectiles/Boss/DarkEnergyBallGlow"), projectile.Center - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, drawStart, texture.Width, height)), Color.White, projectile.rotation, origin, projectile.scale, spriteEffects, 0f);

            color = Color.Lerp(Color.White, Color.Cyan, 0.5f) * projectile.Opacity;

            spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Projectiles/Boss/DarkEnergyBallGlow2"), projectile.Center - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, drawStart, texture.Width, height)), Color.White, projectile.rotation, origin, projectile.scale, spriteEffects, 0f);

        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(projectile.Center, 35f, targetHitbox);

        public override bool CanHitPlayer(Player target) => projectile.Opacity == 1f;

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (projectile.Opacity == 1f)
                target.AddBuff(BuffID.VortexDebuff, 60);
        }

        public override void Kill(int timeLeft)
        {
            for (int num621 = 0; num621 < 3; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 1.2f);
                Main.dust[num622].velocity *= 3f;
                Main.dust[num622].noGravity = true;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 5; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 1.7f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 1f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 2f;
            }
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            target.Calamity().lastProjectileHit = projectile;
        }
    }
}
