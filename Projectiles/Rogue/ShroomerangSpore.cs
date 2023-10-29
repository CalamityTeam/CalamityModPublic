using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class ShroomerangSpore : BaseSporeSacProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Projectiles/Ranged/FungiOrb";

        private bool initialized = false;
        public override Color? LightColor => new Color(0f, 0.35f, 0.5f);
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 3600;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override bool PreAI()
        {
            if (!initialized)
            {
                initialized = true;
                Projectile.localAI[0] = Main.rand.Next(-60, 61); //used for mycoroot stealth strike for minor randomization
            }

            return true;
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.ExpandHitboxBy(56);
            int constant = 36;
            for (int i = 0; i < constant; i++)
            {
                Vector2 rotate = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                rotate = rotate.RotatedBy((double)((float)(i - (constant / 2 - 1)) * 6.28318548f / (float)constant), default) + Projectile.Center;
                Vector2 facingDirection = rotate - Projectile.Center;
                int dust = Dust.NewDust(rotate + facingDirection, 0, 0, 56, facingDirection.X, facingDirection.Y, 100, default, 0.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].noLight = true;
                Main.dust[dust].velocity = facingDirection;
            }
        }
    }
}
