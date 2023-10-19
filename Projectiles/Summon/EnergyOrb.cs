using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class EnergyOrb : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override string Texture => "CalamityMod/Projectiles/Magic/BlueBubble";

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.alpha = 100;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 200;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 4f)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 132, 0f, 0f, 100, default, 1f);
                if (Main.rand.Next(6) != 0)
                    Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 0f;
            }
            NPC potentialTarget = Projectile.Center.MinionHoming(400f, Main.player[Projectile.owner]);
            if (potentialTarget != null)
                Projectile.velocity = (Projectile.velocity * 20f + Projectile.SafeDirectionTo(potentialTarget.Center) * 10f) / 21f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Electrified, 120);
    }
}
