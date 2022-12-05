using CalamityMod.Buffs.StatBuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Magic
{
    public class AsteroidMolten : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Asteroid");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.extraUpdates = 2;
        }

        public override void AI()
        {
            if (Projectile.position.Y > Main.player[Projectile.owner].position.Y - 300f)
            {
                Projectile.tileCollide = true;
            }
            if ((double)Projectile.position.Y < Main.worldSurface * 16.0)
            {
                Projectile.tileCollide = true;
            }
            Projectile.scale = Projectile.ai[1];
            Projectile.rotation += Projectile.velocity.X * 2f;
            Vector2 position = Projectile.Center + Vector2.Normalize(Projectile.velocity) * 10f;
            Dust dust20 = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 244, 0f, 0f, 0, new Color(255, Main.DiscoG, 0), 1f)];
            dust20.position = position;
            dust20.velocity = Projectile.velocity.RotatedBy(1.5707963705062866, default) * 0.33f + Projectile.velocity / 4f;
            dust20.position += Projectile.velocity.RotatedBy(1.5707963705062866, default);
            dust20.fadeIn = 0.5f;
            dust20.noGravity = true;
            dust20 = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 244, 0f, 0f, 0, new Color(255, Main.DiscoG, 0), 1f)];
            dust20.position = position;
            dust20.velocity = Projectile.velocity.RotatedBy(-1.5707963705062866, default) * 0.33f + Projectile.velocity / 4f;
            dust20.position += Projectile.velocity.RotatedBy(-1.5707963705062866, default);
            dust20.fadeIn = 0.5f;
            dust20.noGravity = true;
            for (int num189 = 0; num189 < 1; num189++)
            {
                int num190 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 244, 0f, 0f, 0, new Color(255, Main.DiscoG, 0), 1f);
                Main.dust[num190].velocity *= 0.5f;
                Main.dust[num190].scale *= 1.3f;
                Main.dust[num190].fadeIn = 1f;
                Main.dust[num190].noGravity = true;
            }
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item89, Projectile.position);
            Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
            Projectile.width = (int)(128f * Projectile.scale);
            Projectile.height = (int)(128f * Projectile.scale);
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            for (int num336 = 0; num336 < 8; num336++)
            {
                Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 244, 0f, 0f, 100, new Color(255, Main.DiscoG, 0), 1.5f);
            }
            for (int num337 = 0; num337 < 32; num337++)
            {
                int num338 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 244, 0f, 0f, 100, new Color(255, Main.DiscoG, 0), 2.5f);
                Main.dust[num338].noGravity = true;
                Main.dust[num338].velocity *= 3f;
                num338 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 244, 0f, 0f, 100, new Color(255, Main.DiscoG, 0), 1.5f);
                Main.dust[num338].velocity *= 2f;
                Main.dust[num338].noGravity = true;
            }
            if (Main.netMode != NetmodeID.Server)
            {
                for (int num339 = 0; num339 < 2; num339++)
                {
                    int num340 = Gore.NewGore(Projectile.GetSource_Death(), Projectile.position + new Vector2((float)(Projectile.width * Main.rand.Next(100)) / 100f, (float)(Projectile.height * Main.rand.Next(100)) / 100f) - Vector2.One * 10f, default, Main.rand.Next(61, 64), 1f);
                    Gore gore = Main.gore[num340];
                    gore.velocity *= 0.3f;
                    gore.velocity.X += (float)Main.rand.Next(-10, 11) * 0.05f;
                    gore.velocity.Y += (float)Main.rand.Next(-10, 11) * 0.05f;
                }
            }
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.localAI[1] = -1f;
                Projectile.maxPenetrate = 0;
                Projectile.Damage();
            }
            for (int num341 = 0; num341 < 5; num341++)
            {
                int num342 = Utils.SelectRandom(Main.rand, new int[]
                {
                    244,
                    259,
                    158
                });
                int num343 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, num342, 2.5f * (float)Projectile.direction, -2.5f, 0, new Color(255, Main.DiscoG, 0), 1f);
                Main.dust[num343].alpha = 200;
                Main.dust[num343].velocity *= 2.4f;
                Main.dust[num343].scale += Main.rand.NextFloat();
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => target.AddBuff(BuffID.OnFire3, 180);

        public override void OnHitPvp(Player target, int damage, bool crit) => target.AddBuff(BuffID.OnFire3, 180);

        public override bool PreDraw(ref Color lightColor)
        {
            //Changes the texture of the projectile
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            switch ((int)Projectile.ai[0])
            {
                case 0:
                    break;
                case 1:
                    texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Magic/AsteroidMolten2").Value;
                    break;
                case 2:
                    texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Magic/AsteroidMolten3").Value;
                    break;
                case 3:
                    texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Magic/AsteroidMolten4").Value;
                    break;
                case 4:
                    texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Magic/AsteroidMolten5").Value;
                    break;
                case 5:
                    texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Magic/AsteroidMolten6").Value;
                    break;
                default:
                    break;
            }
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1, texture);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Magic/AsteroidMoltenGlow").Value;
            switch ((int)Projectile.ai[0])
            {
                case 0:
                    break;
                case 1:
                    texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Magic/AsteroidMoltenGlow2").Value;
                    break;
                case 2:
                    texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Magic/AsteroidMoltenGlow3").Value;
                    break;
                case 3:
                    texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Magic/AsteroidMoltenGlow4").Value;
                    break;
                case 4:
                    return;
                case 5:
                    texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Magic/AsteroidMoltenGlow6").Value;
                    break;
                default:
                    break;
            }
            Vector2 origin = texture.Size() / 2f;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
        }
    }
}
