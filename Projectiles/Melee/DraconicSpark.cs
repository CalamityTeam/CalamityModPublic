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
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = Lifetime;
            Projectile.alpha = 255;
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
            float targetX = Projectile.Center.X;
            float targetY = Projectile.Center.Y;
            bool foundTarget = false;
            float maxRange = MaxHomingRange;
            for (int i = 0; i < 200; i++)
            {
                if (Main.npc[i].CanBeChasedBy(Projectile, false) && Collision.CanHit(Projectile.Center, 1, 1, Main.npc[i].Center, 1, 1))
                {
                    float iterCenterX = Main.npc[i].position.X + (float)(Main.npc[i].width / 2);
                    float iterCenterY = Main.npc[i].position.Y + (float)(Main.npc[i].height / 2);
                    float dist = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - iterCenterX) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - iterCenterY);
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
                Vector2 projCenter = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
                float xDist = targetX - projCenter.X;
                float yDist = targetY - projCenter.Y;
                float dist = (float)Math.Sqrt((double)(xDist * xDist + yDist * yDist));
                dist = speed / dist;
                xDist *= dist;
                yDist *= dist;
                Projectile.velocity.X = (Projectile.velocity.X * 20f + xDist) / 21f;
                Projectile.velocity.Y = (Projectile.velocity.Y * 20f + yDist) / 21f;
                return;
            }
        }

        // Debuff applied depends on spark color.
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Projectile.ai[0] == 0f)
                target.AddBuff(BuffID.Daybreak, 180);
            else if (Projectile.ai[0] == 2f)
                target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
        }

        // Pure dust projectile, but dust used depends on AI variables
        private void DrawProjectile()
        {
            if (Projectile.ai[0] == 0) // daybroken spark
            {
                int dustID = 244;
                if (Main.rand.Next(3) != 0)
                {
                    float scale = Main.rand.NextFloat(0.8f, 1.4f);
                    int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustID);
                    Main.dust[idx].noGravity = true;
                    Main.dust[idx].velocity += Projectile.velocity * 0.1f;
                }
            }
            else if (Projectile.ai[0] == 1) // abyssal flames spark
            {
                int dustID = 235;
                if (Main.rand.Next(3) != 0)
                {
                    float scale = Main.rand.NextFloat(0.6f, 1.2f);
                    int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustID, 0f, 0f, 100, default, scale);
                    Main.dust[idx].noGravity = true;
                    Main.dust[idx].velocity += Projectile.velocity * 0.1f;
                }
            }
            else if (Projectile.ai[0] == 2) // holy flames spark
            {
                int dustID = 246;
                if (Main.rand.Next(3) != 0)
                {
                    float scale = Main.rand.NextFloat(0.8f, 1.4f);
                    int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustID);
                    Main.dust[idx].noGravity = true;
                    Main.dust[idx].velocity += Projectile.velocity * 0.1f;
                }
            }
        }
    }
}
