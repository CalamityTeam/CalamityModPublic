using CalamityMod.NPCs;
using CalamityMod.NPCs.AcidRain;
using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class CryonicShield : ModProjectile
    {
        public Player Owner => Main.player[Projectile.owner];
        public override string Texture => "CalamityMod/NPCs/Cryogen/CryogenShield";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cryonic Shield");
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 222;
            Projectile.height = 216;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 0f;
            Projectile.timeLeft = 90000;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 25;
        }

        public override void AI()
        {
            // Protect against projectile reflection.
            Projectile.friendly = true;
            Projectile.hostile = false;

            // Spin around.
            Projectile.rotation += MathHelper.Pi / 48f;

            Projectile.Center = Owner.Center;
            Lighting.AddLight(Projectile.Center, Projectile.Opacity * 0.2f, Projectile.Opacity * 0.45f, Projectile.Opacity * 0.5f);

            if (Owner is null || !Owner.active || Owner.dead)
                Projectile.Kill();
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 180);
            target.AddBuff(ModContent.BuffType<GlacialState>(), 30);

            if (target.knockBackResist <= 0f)
                return;

            if (CalamityGlobalNPC.ShouldAffectNPC(target))
            {
                float knockbackMultiplier = knockback - (1f - target.knockBackResist);
                if (knockbackMultiplier < 0)
                    knockbackMultiplier = 0;

                Vector2 trueKnockback = Projectile.SafeDirectionTo(target.Center);
                target.velocity = trueKnockback * knockbackMultiplier;
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 180);
            target.AddBuff(ModContent.BuffType<GlacialState>(), 30);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, Projectile.Size.Length() * 0.5f, targetHitbox);

        public override bool? CanHitNPC(NPC target)
        {
            if (target.catchItem != 0 && target.type != ModContent.NPCType<Radiator>())
                return false;

            return null;
        }
    }
}
