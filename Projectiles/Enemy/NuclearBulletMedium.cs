using Microsoft.Xna.Framework;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Enemy
{
    public class NuclearBulletMedium : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Enemy";
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 12;
            Projectile.hostile = true;
            Projectile.timeLeft = 360;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 1.25f);
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.ai[0]++;
            if (Projectile.ai[0] < 85f)
            {
                Projectile.velocity.Y += Math.Sign(Projectile.localAI[0]) * 0.1f;
            }
            if (Projectile.ai[0] == 160f)
            {
                Projectile.ai[1] = Player.FindClosest(Projectile.Center, 1, 1);
            }
            if (Projectile.ai[0] >= 160f && Projectile.ai[0] <= 200f)
            {
                Player player = Main.player[(int)Projectile.ai[1]];
                float inertia = 10f;
                if (Projectile.Distance(player.Center) > 70f)
                    Projectile.velocity = (Projectile.velocity * inertia + Projectile.SafeDirectionTo(player.Center) * 18.5f) / (inertia + 1f);

                Projectile.tileCollide = true;
            }
            else
            {
                Projectile.tileCollide = false;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i <= 2; i++)
            {
                int idx = Dust.NewDust(Projectile.position, 8, 8, (int)CalamityDusts.SulfurousSeaAcid, 0, 0, 0, default, 0.75f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 3f;
                idx = Dust.NewDust(Projectile.position, 8, 8, (int)CalamityDusts.SulfurousSeaAcid, 0, 0, 0, default, 0.75f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 3f;
            }
        }
    }
}
