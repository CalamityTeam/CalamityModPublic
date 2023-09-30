using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class ToxicannonShot : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/Enemy/FlakAcid";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.timeLeft = 480;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
        }

        public override void AI()
        {
            if (Projectile.ai[0]++ <= 30f)
            {
                Projectile.alpha = (int)MathHelper.Lerp(255f, 0f, Projectile.ai[0] / 30f);
            }
            if (Projectile.velocity.Y < 20f)
                Projectile.velocity.Y += 0.3f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (Math.Sign(Projectile.velocity.Y) != Math.Sign(Projectile.oldVelocity.Y) && Projectile.ai[0] >= 5f)
            {
                Projectile.Kill();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 240);
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.ExpandHitboxBy(180);
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.damage /= 2;
            Projectile.Damage();
            for (int i = 0; i <= 40; i++)
            {
                int idx = Dust.NewDust(Projectile.position, 230, 230, (int)CalamityDusts.SulfurousSeaAcid, 0, 0, 0, default, 0.75f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * (Main.dust[idx].position - Projectile.Center).Length() / 25f;
                Main.dust[idx].scale = 2.5f;
            }
            for (int i = 0; i <= 90; i++)
            {
                int idx = Dust.NewDust(Projectile.Center, 0, 0, (int)CalamityDusts.SulfurousSeaAcid);
                Main.dust[idx].velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * 12f;
                Main.dust[idx].scale = 3f;
                Main.dust[idx].noGravity = true;
            }
            float count = Main.rand.Next(1, 2 + 1);
            for (int i = 0; i < count; i++)
            {
                float angle = MathHelper.TwoPi * i / count;
                angle += Main.rand.NextFloat(0.1f, 0.35f) * Main.rand.NextBool().ToDirectionInt();
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, angle.ToRotationVector2() * Main.rand.NextFloat(4f, 16f),
                        ModContent.ProjectileType<ToxicannonDrop>(), Projectile.damage, 2.5f, Projectile.owner);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
