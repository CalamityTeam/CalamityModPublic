using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Boss
{
    public class OldDukeGore : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 420;
            Projectile.alpha = 255;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            Lighting.AddLight((int)((Projectile.position.X + (Projectile.width / 2)) / 16f), (int)((Projectile.position.Y + (Projectile.height / 2)) / 16f), 0.5f, 0.4f, 0f);

            Projectile.alpha -= 50;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;

            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] >= 15f)
                Projectile.velocity.Y += 0.1f;

            if (Projectile.velocity.Y > 12f)
                Projectile.velocity.Y = 12f;

            Projectile.tileCollide = Projectile.timeLeft < 300;

            Projectile.rotation += Projectile.velocity.X * 0.1f;

            int blood = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0f, 0f, 100, default, 1f);
            Main.dust[blood].noGravity = true;
            Main.dust[blood].velocity *= 0f;

            int acid = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, (int)CalamityDusts.SulfurousSeaAcid, 0f, 0f, 100, default, 1f);
            Main.dust[acid].noGravity = true;
            Main.dust[acid].velocity *= 0f;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath12, Projectile.Center);

            int dustAmt = 8;
            for (int i = 0; i < dustAmt; i++)
            {
                Vector2 dustRotation = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                dustRotation = dustRotation.RotatedBy((double)((float)(i - (dustAmt / 2 - 1)) * 6.28318548f / (float)dustAmt), default) + Projectile.Center;
                Vector2 dustVel = dustRotation - Projectile.Center;
                int blood = Dust.NewDust(dustRotation + dustVel, 0, 0, DustID.Blood, dustVel.X, dustVel.Y, 100, default, 1.2f);
                Main.dust[blood].noGravity = true;
                Main.dust[blood].noLight = true;
                Main.dust[blood].velocity = dustVel;
            }

            for (int d = 0; d < 6; d++)
            {
                int acid = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, (int)CalamityDusts.SulfurousSeaAcid, 0f, 0f, 100, default(Color), 3f);
                Main.dust[acid].noGravity = true;
                Main.dust[acid].velocity *= 5f;
                acid = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, (int)CalamityDusts.SulfurousSeaAcid, 0f, 0f, 100, default(Color), 2f);
                Main.dust[acid].velocity *= 2f;
                Main.dust[acid].noGravity = true;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
