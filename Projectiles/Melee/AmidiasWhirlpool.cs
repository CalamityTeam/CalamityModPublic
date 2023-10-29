using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class AmidiasWhirlpool : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 58;
            Projectile.height = 58;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.alpha = 100;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void AI()
        {
            for (int i = 0; i < 2; i++)
            {
                float shortXVel = Projectile.velocity.X / 3f * (float)i;
                float shortYVel = Projectile.velocity.Y / 3f * (float)i;
                int fourConst = 4;
                int waterDust = Dust.NewDust(new Vector2(Projectile.position.X + (float)fourConst, Projectile.position.Y + (float)fourConst), Projectile.width - fourConst * 2, Projectile.height - fourConst * 2, 33, 0f, 0f, 0, new Color(0, 142, 255), 1.5f);
                Dust dust = Main.dust[waterDust];
                dust.noGravity = true;
                dust.velocity *= 0.1f;
                dust.velocity += Projectile.velocity * 0.1f;
                dust.position.X -= shortXVel;
                dust.position.Y -= shortYVel;
            }
            Projectile.ai[0] += 1f;
            int homeTracker = 0;
            if (Projectile.velocity.Length() <= 8f) //4
            {
                homeTracker = 1;
            }
            if (homeTracker == 0)
            {
                Projectile.rotation -= 0.104719758f;

                if (Projectile.ai[0] >= 30f)
                {
                    Projectile.velocity *= 0.98f;
                    Projectile.rotation -= 0.0174532924f;
                }
                if (Projectile.velocity.Length() < 8.2f) //4.1
                {
                    Projectile.velocity.Normalize();
                    Projectile.velocity *= 4f;
                    Projectile.ai[0] = 0f;
                }
            }
            else if (homeTracker == 1)
            {
                int inc;
                Projectile.rotation -= 0.104719758f;
                Vector2 projCenter = Projectile.Center;
                float homingRange = 150f;
                bool isHoming = false;
                int npcTracker = 0;
                if (Projectile.ai[1] == 0f)
                {
                    for (int j = 0; j < Main.maxNPCs; j = inc + 1)
                    {
                        if (Main.npc[j].CanBeChasedBy(Projectile, false))
                        {
                            Vector2 npcCenter = Main.npc[j].Center;
                            if (Projectile.Distance(npcCenter) < homingRange && Collision.CanHit(new Vector2(Projectile.position.X + (float)(Projectile.width / 2), Projectile.position.Y + (float)(Projectile.height / 2)), 1, 1, Main.npc[j].position, Main.npc[j].width, Main.npc[j].height))
                            {
                                homingRange = Projectile.Distance(npcCenter);
                                projCenter = npcCenter;
                                isHoming = true;
                                npcTracker = j;
                                break;
                            }
                        }
                        inc = j;
                    }
                    if (isHoming)
                    {
                        if (Projectile.ai[1] != (float)(npcTracker + 1))
                        {
                            Projectile.netUpdate = true;
                        }
                        Projectile.ai[1] = (float)(npcTracker + 1);
                    }
                    isHoming = false;
                }
                if (Projectile.ai[1] != 0f)
                {
                    int npcTrackAgain = (int)(Projectile.ai[1] - 1f);
                    if (Main.npc[npcTrackAgain].active && Main.npc[npcTrackAgain].CanBeChasedBy(Projectile, true) && Projectile.Distance(Main.npc[npcTrackAgain].Center) < 1000f)
                    {
                        isHoming = true;
                        projCenter = Main.npc[npcTrackAgain].Center;
                    }
                }
                if (!Projectile.friendly)
                {
                    isHoming = false;
                }
                if (isHoming)
                {
                    int waterDust0 = 10;
                    Vector2 dustDirection = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
                    float waterDust1 = projCenter.X - dustDirection.X;
                    float waterDust2 = projCenter.Y - dustDirection.Y;
                    float waterDust3 = (float)Math.Sqrt((double)(waterDust1 * waterDust1 + waterDust2 * waterDust2));
                    waterDust3 = 24f / waterDust3;
                    waterDust1 *= waterDust3;
                    waterDust2 *= waterDust3;
                    Projectile.velocity.X = (Projectile.velocity.X * (float)(waterDust0 - 1) + waterDust1) / (float)waterDust0;
                    Projectile.velocity.Y = (Projectile.velocity.Y * (float)(waterDust0 - 1) + waterDust2) / (float)waterDust0;
                }
            }
            Lighting.AddLight(Projectile.Center, 0f, 0.1f, 0.9f);
            if (Projectile.ai[0] >= 120f)
            {
                Projectile.Kill();
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(30, 255, 253);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int k = 0; k < 20; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 33, Projectile.oldVelocity.X, Projectile.oldVelocity.Y, 0, new Color(0, 142, 255), 1f);
            }
        }
    }
}
