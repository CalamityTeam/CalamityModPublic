using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class ElementBallShiv : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 2;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
            Projectile.aiStyle = ProjAIStyleID.Beam;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            int rainbow = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 66, (float)(Projectile.direction * 2), 0f, 150, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1.3f);
            Main.dust[rainbow].noGravity = true;
            Main.dust[rainbow].velocity = Vector2.Zero;

            CalamityUtils.HomeInOnNPC(Projectile, true, 300f, 12f, 20f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > 50)
                return false;

            return true;
        }

        public override void OnKill(int timeLeft)
        {
            for (int k = 0; k < 2; k++)
            {
                int rainbow = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 66, (float)(Projectile.direction * 2), 0f, 150, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1f);
                Main.dust[rainbow].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ElementalMix>(), 30);
            OnHitEffects(target.Center);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            OnHitEffects(target.Center);
        }

        private void OnHitEffects(Vector2 targetPos)
        {
            var source = Projectile.GetSource_FromThis();
            for (int x = 0; x < 3; x++)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    CalamityUtils.ProjectileBarrage(source, Projectile.Center, targetPos, x > 2, 800f, 800f, 0f, 800f, 20f, ModContent.ProjectileType<SHIV>(), Projectile.damage, Projectile.knockBack, Projectile.owner, false, 50f);
                }
            }
        }
    }
}
