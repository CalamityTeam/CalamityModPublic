using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class MonstrousKnife : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.aiStyle = ProjAIStyleID.ThrownProjectile;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] >= 30f)
            {
                Projectile.alpha += 10;
                if (Projectile.damage > 1)
                    Projectile.damage = (int)(Projectile.damage * 0.9);
                Projectile.knockBack = Projectile.knockBack * 0.9f;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
            }
            if (Projectile.ai[0] < 30f)
                Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + MathHelper.PiOver2;
        }

        public override void OnKill(int timeLeft)
        {
            for (int dustIndex = 0; dustIndex < 3; ++dustIndex)
            {
                int redDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 182, 0f, 0f, 100, new Color(), 0.8f);
                Dust dust = Main.dust[redDust];
                dust.noGravity = true;
                dust.velocity *= 1.2f;
                dust.velocity -= Projectile.oldVelocity * 0.3f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.myPlayer != Projectile.owner)
                return;

            if (target.lifeMax <= 5 || Main.player[Projectile.owner].moonLeech)
                return;

            if (Main.player[Projectile.owner].lifeSteal <= 0f)
                return;

            float healAmt = hit.Damage * Main.rand.NextFloat(0.075f, 0.9f);
            if (healAmt < 1f)
                healAmt = 1f;

            if (healAmt > CalamityMod.lifeStealCap)
                healAmt = CalamityMod.lifeStealCap;

            if (Main.rand.NextBool(3))
                CalamityGlobalProjectile.SpawnLifeStealProjectile(Projectile, Main.player[Projectile.owner], healAmt, ProjectileID.VampireHeal, 1200f, 3f);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Main.myPlayer != Projectile.owner)
                return;

            if (Main.player[Projectile.owner].moonLeech)
                return;

            if (Main.player[Projectile.owner].lifeSteal <= 0f)
                return;

            float healAmt = info.Damage * Main.rand.NextFloat(0.075f, 0.9f);
            if (healAmt < 1f)
                healAmt = 1f;

            if (healAmt > CalamityMod.lifeStealCap)
                healAmt = CalamityMod.lifeStealCap;

            if (Main.rand.NextBool(3))
                CalamityGlobalProjectile.SpawnLifeStealProjectile(Projectile, Main.player[Projectile.owner], healAmt, ProjectileID.VampireHeal, 1200f, 3f);
        }
    }
}
