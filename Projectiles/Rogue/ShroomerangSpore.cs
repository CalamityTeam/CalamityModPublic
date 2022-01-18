using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityMod.Projectiles.Rogue
{
    public class ShroomerangSpore : BaseSporeSacProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Ranged/FungiOrb";

        private bool initialized = false;
        public override Color? LightColor => new Color(0f, 0.35f, 0.5f);
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spore");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 3600;
            projectile.Calamity().rogue = true;
        }

        public override bool PreAI()
        {
            if (!initialized)
            {
                initialized = true;
                projectile.localAI[0] = Main.rand.Next(-60, 61); //used for mycoroot stealth strike for minor randomization
            }

            return true;
        }

        public override void Kill(int timeLeft)
        {
            CalamityGlobalProjectile.ExpandHitboxBy(projectile, 56);
            int num226 = 36;
            for (int num227 = 0; num227 < num226; num227++)
            {
                Vector2 vector6 = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + projectile.Center;
                Vector2 vector7 = vector6 - projectile.Center;
                int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 56, vector7.X, vector7.Y, 100, default, 0.5f);
                Main.dust[num228].noGravity = true;
                Main.dust[num228].noLight = true;
                Main.dust[num228].velocity = vector7;
            }
        }
    }
}
