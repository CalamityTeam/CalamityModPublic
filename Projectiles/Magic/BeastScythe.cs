using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class BeastScythe : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 54;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.extraUpdates = 1;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 20;
            }
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }

            Projectile.rotation += 0.5f;

            Lighting.AddLight(Projectile.Center, 0.35f, 0f, 0.35f);
            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 173, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f);
            }

            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] <= 30f)
            {
                Projectile.velocity *= 0.999f;
            }
            if (Main.myPlayer == Projectile.owner && Projectile.ai[0] == 30f)
            {
                if (Main.player[Projectile.owner].channel)
                {
                    float projDelay = 20f;
                    Vector2 vector10 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
                    float projXDirection = (float)Main.mouseX + Main.screenPosition.X - vector10.X;
                    float projYDirection = (float)Main.mouseY + Main.screenPosition.Y - vector10.Y;
                    if (Main.player[Projectile.owner].gravDir == -1f)
                    {
                        projYDirection = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector10.Y;
                    }
                    float projDistance = (float)Math.Sqrt((double)(projXDirection * projXDirection + projYDirection * projYDirection));
                    projDistance = (float)Math.Sqrt((double)(projXDirection * projXDirection + projYDirection * projYDirection));
                    if (projDistance > projDelay)
                    {
                        projDistance = projDelay / projDistance;
                        projXDirection *= projDistance;
                        projYDirection *= projDistance;
                        int projXSpeed = (int)(projXDirection * 1000f);
                        int projXSpeedMagnified = (int)(Projectile.velocity.X * 1000f);
                        int projYSpeed = (int)(projYDirection * 1000f);
                        int projYSpeedMagnified = (int)(Projectile.velocity.Y * 1000f);
                        if (projXSpeed != projXSpeedMagnified || projYSpeed != projYSpeedMagnified)
                        {
                            Projectile.netUpdate = true;
                        }
                        Projectile.velocity.X = projXDirection;
                        Projectile.velocity.Y = projYDirection;
                    }
                    else
                    {
                        int projXSpeedElse = (int)(projXDirection * 1000f);
                        int projXSpeedMagnifiedElse = (int)(Projectile.velocity.X * 1000f);
                        int projYSpeedElse = (int)(projYDirection * 1000f);
                        int projYSpeedMagnifiedElse = (int)(Projectile.velocity.Y * 1000f);
                        if (projXSpeedElse != projXSpeedMagnifiedElse || projYSpeedElse != projYSpeedMagnifiedElse)
                        {
                            Projectile.netUpdate = true;
                        }
                        Projectile.velocity.X = projXDirection;
                        Projectile.velocity.Y = projYDirection;
                    }
                }
                else
                {
                    Projectile.netUpdate = true;
                    Vector2 projDirection = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
                    float projXDir = (float)Main.mouseX + Main.screenPosition.X - projDirection.X;
                    float projYDir = (float)Main.mouseY + Main.screenPosition.Y - projDirection.Y;
                    if (Main.player[Projectile.owner].gravDir == -1f)
                    {
                        projYDir = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - projDirection.Y;
                    }
                    float projDistancing = (float)Math.Sqrt((double)(projXDir * projXDir + projYDir * projYDir));
                    if (projDistancing == 0f || Projectile.ai[0] < 0f)
                    {
                        projDirection = new Vector2(Main.player[Projectile.owner].position.X + (float)(Main.player[Projectile.owner].width / 2), Main.player[Projectile.owner].position.Y + (float)(Main.player[Projectile.owner].height / 2));
                        projXDir = Projectile.position.X + (float)Projectile.width * 0.5f - projDirection.X;
                        projYDir = Projectile.position.Y + (float)Projectile.height * 0.5f - projDirection.Y;
                        projDistancing = (float)Math.Sqrt((double)(projXDir * projXDir + projYDir * projYDir));
                    }
                    projDistancing = 20f / projDistancing;
                    projXDir *= projDistancing;
                    projYDir *= projDistancing;
                    Projectile.velocity.X = projXDir;
                    Projectile.velocity.Y = projYDir;
                }
            }
            if (Projectile.ai[0] >= 30f)
            {
                Projectile.velocity *= 1.001f;

                CalamityUtils.HomeInOnNPC(Projectile, true, 200f, 12f, 20f);
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 100;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.damage /= 2;
            Projectile.Damage();
            bool isInTile = WorldGen.SolidTile(Framing.GetTileSafely((int)Projectile.position.X / 16, (int)Projectile.position.Y / 16));
            for (int m = 0; m < 4; m++)
            {
                Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 173, 0f, 0f, 100, default, 1.5f);
            }
            for (int n = 0; n < 4; n++)
            {
                int beastial = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 173, 0f, 0f, 0, default, 2.5f);
                Main.dust[beastial].noGravity = true;
                Main.dust[beastial].velocity *= 3f;
                if (isInTile)
                {
                    Main.dust[beastial].noLight = true;
                }
                beastial = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 173, 0f, 0f, 100, default, 1.5f);
                Main.dust[beastial].velocity *= 2f;
                Main.dust[beastial].noGravity = true;
                if (isInTile)
                {
                    Main.dust[beastial].noLight = true;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 2);
            return false;
        }
    }
}
