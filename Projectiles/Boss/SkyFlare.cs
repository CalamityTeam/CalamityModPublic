using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Boss
{
    public class SkyFlare : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public int blowTimer = 0;

        public static readonly SoundStyle FlareSound = new("CalamityMod/Sounds/Custom/Yharon/YharonInfernado");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.9995f;
            int addStuff = Main.rand.Next(5);
            blowTimer += addStuff;
            if (blowTimer >= 900)
            {
                Projectile.Kill();
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 5)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 4)
            {
                Projectile.frame = 0;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, Main.DiscoG, 53, Projectile.alpha);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = ModContent.Request<Texture2D>(Texture).Value;
            int framing = ModContent.Request<Texture2D>(Texture).Value.Height / Main.projFrames[Projectile.type];
            int y6 = framing * Projectile.frame;
            Main.spriteBatch.Draw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, framing)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)framing / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            SoundEngine.PlaySound(FlareSound, Projectile.Center);
            int dustAmt = 36;
            for (int i = 0; i < dustAmt; i++)
            {
                Vector2 dustRotate = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                dustRotate = dustRotate.RotatedBy((double)((float)(i - (dustAmt / 2 - 1)) * 6.28318548f / (float)dustAmt), default) + Projectile.Center;
                Vector2 dustDirection = dustRotate - Projectile.Center;
                int flareDust = Dust.NewDust(dustRotate + dustDirection, 0, 0, 244, dustDirection.X * 2f, dustDirection.Y * 2f, 100, default, 1.4f);
                Main.dust[flareDust].noGravity = true;
                Main.dust[flareDust].noLight = true;
                Main.dust[flareDust].velocity = dustDirection;
            }
            if (Projectile.owner == Main.myPlayer)
            {
                int projTileX = (int)(Projectile.Center.Y / 16f);
                int projTileY = (int)(Projectile.Center.X / 16f);
                if (projTileY < 10)
                {
                    projTileY = 10;
                }
                if (projTileY > Main.maxTilesX - 10)
                {
                    projTileY = Main.maxTilesX - 10;
                }
                if (projTileX < 10)
                {
                    projTileX = 10;
                }
                if (projTileX > Main.maxTilesY - 110)
                {
                    projTileX = Main.maxTilesY - 110;
                }
                for (int j = projTileX; j < projTileX + 100; j++)
                {
                    Tile tile = Main.tile[projTileY, j];
                    if (tile.HasTile && (Main.tileSolid[(int)tile.TileType] || tile.LiquidAmount != 0))
                    {
                        projTileX = j;
                        break;
                    }
                }
                int tornadoType = Main.rand.Next(6);
                if (tornadoType < 5)
                {
                    int nadoDamage = Main.expertMode ? 180 : 300; //720
                    int nadoSpawn = Projectile.NewProjectile(Projectile.GetSource_FromThis(), (float)(projTileY * 16 + 8), (float)(projTileX * 16 - 24), 0f, 0f, ModContent.ProjectileType<Flarenado>(), nadoDamage, 4f, Main.myPlayer, 16f, 15f + (revenge ? 4f : 0f));
                    Main.projectile[nadoSpawn].netUpdate = true;
                }
                else
                {
                    int nadoDamage = Main.expertMode ? 230 : 400; //920
                    int nadoSpawn = Projectile.NewProjectile(Projectile.GetSource_FromThis(), (float)(projTileY * 16 + 8), (float)(projTileX * 16 - 24), 0f, 0f, ModContent.ProjectileType<Infernado>(), nadoDamage, 4f, Main.myPlayer, 16f, 16f + (revenge ? 4f : 0f));
                    Main.projectile[nadoSpawn].netUpdate = true;
                }
            }
        }
    }
}
