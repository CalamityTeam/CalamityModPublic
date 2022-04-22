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
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 3;
        }
        public override Color SpecialDrawColor => new(255, 200, 0);
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
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity, 16f * Projectile.scale, ref num8))
            {
                return true;
            }
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 240);
            if (Projectile.localAI[1] <= 0f && Projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center.X, target.Center.Y, 0f, 0f, ModContent.ProjectileType<Nebudust>(), Projectile.damage / 2, knockback, Projectile.owner, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
            }
            Projectile.localAI[1] = 4f;
        }
    }
}
