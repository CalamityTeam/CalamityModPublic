using CalamityMod.Buffs.StatBuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class AsteroidMolten : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Asteroid");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.magic = true;
            projectile.extraUpdates = 2;
        }

        public override void AI()
        {
            if (projectile.position.Y > Main.player[projectile.owner].position.Y - 300f)
            {
                projectile.tileCollide = true;
            }
            if ((double)projectile.position.Y < Main.worldSurface * 16.0)
            {
                projectile.tileCollide = true;
            }
            projectile.scale = projectile.ai[1];
            projectile.rotation += projectile.velocity.X * 2f;
            Vector2 position = projectile.Center + Vector2.Normalize(projectile.velocity) * 10f;
            Dust dust20 = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 244, 0f, 0f, 0, new Color(255, Main.DiscoG, 0), 1f)];
            dust20.position = position;
            dust20.velocity = projectile.velocity.RotatedBy(1.5707963705062866, default) * 0.33f + projectile.velocity / 4f;
            dust20.position += projectile.velocity.RotatedBy(1.5707963705062866, default);
            dust20.fadeIn = 0.5f;
            dust20.noGravity = true;
            dust20 = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 244, 0f, 0f, 0, new Color(255, Main.DiscoG, 0), 1f)];
            dust20.position = position;
            dust20.velocity = projectile.velocity.RotatedBy(-1.5707963705062866, default) * 0.33f + projectile.velocity / 4f;
            dust20.position += projectile.velocity.RotatedBy(-1.5707963705062866, default);
            dust20.fadeIn = 0.5f;
            dust20.noGravity = true;
            for (int num189 = 0; num189 < 1; num189++)
            {
                int num190 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 244, 0f, 0f, 0, new Color(255, Main.DiscoG, 0), 1f);
                Main.dust[num190].velocity *= 0.5f;
                Main.dust[num190].scale *= 1.3f;
                Main.dust[num190].fadeIn = 1f;
                Main.dust[num190].noGravity = true;
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item89, projectile.position);
            projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
            projectile.width = (int)(128f * projectile.scale);
            projectile.height = (int)(128f * projectile.scale);
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            for (int num336 = 0; num336 < 8; num336++)
            {
                Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 244, 0f, 0f, 100, new Color(255, Main.DiscoG, 0), 1.5f);
            }
            for (int num337 = 0; num337 < 32; num337++)
            {
                int num338 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 244, 0f, 0f, 100, new Color(255, Main.DiscoG, 0), 2.5f);
                Main.dust[num338].noGravity = true;
                Main.dust[num338].velocity *= 3f;
                num338 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 244, 0f, 0f, 100, new Color(255, Main.DiscoG, 0), 1.5f);
                Main.dust[num338].velocity *= 2f;
                Main.dust[num338].noGravity = true;
            }
            for (int num339 = 0; num339 < 2; num339++)
            {
                int num340 = Gore.NewGore(projectile.position + new Vector2((float)(projectile.width * Main.rand.Next(100)) / 100f, (float)(projectile.height * Main.rand.Next(100)) / 100f) - Vector2.One * 10f, default, Main.rand.Next(61, 64), 1f);
                Gore gore = Main.gore[num340];
                gore.velocity *= 0.3f;
                gore.velocity.X += (float)Main.rand.Next(-10, 11) * 0.05f;
                gore.velocity.Y += (float)Main.rand.Next(-10, 11) * 0.05f;
            }
            if (projectile.owner == Main.myPlayer)
            {
                projectile.localAI[1] = -1f;
                projectile.maxPenetrate = 0;
                projectile.Damage();
            }
            for (int num341 = 0; num341 < 5; num341++)
            {
                int num342 = Utils.SelectRandom(Main.rand, new int[]
                {
                    244,
                    259,
                    158
                });
                int num343 = Dust.NewDust(projectile.position, projectile.width, projectile.height, num342, 2.5f * (float)projectile.direction, -2.5f, 0, new Color(255, Main.DiscoG, 0), 1f);
                Main.dust[num343].alpha = 200;
                Main.dust[num343].velocity *= 2.4f;
                Main.dust[num343].scale += Main.rand.NextFloat();
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Player player = Main.player[projectile.owner];
            player.AddBuff(ModContent.BuffType<Molten>(), 360);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            //Changes the texture of the projectile
            Texture2D texture = Main.projectileTexture[projectile.type];
            switch ((int)projectile.ai[0])
            {
                case 0:
                    break;
                case 1:
                    texture = ModContent.GetTexture("CalamityMod/Projectiles/Magic/AsteroidMolten2");
                    break;
                case 2:
                    texture = ModContent.GetTexture("CalamityMod/Projectiles/Magic/AsteroidMolten3");
                    break;
                case 3:
                    texture = ModContent.GetTexture("CalamityMod/Projectiles/Magic/AsteroidMolten4");
                    break;
                case 4:
                    texture = ModContent.GetTexture("CalamityMod/Projectiles/Magic/AsteroidMolten5");
                    break;
                case 5:
                    texture = ModContent.GetTexture("CalamityMod/Projectiles/Magic/AsteroidMolten6");
                    break;
                default:
                    break;
            }
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1, texture);
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = ModContent.GetTexture("CalamityMod/Projectiles/Magic/AsteroidMoltenGlow");
            switch ((int)projectile.ai[0])
            {
                case 0:
                    break;
                case 1:
                    texture = ModContent.GetTexture("CalamityMod/Projectiles/Magic/AsteroidMoltenGlow2");
                    break;
                case 2:
                    texture = ModContent.GetTexture("CalamityMod/Projectiles/Magic/AsteroidMoltenGlow3");
                    break;
                case 3:
                    texture = ModContent.GetTexture("CalamityMod/Projectiles/Magic/AsteroidMoltenGlow4");
                    break;
                case 4:
                    return;
                case 5:
                    texture = ModContent.GetTexture("CalamityMod/Projectiles/Magic/AsteroidMoltenGlow6");
                    break;
                default:
                    break;
            }
            Vector2 origin = texture.Size() / 2f;
            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, Color.White, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
        }
    }
}
