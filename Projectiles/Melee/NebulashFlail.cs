using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Projectiles.BaseProjectiles;

namespace CalamityMod.Projectiles.Melee
{
    public class NebulashFlail : BaseWhipProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nebulash");
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.melee = true;
            projectile.ignoreWater = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 3;
        }
        public override Color SpecialDrawColor => new Color(255, 200, 0);
        public override int ExudeDustType => ModContent.DustType<AstralOrange>();
        public override int WhipDustType => ModContent.DustType<AstralOrange>();
        public override int HandleHeight => 60;
        public override int BodyType1StartY => 64;
        public override int BodyType1SectionHeight => 18;
        public override int BodyType2StartY => 86;
        public override int BodyType2SectionHeight => 18;
        public override int TailStartY => 108;
        public override int TailHeight => 50;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }
            float num8 = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + projectile.velocity, 16f * projectile.scale, ref num8))
            {
                return true;
            }
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 240);
            if (projectile.localAI[1] <= 0f && projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, ModContent.ProjectileType<Nebudust>(), projectile.damage / 2, knockback, projectile.owner, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
            }
            projectile.localAI[1] = 4f;
        }
    }
}
