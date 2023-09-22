using CalamityMod.CalPlayer;
using CalamityMod.NPCs;
using CalamityMod.NPCs.AcidRain;
using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class CryonicShield : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public Player Owner => Main.player[Projectile.owner];
        public override string Texture => "CalamityMod/NPCs/Cryogen/CryogenShield";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 222;
            Projectile.height = 216;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 90000;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 25;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            // Protect against projectile reflection.
            Projectile.friendly = true;
            Projectile.hostile = false;

            // Spin around.
            Projectile.rotation += MathHelper.Pi / 48f;

            Projectile.Center = Owner.Center;
            if (!modPlayer.CryoStoneVanity)
                Lighting.AddLight(Projectile.Center, Projectile.Opacity * 0.2f, Projectile.Opacity * 0.45f, Projectile.Opacity * 0.5f);

            if (Owner is null || !Owner.active || Owner.dead)
                Projectile.Kill();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            if (!modPlayer.CryoStoneVanity)
            {
                target.AddBuff(BuffID.Frostburn2, 180);
                target.AddBuff(ModContent.BuffType<GlacialState>(), 30);

                if (target.knockBackResist <= 0f)
                    return;

                // 12AUG2023: Ozzatron: TML was giving NaN knockback, probably due to 0 base knockback. Do not use hit.Knockback
                if (CalamityGlobalNPC.ShouldAffectNPC(target))
                {
                    float knockbackMultiplier = MathHelper.Clamp(1f - target.knockBackResist, 0f, 1f);
                    Vector2 trueKnockback = target.Center - Projectile.Center;
                    trueKnockback.Normalize();
                    target.velocity = trueKnockback * knockbackMultiplier;
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (!modPlayer.CryoStoneVanity)
            {
                target.AddBuff(BuffID.Frostburn2, 180);
                target.AddBuff(ModContent.BuffType<GlacialState>(), 30);
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, Projectile.Size.Length() * 0.5f, targetHitbox);

        public override bool? CanHitNPC(NPC target)
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if ((target.catchItem != 0 && target.type != ModContent.NPCType<Radiator>()) || modPlayer.CryoStoneVanity)
                return false;

            return null;
        }
    }
}
