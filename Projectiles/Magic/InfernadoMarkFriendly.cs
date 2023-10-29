using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Magic
{
    public class InfernadoMarkFriendly : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public static readonly SoundStyle FlareSound = new("CalamityMod/Sounds/Custom/Yharon/YharonInfernado");

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 5;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
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
                            Vector2 randDustOffset = ((float)(Main.rand.NextDouble() * 3.1415927410125732) - 1.57079637f).ToRotationVector2() * (float)Main.rand.Next(3, 8);
                            int fiery = Dust.NewDust(dustRotate + randDustOffset, 0, 0, 244, randDustOffset.X * 2f, randDustOffset.Y * 2f, 100, default, 1.4f);
                            Main.dust[fiery].noGravity = true;
                            Main.dust[fiery].noLight = true;
                            Main.dust[fiery].velocity /= 4f;
                            Main.dust[fiery].velocity -= Projectile.velocity;
                        }
                        Projectile.alpha -= 5;
                        if (Projectile.alpha < 100)
                        {
                            Projectile.alpha = 100;
                        }
                    }
                    Vector2 projDirection = Main.player[playerTrack].Center - Projectile.Center;
                    float velocityMult = 4f;
                    velocityMult += Projectile.localAI[0] / 20f;
                    Projectile.velocity = Vector2.Normalize(projDirection) * velocityMult;
                    if (projDirection.Length() < 50f)
                    {
                        Projectile.Kill();
                    }
                }
            }
            else
            {
                float swaySize = 0.209439516f;
                float XChangeMult = 4f;
                float projXChange = (float)(Math.Cos((double)(swaySize * Projectile.ai[0])) - 0.5) * XChangeMult;
                Projectile.velocity.Y = Projectile.velocity.Y - projXChange;
                Projectile.ai[0] += 1f;
                projXChange = (float)(Math.Cos((double)(swaySize * Projectile.ai[0])) - 0.5) * XChangeMult;
                Projectile.velocity.Y = Projectile.velocity.Y + projXChange;
                Projectile.localAI[0] += 1f;
                if (Projectile.localAI[0] > 10f)
                {
                    Projectile.alpha -= 5;
                    if (Projectile.alpha < 100)
                    {
                        Projectile.alpha = 100;
                    }
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
            SoundEngine.PlaySound(FlareSound, Projectile.position);
            int dustAmt = 36;
            for (int i = 0; i < dustAmt; i++)
            {
                Vector2 rotate = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                rotate = rotate.RotatedBy((double)((float)(i - (dustAmt / 2 - 1)) * 6.28318548f / (float)dustAmt), default) + Projectile.Center;
                Vector2 faceDirection = rotate - Projectile.Center;
                int infernadoDust = Dust.NewDust(rotate + faceDirection, 0, 0, 244, faceDirection.X * 2f, faceDirection.Y * 2f, 100, default, 1.4f);
                Main.dust[infernadoDust].noGravity = true;
                Main.dust[infernadoDust].noLight = true;
                Main.dust[infernadoDust].velocity = faceDirection;
            }
            if (Projectile.owner == Main.myPlayer)
            {
                int projTileY = (int)(Projectile.Center.Y / 16f);
                int projTileX = (int)(Projectile.Center.X / 16f);
                int offsetUpwards = 100;
                if (projTileX < 10)
                {
                    projTileX = 10;
                }
                if (projTileX > Main.maxTilesX - 10)
                {
                    projTileX = Main.maxTilesX - 10;
                }
                if (projTileY < 10)
                {
                    projTileY = 10;
                }
                if (projTileY > Main.maxTilesY - offsetUpwards - 10)
                {
                    projTileY = Main.maxTilesY - offsetUpwards - 10;
                }
                for (int j = projTileY; j < projTileY + offsetUpwards; j++)
                {
                    Tile tile = Main.tile[projTileX, j];
                    if (tile.HasTile && (Main.tileSolid[(int)tile.TileType] || tile.LiquidAmount != 0))
                    {
                        projTileY = j;
                        break;
                    }
                }
                int infernado = Projectile.NewProjectile(Projectile.GetSource_FromThis(), (float)(projTileX * 16 + 8), (float)(projTileY * 16 - 24), 0f, 0f, ModContent.ProjectileType<InfernadoFriendly>(), Projectile.damage, Projectile.knockBack * 30f, Main.myPlayer, 16f, 16f);
                Main.projectile[infernado].netUpdate = true;
            }
        }
    }
}
