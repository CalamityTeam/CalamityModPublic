using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class EnergyOrb : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Magic/BlueBubble";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Energy Orb");
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.alpha = 100;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 200;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 4f)
            {
                int num469 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 132, 0f, 0f, 100, default, 1f);
                if (Main.rand.Next(6) != 0)
                    Main.dust[num469].noGravity = true;
                Main.dust[num469].velocity *= 0f;
            }
            NPC potentialTarget = Projectile.Center.MinionHoming(400f, Main.player[Projectile.owner]);
            if (potentialTarget != null)
                Projectile.velocity = (Projectile.velocity * 20f + Projectile.SafeDirectionTo(potentialTarget.Center) * 10f) / 21f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Electrified, 120);
        }
    }
}
