using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Rogue
{
    public class DesecratedBubble : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 120;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            Projectile.scale += 0.002f;
            if (Projectile.alpha <= 0)
            {
                Projectile.alpha = 0;
            }
            else if (Projectile.alpha > 50)
            {
                Projectile.alpha -= 20;
            }
            if (Projectile.timeLeft <= 100)
            {
                Projectile.ai[1] = 0f;
            }
            else
            {
                Projectile.velocity *= 0.995f;
            }
            if (Main.player[Projectile.owner].active && !Main.player[Projectile.owner].dead)
            {
                if (Projectile.ai[1] == 0f)
                {
                    CalamityUtils.HomeInOnNPC(Projectile, true, 200f, 8f, 20f);
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item54, Projectile.position);
            int rando = Main.rand.Next(5, 9);
            for (int i = 0; i < rando; i++)
            {
                int dust = Dust.NewDust(Projectile.Center, 0, 0, 179, 0f, 0f, 100, default, 1.4f);
                Main.dust[dust].velocity *= 0.8f;
                Main.dust[dust].position = Vector2.Lerp(Main.dust[dust].position, Projectile.Center, 0.5f);
                Main.dust[dust].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[0] == 1f)
            {
                target.AddBuff(BuffID.Ichor, 180);
                target.AddBuff(BuffID.CursedInferno, 180);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Projectile.ai[0] == 1f)
            {
                target.AddBuff(BuffID.Ichor, 180);
                target.AddBuff(BuffID.CursedInferno, 180);
            }
        }

        // Cannot deal damage for the first several frames of existence.
        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.timeLeft >= 100)
            {
                return false;
            }
            return null;
        }

        public override bool CanHitPvp(Player target) => Projectile.timeLeft < 100;
    }
}
