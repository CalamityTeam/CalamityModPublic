using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class DraconicSpark : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public static int Lifetime = 120;
        public static float MaxHomingRange = 600f;
        public static float HomingVelocity = 20f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Draconic Spark");
        }

        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.extraUpdates = 1;
            projectile.timeLeft = Lifetime;
            projectile.alpha = 255;
        }

        // ai[0] controls what type of Draconic Spark this is.
        // 0 = Orange spark (inflicts Daybroken)
        // 1 = Red spark (inflicts Abyssal Flames)
        // 2 = Yellow spark (inflicts Holy Flames)
        public override void AI()
        {
            DrawProjectile();

            // Homing code copied from Arch Amaryllis
            ArchAmaryllisHoming();
        }

        private void ArchAmaryllisHoming()
        {
            float targetX = projectile.Center.X;
            float targetY = projectile.Center.Y;
            bool foundTarget = false;
            float maxRange = MaxHomingRange;
            for (int i = 0; i < 200; i++)
            {
                if (Main.npc[i].CanBeChasedBy(projectile, false) && Collision.CanHit(projectile.Center, 1, 1, Main.npc[i].Center, 1, 1))
                {
                    float iterCenterX = Main.npc[i].position.X + (float)(Main.npc[i].width / 2);
                    float iterCenterY = Main.npc[i].position.Y + (float)(Main.npc[i].height / 2);
                    float dist = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - iterCenterX) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - iterCenterY);
                    if (dist < maxRange)
                    {
                        maxRange = dist;
                        targetX = iterCenterX;
                        targetY = iterCenterY;
                        foundTarget = true;
                    }
                }
            }
            if (foundTarget)
            {
                float speed = HomingVelocity;
                Vector2 projCenter = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                float xDist = targetX - projCenter.X;
                float yDist = targetY - projCenter.Y;
                float dist = (float)Math.Sqrt((double)(xDist * xDist + yDist * yDist));
                dist = speed / dist;
                xDist *= dist;
                yDist *= dist;
                projectile.velocity.X = (projectile.velocity.X * 20f + xDist) / 21f;
                projectile.velocity.Y = (projectile.velocity.Y * 20f + yDist) / 21f;
                return;
            }
        }

        // Debuff applied depends on spark color.
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.ai[0] == 0f)
                target.AddBuff(BuffID.Daybreak, 180);
            else if (projectile.ai[0] == 1f)
                target.AddBuff(ModContent.BuffType<AbyssalFlames>(), 180);
            else if (projectile.ai[0] == 2f)
                target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
        }

        // Pure dust projectile, but dust used depends on AI variables
        private void DrawProjectile()
        {
            if (projectile.ai[0] == 0) // daybroken spark
            {
                int dustID = 244;
                if (Main.rand.Next(3) != 0)
                {
                    float scale = Main.rand.NextFloat(0.8f, 1.4f);
                    int idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustID);
                    Main.dust[idx].noGravity = true;
                    Main.dust[idx].velocity += projectile.velocity * 0.1f;
                }
            }
            else if (projectile.ai[0] == 1) // abyssal flames spark
            {
                int dustID = 235;
                if (Main.rand.Next(3) != 0)
                {
                    float scale = Main.rand.NextFloat(0.6f, 1.2f);
                    int idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustID, 0f, 0f, 100, default, scale);
                    Main.dust[idx].noGravity = true;
                    Main.dust[idx].velocity += projectile.velocity * 0.1f;
                }
            }
            else if (projectile.ai[0] == 2) // holy flames spark
            {
                int dustID = 246;
                if (Main.rand.Next(3) != 0)
                {
                    float scale = Main.rand.NextFloat(0.8f, 1.4f);
                    int idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustID);
                    Main.dust[idx].noGravity = true;
                    Main.dust[idx].velocity += projectile.velocity * 0.1f;
                }
            }
        }
    }
}
