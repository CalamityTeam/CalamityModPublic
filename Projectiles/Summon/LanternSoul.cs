using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class LanternSoul : ModProjectile
    {
        public float count = 0f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lantern");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.sentry = true;
            Projectile.timeLeft = Projectile.SentryLifeTime;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 1f, 0f, 0f);
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= 4)
            {
                Projectile.frame = 0;
            }

            Player player = Main.player[Projectile.owner];
            Projectile.velocity = new Vector2(0f, (float)Math.Sin((double)(6.28318548f * Projectile.ai[0] / 300f)) * 0.5f);
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] >= 300f)
            {
                Projectile.ai[0] = 0f;
                Projectile.netUpdate = true;
            }

            int num = 0;
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].type == ModContent.ProjectileType<LanternFlame>())
                {
                    num++;
                }
            }
            if (num < 20)
            {
                Projectile.ai[1] += (float)Main.rand.Next(2,6) + 1f;
                if (Projectile.ai[1] >= 75f)
                {
                    Projectile.ai[1] = 0f;
                    Projectile.netUpdate = true;
                    if (Projectile.owner == Main.myPlayer)
                    {
                        float startOffsetX = Main.rand.NextFloat(15f, 200f) * (Main.rand.NextBool() ? -1f : 1f);
                        float startOffsetY = Main.rand.NextFloat(15f, 200f) * (Main.rand.NextBool() ? -1f : 1f);
                        Vector2 startPos = new Vector2(Projectile.position.X + startOffsetX, Projectile.position.Y + startOffsetY);
                        Vector2 speed = new Vector2(0f, 0f);
                        int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), startPos, speed, ModContent.ProjectileType<LanternFlame>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
                        if (Main.projectile.IndexInRange(p))
                            Main.projectile[p].originalDamage = Projectile.originalDamage;
                    }
                }
            }
        }

        public override bool? CanDamage() => false;
    }
}
