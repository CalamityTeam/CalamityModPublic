using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Healing;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class IllustriousKnife : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] >= 240f)
            {
                Projectile.alpha += 4;
                Projectile.damage = (int)(Projectile.damage * 0.95);
                Projectile.knockBack = (int)(Projectile.knockBack * 0.95);
            }
            if (Projectile.ai[0] < 240f)
                Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathHelper.PiOver2;
            else
            {
                Projectile.rotation += 0.5f;
            }
            CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 300f, 12f, 20f);
            if (Main.rand.NextBool(6))
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 20, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
        }

        //Give it a custom hitbox shape so it may remain rectangular and elongated
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            float bladeHalfLength = 25f * Projectile.scale / 2f;
            float bladeWidth = 14f * Projectile.scale;

            Vector2 direction = (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2();

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - direction * bladeHalfLength, Projectile.Center + direction * bladeHalfLength, bladeWidth, ref collisionPoint);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 3; i++)
            {
                int illustrious = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 20, 0f, 0f, 100, default, 0.8f);
                Main.dust[illustrious].noGravity = true;
                Main.dust[illustrious].velocity *= 1.2f;
                Main.dust[illustrious].velocity -= Projectile.oldVelocity * 0.3f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);

            float healAmt = hit.Damage * 0.015f;
            if ((int)healAmt == 0)
                return;

            if (Main.player[Main.myPlayer].lifeSteal <= 0f)
                return;

            if (healAmt > CalamityMod.lifeStealCap)
                healAmt = CalamityMod.lifeStealCap;

            CalamityGlobalProjectile.SpawnLifeStealProjectile(Projectile, Main.player[Projectile.owner], healAmt, ModContent.ProjectileType<RoyalHeal>(), 1200f, 3f);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);

            float healAmt = info.Damage * 0.015f;
            if ((int)healAmt == 0)
                return;

            if (Main.player[Main.myPlayer].lifeSteal <= 0f)
                return;

            if (healAmt > CalamityMod.lifeStealCap)
                healAmt = CalamityMod.lifeStealCap;

            CalamityGlobalProjectile.SpawnLifeStealProjectile(Projectile, Main.player[Projectile.owner], healAmt, ModContent.ProjectileType<RoyalHeal>(), 1200f, 3f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
