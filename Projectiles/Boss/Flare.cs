using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Boss
{
    public class Flare : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public static readonly SoundStyle FlareSound = new("CalamityMod/Sounds/Custom/Yharon/YharonInfernado");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.hostile = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }

            float wavingAmplitude = 0.209439516f;
            float projVelocityMult = 4f;
            float projFloatDirection = (float)(Math.Cos((double)(wavingAmplitude * Projectile.ai[0])) - 0.5) * projVelocityMult;
            Projectile.velocity.Y = Projectile.velocity.Y - projFloatDirection;
            Projectile.ai[0] += 1f;
            projFloatDirection = (float)(Math.Cos((double)(wavingAmplitude * Projectile.ai[0])) - 0.5) * projVelocityMult;
            Projectile.velocity.Y = Projectile.velocity.Y + projFloatDirection;

            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 10f)
            {
                Projectile.alpha -= 5;
                if (Projectile.alpha < 100)
                {
                    Projectile.alpha = 100;
                }
            }

            if (Projectile.wet)
            {
                Projectile.position.Y = Projectile.position.Y - 16f;
                Projectile.Kill();
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, Main.DiscoG, 53, Projectile.alpha);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
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
                int spawnLimitY = (int)(Main.player[Projectile.owner].Center.Y / 16f) + 25;
                if (projTileX > spawnLimitY)
                {
                    projTileX = spawnLimitY;
                }
                int flarenadoSpawn = Projectile.NewProjectile(Projectile.GetSource_FromThis(), (float)(projTileY * 16 + 8), (float)(projTileX * 16 - 24), 0f, 0f, ModContent.ProjectileType<Flarenado>(), 0, 4f, Main.myPlayer, 11f, 10f + (revenge ? 1f : 0f));
                Main.projectile[flarenadoSpawn].netUpdate = true;
            }
        }
    }
}
