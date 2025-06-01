using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class FishronRPG : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override void SetStaticDefaults() => ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 95;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void AI()
        {
            if (Projectile.ai[1] == 0f)
            {
                for (int i = 0; i < 5; i++)
                {
                    int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Water, 0f, 0f, 100, default, 2f);
                    Main.dust[dust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[dust].scale = 0.5f;
                        Main.dust[dust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                Projectile.ai[1] = 1f;
                SoundEngine.PlaySound(SoundID.Item92, Projectile.position);
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.owner == Main.myPlayer && Projectile.timeLeft <= 3)
            {
                Projectile.tileCollide = false;
                Projectile.ai[1] = 0f;
                Projectile.alpha = 255;
                Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
                Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
                Projectile.width = 200;
                Projectile.height = 200;
                Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
                Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
                Projectile.knockBack = 10f;
            }
            else
            {
                if (Math.Abs(Projectile.velocity.X) >= 8f || Math.Abs(Projectile.velocity.Y) >= 8f)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        float halfX = 0f;
                        float halfY = 0f;
                        if (j == 1)
                        {
                            halfX = Projectile.velocity.X * 0.5f;
                            halfY = Projectile.velocity.Y * 0.5f;
                        }
                        int fishDust = Dust.NewDust(new Vector2(Projectile.position.X + 3f + halfX, Projectile.position.Y + 3f + halfY) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, DustID.Water, 0f, 0f, 100, default, 1f);
                        Main.dust[fishDust].scale *= 2f + (float)Main.rand.Next(10) * 0.1f;
                        Main.dust[fishDust].velocity *= 0.2f;
                        Main.dust[fishDust].noGravity = true;
                        fishDust = Dust.NewDust(new Vector2(Projectile.position.X + 3f + halfX, Projectile.position.Y + 3f + halfY) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, DustID.Water, 0f, 0f, 100, default, 0.5f);
                        Main.dust[fishDust].fadeIn = 1f + (float)Main.rand.Next(5) * 0.1f;
                        Main.dust[fishDust].velocity *= 0.05f;
                    }
                }
            }
            CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 200f, 12f, 20f);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            return true;
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 48;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();
            SoundEngine.PlaySound(SoundID.Item92, Projectile.position);
            for (int i = 0; i < 10; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Water, 0f, 0f, 100, default, 2f);
                Main.dust[dust].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[dust].scale = 0.5f;
                    Main.dust[dust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int i = 0; i < 20; i++)
            {
                int dusty = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Water, 0f, 0f, 100, default, 3f);
                Main.dust[dusty].noGravity = true;
                Main.dust[dusty].velocity *= 5f;
                dusty = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Water, 0f, 0f, 100, default, 2f);
                Main.dust[dusty].velocity *= 2f;
            }
        }
    }
}
