using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class BrinyTyphoonBubble : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 5;
        }

        public override void AI()
        {
            if (Projectile.ai[1] > 0f)
            {
                int playerTrack = (int)Projectile.ai[1] - 1;
                if (playerTrack < 255)
                {
                    Projectile.localAI[0] += 1f;
                    if (Projectile.localAI[0] > 10f)
                    {
                        int dustAmt = 6;
                        for (int i = 0; i < dustAmt; i++)
                        {
                            Vector2 dustRotate = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                            dustRotate = dustRotate.RotatedBy((double)(i - (dustAmt / 2 - 1)) * 3.1415926535897931 / (double)(float)dustAmt, default) + Projectile.Center;
                            Vector2 faceDirection = ((float)(Main.rand.NextDouble() * 3.1415927410125732) - 1.57079637f).ToRotationVector2() * (float)Main.rand.Next(3, 8);
                            int bluishDust = Dust.NewDust(dustRotate + faceDirection, 0, 0, 187, faceDirection.X * 2f, faceDirection.Y * 2f, 100, new Color(53, Main.DiscoG, 255), 1.4f);
                            Main.dust[bluishDust].noGravity = true;
                            Main.dust[bluishDust].noLight = true;
                            Main.dust[bluishDust].velocity /= 4f;
                            Main.dust[bluishDust].velocity -= Projectile.velocity;
                        }
                        Projectile.alpha -= 5;
                        if (Projectile.alpha < 100)
                        {
                            Projectile.alpha = 100;
                        }
                        Projectile.rotation += Projectile.velocity.X * 0.1f;
                        Projectile.frame = (int)(Projectile.localAI[0] / 3f) % 3;
                    }
                    Vector2 playerDirection = Main.player[playerTrack].Center - Projectile.Center;
                    float projVelocity = 4f;
                    projVelocity += Projectile.localAI[0] / 20f;
                    Projectile.velocity = Vector2.Normalize(playerDirection) * projVelocity;
                    if (playerDirection.Length() < 50f)
                    {
                        Projectile.Kill();
                    }
                }
            }
            else
            {
                float spoutSway = 0.209439516f;
                float spoutSpawn = (float)(Math.Cos((double)(spoutSway * Projectile.ai[0])) - 0.5) * 4f;
                Projectile.velocity.Y = Projectile.velocity.Y - spoutSpawn;
                Projectile.ai[0] += 1f;
                spoutSpawn = (float)(Math.Cos((double)(spoutSway * Projectile.ai[0])) - 0.5) * 4f;
                Projectile.velocity.Y = Projectile.velocity.Y + spoutSpawn;
                Projectile.localAI[0] += 1f;
                if (Projectile.localAI[0] > 10f)
                {
                    Projectile.alpha -= 5;
                    if (Projectile.alpha < 100)
                    {
                        Projectile.alpha = 100;
                    }
                    Projectile.rotation += Projectile.velocity.X * 0.1f;
                    Projectile.frame = (int)(Projectile.localAI[0] / 3f) % 3;
                }
            }
            if (Projectile.wet)
            {
                Projectile.position.Y = Projectile.position.Y - 16f;
                Projectile.Kill();
                return;
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item96, Projectile.Center);
            int moreDustAmt = 36;
            for (int j = 0; j < moreDustAmt; j++)
            {
                Vector2 rotate = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                rotate = rotate.RotatedBy((double)((float)(j - (moreDustAmt / 2 - 1)) * 6.28318548f / (float)moreDustAmt), default) + Projectile.Center;
                Vector2 facingDirection = rotate - Projectile.Center;
                int killDust = Dust.NewDust(rotate + facingDirection, 0, 0, 187, facingDirection.X * 2f, facingDirection.Y * 2f, 100, new Color(53, Main.DiscoG, 255), 1.4f);
                Main.dust[killDust].noGravity = true;
                Main.dust[killDust].noLight = true;
                Main.dust[killDust].velocity = facingDirection;
            }
            if (Projectile.owner == Main.myPlayer)
            {
                int projTileX = (int)(Projectile.Center.Y / 16f);
                int projTileY = (int)(Projectile.Center.X / 16f);
                int posModifier = 100;
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
                if (projTileX > Main.maxTilesY - posModifier - 10)
                {
                    projTileX = Main.maxTilesY - posModifier - 10;
                }
                for (int k = projTileX; k < projTileX + posModifier; k++)
                {
                    Tile tile = Main.tile[projTileY, k];
                    if (tile.HasTile && (Main.tileSolid[(int)tile.TileType] || tile.LiquidAmount != 0))
                    {
                        projTileX = k;
                        break;
                    }
                }
                int SPOUT = Projectile.NewProjectile(Projectile.GetSource_FromThis(), (float)(projTileY * 16 + 8), (float)(projTileX * 16 - 24), 0f, 0f, ModContent.ProjectileType<BrinySpout>(), Projectile.damage, 6f, Main.myPlayer, 8f, 25f);
                Main.projectile[SPOUT].netUpdate = true;
            }
        }
    }
}
