using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using CalamityMod.Projectiles.BaseProjectiles;

namespace CalamityMod.Projectiles.Melee.Shortswords
{
    public class SubmarineShockerProj: BaseShortswordProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Melee/SubmarineShocker";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Submarine Shocker");
        }

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(16); // This sets width and height to the same value (important when projectiles can rotate)
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.scale = 1f;
            Projectile.DamageType = TrueMeleeDamageClass.Instance;
            Projectile.ownerHitCheck = true;
            Projectile.timeLeft = 360;
            Projectile.hide = true;
            Projectile.ownerHitCheck = true;
        }

        public override void SetVisualOffsets()
        {
            const int HalfSpriteWidth = 32 / 2;
            const int HalfSpriteHeight = 32 / 2;

            int HalfProjWidth = Projectile.width / 2;
            int HalfProjHeight = Projectile.height / 2;

            DrawOriginOffsetX = 0;
            DrawOffsetX = -(HalfSpriteWidth - HalfProjWidth);
            DrawOriginOffsetY = -(HalfSpriteHeight - HalfProjHeight);
        }

        public override void ExtraBehavior()
        {
            if (Main.rand.NextBool(5))
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Electric);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            var source = Projectile.GetSource_FromThis();
            if (crit)
                damage /= 2;
            Projectile.NewProjectile(source, target.Center, Vector2.Zero, ModContent.ProjectileType<Spark>(), (int)(damage * 0.7f), knockback, Main.myPlayer);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            var source = Projectile.GetSource_FromThis();
            if (crit)
                damage /= 2;
            Projectile.NewProjectile(source, target.Center, Vector2.Zero, ModContent.ProjectileType<Spark>(), (int)(damage * 0.7f), Projectile.knockBack, Main.myPlayer);
        }
    }
}
