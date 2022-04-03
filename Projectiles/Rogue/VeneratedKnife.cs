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
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = lifetime;
            Projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (Projectile.timeLeft < lifetime - 5)
            {
                float minDist = 999f;
                int index = 0;
                for (int i = 0; i < Main.npc.Length; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy(Projectile, false))
                    {
                        float dist = (Projectile.Center - npc.Center).Length();
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
                    velocityNew = Main.npc[index].Center - Projectile.Center;
                    velocityNew.Normalize();
                    velocityNew *= 5f;
                    Projectile.velocity += velocityNew;
                    if (Projectile.velocity.Length() > 15f)
                    {
                        Projectile.velocity.Normalize();
                        Projectile.velocity *= 15f;
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {

            if (Projectile.ai[0] == 0f)
            {
                Texture2D knife1 = ModContent.Request<Texture2D>(Texture).Value;
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 3, knife1);
            }
            else if (Projectile.ai[0] == 1f)
            {
                Texture2D knife2 = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Rogue/VeneratedKnife2");
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 3, knife2);
            }
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {

            Vector2 origin = new Vector2(Projectile.width / 2, Projectile.height / 2);
            if (Projectile.ai[0] == 0f)
            {
                Texture2D knife1Glow = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Rogue/VeneratedKnifeGlow");
                spriteBatch.Draw(knife1Glow, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, origin, 1f, SpriteEffects.None, 0f);
            }
            else if (Projectile.ai[0] == 1f)
            {
                Texture2D knife2Glow = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Rogue/VeneratedKnife2Glow");
                spriteBatch.Draw(knife2Glow, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, origin, 1f, SpriteEffects.None, 0f);
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                int dustType = 0;
                if (Projectile.ai[0] == 0f)
                {
                    dustType = 111;
                }
                else if (Projectile.ai[0] == 1f)
                {
                    dustType = 112;
                }

                int dust = Dust.NewDust(Projectile.Center, 1, 1, dustType, 0, 0, 0, default, 1.5f);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
