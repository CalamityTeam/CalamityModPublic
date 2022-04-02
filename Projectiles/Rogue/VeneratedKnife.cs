using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class VeneratedKnife : ModProjectile
    {
        int lifetime = 150;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Venerated Knife");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.penetrate = 1;
            projectile.timeLeft = lifetime;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (projectile.timeLeft < lifetime - 5)
            {
                float minDist = 999f;
                int index = 0;
                for (int i = 0; i < Main.npc.Length; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy(projectile, false))
                    {
                        float dist = (projectile.Center - npc.Center).Length();
                        if (dist < minDist)
                        {
                            minDist = dist;
                            index = i;
                        }
                    }
                }

                Vector2 velocityNew;
                if (minDist < 999f)
                {
                    velocityNew = Main.npc[index].Center - projectile.Center;
                    velocityNew.Normalize();
                    velocityNew *= 5f;
                    projectile.velocity += velocityNew;
                    if (projectile.velocity.Length() > 15f)
                    {
                        projectile.velocity.Normalize();
                        projectile.velocity *= 15f;
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {

            if (projectile.ai[0] == 0f)
            {
                Texture2D knife1 = Main.projectileTexture[projectile.type];
                CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 3, knife1);
            }
            else if (projectile.ai[0] == 1f)
            {
                Texture2D knife2 = ModContent.GetTexture("CalamityMod/Projectiles/Rogue/VeneratedKnife2");
                CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 3, knife2);
            }
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {

            Vector2 origin = new Vector2(projectile.width / 2, projectile.height / 2);
            if (projectile.ai[0] == 0f)
            {
                Texture2D knife1Glow = ModContent.GetTexture("CalamityMod/Projectiles/Rogue/VeneratedKnifeGlow");
                spriteBatch.Draw(knife1Glow, projectile.Center - Main.screenPosition, null, Color.White, projectile.rotation, origin, 1f, SpriteEffects.None, 0f);
            }
            else if (projectile.ai[0] == 1f)
            {
                Texture2D knife2Glow = ModContent.GetTexture("CalamityMod/Projectiles/Rogue/VeneratedKnife2Glow");
                spriteBatch.Draw(knife2Glow, projectile.Center - Main.screenPosition, null, Color.White, projectile.rotation, origin, 1f, SpriteEffects.None, 0f);
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                int dustType = 0;
                if (projectile.ai[0] == 0f)
                {
                    dustType = 111;
                }
                else if (projectile.ai[0] == 1f)
                {
                    dustType = 112;
                }

                int dust = Dust.NewDust(projectile.Center, 1, 1, dustType, 0, 0, 0, default, 1.5f);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
