using CalamityMod.Buffs.StatBuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Magic
{
    public class AsteroidMolten : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override void SetStaticDefaults()
        {
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
            Dust flaming = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 244, 0f, 0f, 0, new Color(255, Main.DiscoG, 0), 1f)];
            flaming.position = position;
            flaming.velocity = Projectile.velocity.RotatedBy(1.5707963705062866, default) * 0.33f + Projectile.velocity / 4f;
            flaming.position += Projectile.velocity.RotatedBy(1.5707963705062866, default);
            flaming.fadeIn = 0.5f;
            flaming.noGravity = true;
            flaming = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 244, 0f, 0f, 0, new Color(255, Main.DiscoG, 0), 1f)];
            flaming.position = position;
            flaming.velocity = Projectile.velocity.RotatedBy(-1.5707963705062866, default) * 0.33f + Projectile.velocity / 4f;
            flaming.position += Projectile.velocity.RotatedBy(-1.5707963705062866, default);
            flaming.fadeIn = 0.5f;
            flaming.noGravity = true;

            int fiery = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 244, 0f, 0f, 0, new Color(255, Main.DiscoG, 0), 1f);
            Main.dust[fiery].velocity *= 0.5f;
            Main.dust[fiery].scale *= 1.3f;
            Main.dust[fiery].fadeIn = 1f;
            Main.dust[fiery].noGravity = true;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item89, Projectile.position);
            Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
            Projectile.width = (int)(128f * Projectile.scale);
            Projectile.height = (int)(128f * Projectile.scale);
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            for (int i = 0; i < 8; i++)
            {
                Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 244, 0f, 0f, 100, new Color(255, Main.DiscoG, 0), 1.5f);
            }
            for (int j = 0; j < 32; j++)
            {
                int killFire = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 244, 0f, 0f, 100, new Color(255, Main.DiscoG, 0), 2.5f);
                Main.dust[killFire].noGravity = true;
                Main.dust[killFire].velocity *= 3f;
                killFire = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 244, 0f, 0f, 100, new Color(255, Main.DiscoG, 0), 1.5f);
                Main.dust[killFire].velocity *= 2f;
                Main.dust[killFire].noGravity = true;
            }
            if (Main.netMode != NetmodeID.Server)
            {
                for (int k = 0; k < 2; k++)
                {
                    int gored = Gore.NewGore(Projectile.GetSource_Death(), Projectile.position + new Vector2((float)(Projectile.width * Main.rand.Next(100)) / 100f, (float)(Projectile.height * Main.rand.Next(100)) / 100f) - Vector2.One * 10f, default, Main.rand.Next(61, 64), 1f);
                    Gore gore = Main.gore[gored];
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
            for (int l = 0; l < 5; l++)
            {
                int dustType = Utils.SelectRandom(Main.rand, new int[]
                {
                    244,
                    259,
                    158
                });
                int exploding = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 2.5f * (float)Projectile.direction, -2.5f, 0, new Color(255, Main.DiscoG, 0), 1f);
                Main.dust[exploding].alpha = 200;
                Main.dust[exploding].velocity *= 2.4f;
                Main.dust[exploding].scale += Main.rand.NextFloat();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.OnFire3, 180);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.OnFire3, 180);

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
