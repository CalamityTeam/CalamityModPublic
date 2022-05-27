using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Boss
{
    public class OldDukeGore : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sulphurous Gore");
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
            Projectile.timeLeft = 300;
            Projectile.alpha = 255;
            CooldownSlot = 1;
        }

        public override void AI()
        {
            Projectile.alpha -= 50;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;

            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] >= 15f)
            {
                Projectile.velocity.Y += 0.1f;
            }

            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }

            Projectile.tileCollide = Projectile.timeLeft < 240;

            Projectile.rotation += Projectile.velocity.X * 0.1f;

            int blood = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0f, 0f, 100, default, 1f);
            Main.dust[blood].noGravity = true;
            Main.dust[blood].velocity *= 0f;

            int acid = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, (int)CalamityDusts.SulfurousSeaAcid, 0f, 0f, 100, default, 1f);
            Main.dust[acid].noGravity = true;
            Main.dust[acid].velocity *= 0f;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath12, Projectile.position);

            int num226 = 8;
            for (int num227 = 0; num227 < num226; num227++)
            {
                Vector2 source = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                source = source.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + Projectile.Center;
                Vector2 dustVel = source - Projectile.Center;
                int blood = Dust.NewDust(source + dustVel, 0, 0, DustID.Blood, dustVel.X, dustVel.Y, 100, default, 1.2f);
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

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            target.Calamity().lastProjectileHit = Projectile;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
