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
        public Player Owner => Main.player[projectile.owner];
        public override string Texture => "CalamityMod/NPCs/Cryogen/CryogenIce";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cryonic Shield");
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 222;
            projectile.height = 216;
            projectile.ignoreWater = true;
            projectile.minionSlots = 0f;
            projectile.timeLeft = 90000;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 25;
        }

        public override void AI()
        {
            // Protect against projectile reflection.
            projectile.friendly = true;
            projectile.hostile = false;

            // Spin around.
            projectile.rotation += MathHelper.Pi / 48f;

            projectile.Center = Owner.Center;
            Lighting.AddLight(projectile.Center, projectile.Opacity * 0.2f, projectile.Opacity * 0.45f, projectile.Opacity * 0.5f);

            if (Owner is null || !Owner.active || Owner.dead)
                projectile.Kill();
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

                Vector2 trueKnockback = projectile.SafeDirectionTo(target.Center);
                target.velocity = trueKnockback * knockbackMultiplier;
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 180);
            target.AddBuff(ModContent.BuffType<GlacialState>(), 30);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(projectile.Center, projectile.Size.Length() * 0.5f, targetHitbox);

        public override bool? CanHitNPC(NPC target)
        {
            if (target.catchItem != 0 && target.type != ModContent.NPCType<Radiator>())
                return false;

            return null;
        }
    }
}
