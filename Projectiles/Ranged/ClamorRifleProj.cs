using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;

namespace CalamityMod.Projectiles.Ranged
{
    public class ClamorRifleProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
        }

        public override void AI()
        {
            Projectile.rotation += 0.15f;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 3f)
            {
                Lighting.AddLight(Projectile.Center, new Vector3(44, 191, 232) * (1.3f / 255));
                for (int i = 0; i < 2; i++)
                {
                    int blueDust = Dust.NewDust(new Vector2(Projectile.Center.X, Projectile.Center.Y), Projectile.width - 28, Projectile.height - 28, 68, 0f, 0f, 100, default, 1f);
                    Main.dust[blueDust].noGravity = true;
                    Main.dust[blueDust].velocity *= 0.1f;
                    Main.dust[blueDust].velocity += Projectile.velocity * 0.5f;
                }
            }
            CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 150f, 12f, 25f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Eutrophication>(), 180);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<Eutrophication>(), 180);

        public override void OnKill(int timeLeft)
        {
            int bulletAmt = 2;
            if (Projectile.owner == Main.myPlayer)
            {
                for (int b = 0; b < bulletAmt; b++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<ClamorRifleProjSplit>(), (int)(Projectile.damage * 0.4), 0f, Projectile.owner, 0f, 0f);
                }
            }
            SoundEngine.PlaySound(SoundID.Item118, Projectile.Center);
        }
    }
}
