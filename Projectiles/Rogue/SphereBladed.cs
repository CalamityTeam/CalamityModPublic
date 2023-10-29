using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Rogue
{
    public class SphereBladed : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 1f, 0f, 0f);
            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 229, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 100);
            }
            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 8;
                SoundEngine.PlaySound(SoundID.Item7, Projectile.position);
            }
            if (Projectile.ai[0] == 0f)
            {
                Projectile.ai[1] += 1f;
                if (Projectile.ai[1] >= 25f)
                {
                    Projectile.ai[0] = 1f;
                    Projectile.ai[1] = 0f;
                    Projectile.netUpdate = true;
                }
            }
            else
            {
                float acceleration = 3.2f;
                Vector2 vector2 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
                float xdistance = Main.player[Projectile.owner].position.X + (float)(Main.player[Projectile.owner].width / 2) - vector2.X;
                float ydistance = Main.player[Projectile.owner].position.Y + (float)(Main.player[Projectile.owner].height / 2) - vector2.Y;
                float totalDist = (float)Math.Sqrt((double)(xdistance * xdistance + ydistance * ydistance));
                if (totalDist > 3000f)
                {
                    Projectile.Kill();
                }
                totalDist = 16f / totalDist;
                xdistance *= totalDist;
                ydistance *= totalDist;
                if (Projectile.velocity.X < xdistance)
                {
                    Projectile.velocity.X = Projectile.velocity.X + acceleration;
                    if (Projectile.velocity.X < 0f && xdistance > 0f)
                    {
                        Projectile.velocity.X = Projectile.velocity.X + acceleration;
                    }
                }
                else if (Projectile.velocity.X > xdistance)
                {
                    Projectile.velocity.X = Projectile.velocity.X - acceleration;
                    if (Projectile.velocity.X > 0f && xdistance < 0f)
                    {
                        Projectile.velocity.X = Projectile.velocity.X - acceleration;
                    }
                }
                if (Projectile.velocity.Y < ydistance)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + acceleration;
                    if (Projectile.velocity.Y < 0f && ydistance > 0f)
                    {
                        Projectile.velocity.Y = Projectile.velocity.Y + acceleration;
                    }
                }
                else if (Projectile.velocity.Y > ydistance)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - acceleration;
                    if (Projectile.velocity.Y > 0f && ydistance < 0f)
                    {
                        Projectile.velocity.Y = Projectile.velocity.Y - acceleration;
                    }
                }
                if (Main.myPlayer == Projectile.owner)
                {
                    Rectangle rectangle = new Rectangle((int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width, Projectile.height);
                    Rectangle playerPos = new Rectangle((int)Main.player[Projectile.owner].position.X, (int)Main.player[Projectile.owner].position.Y, Main.player[Projectile.owner].width, Main.player[Projectile.owner].height);
                    if (rectangle.Intersects(playerPos))
                    {
                        Projectile.Kill();
                    }
                }
            }
            Projectile.rotation += 0.5f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.NPCHit34, Projectile.position);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
