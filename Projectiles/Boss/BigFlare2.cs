using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Boss
{
    public class BigFlare2 : ModProjectile, ILocalizedModType
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
            Projectile.timeLeft = 1200;
            Projectile.scale = 1.5f;
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

            if (Projectile.ai[1] > 0f)
            {
                int playerTracker = (int)Projectile.ai[1] - 1;
                if (playerTracker < 255)
                {
                    Projectile.localAI[0] += 1f;
                    if (Projectile.localAI[0] > 10f)
                    {
                        int dustAmt = 6;
                        for (int i = 0; i < dustAmt; i++)
                        {
                            Vector2 dustRotation = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                            dustRotation = dustRotation.RotatedBy((double)(i - (dustAmt / 2 - 1)) * 3.1415926535897931 / (double)(float)dustAmt, default) + Projectile.Center;
                            Vector2 randomDustPos = ((float)(Main.rand.NextDouble() * 3.1415927410125732) - 1.57079637f).ToRotationVector2() * (float)Main.rand.Next(3, 8);
                            int flareDust = Dust.NewDust(dustRotation + randomDustPos, 0, 0, 244, randomDustPos.X * 2f, randomDustPos.Y * 2f, 100, default, 1.4f);
                            Main.dust[flareDust].noGravity = true;
                            Main.dust[flareDust].noLight = true;
                            Main.dust[flareDust].velocity /= 4f;
                            Main.dust[flareDust].velocity -= Projectile.velocity;
                        }
                        Projectile.alpha -= 5;
                        if (Projectile.alpha < 100)
                        {
                            Projectile.alpha = 100;
                        }
                    }

                    Vector2 playerDistance = Main.player[playerTracker].Center - Projectile.Center;
                    float projVelocityMult = 4f;
                    projVelocityMult += Projectile.localAI[0] / 60f;
                    Projectile.velocity = Vector2.Normalize(playerDistance) * projVelocityMult;
                    if (playerDistance.Length() < 64f)
                    {
                        Projectile.Kill();
                    }
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
            int killDustAmt = 36;
            for (int i = 0; i < killDustAmt; i++)
            {
                Vector2 killDustRotate = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                killDustRotate = killDustRotate.RotatedBy((double)((float)(i - (killDustAmt / 2 - 1)) * 6.28318548f / (float)killDustAmt), default) + Projectile.Center;
                Vector2 killDustDirection = killDustRotate - Projectile.Center;
                int killFlareDust = Dust.NewDust(killDustRotate + killDustDirection, 0, 0, 244, killDustDirection.X * 2f, killDustDirection.Y * 2f, 100, default, 1.4f);
                Main.dust[killFlareDust].noGravity = true;
                Main.dust[killFlareDust].noLight = true;
                Main.dust[killFlareDust].velocity = killDustDirection;
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
                int spawnAreaY = Main.maxTilesY - projTileX;
                for (int j = projTileX; j < projTileX + spawnAreaY; j++)
                {
                    Tile tile = Main.tile[projTileY, j + 10];
                    if (tile.HasTile && !TileID.Sets.Platforms[tile.TileType] && (Main.tileSolid[(int)tile.TileType] || tile.LiquidAmount != 0))
                    {
                        projTileX = j;
                        break;
                    }
                }
                int spawnLimitY = (int)(Main.player[Projectile.owner].Center.Y / 16f) + 75;
                if (projTileX > spawnLimitY)
                {
                    projTileX = spawnLimitY;
                }
                int infernadoSpawn = Projectile.NewProjectile(Projectile.GetSource_FromThis(), (float)(projTileY * 16 + 8), (float)(projTileX * 16 - 24), 0f, 0f, ModContent.ProjectileType<Infernado2>(), 0, 4f, Main.myPlayer, 11f, 24f + (revenge ? 2f : 0f));
                Main.projectile[infernadoSpawn].netUpdate = true;
            }
        }
    }
}
