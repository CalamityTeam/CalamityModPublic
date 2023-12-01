using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Boss
{
    public class HellfireExplosion : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 130;
            Projectile.height = 130;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.75f, 0f, 0f);
            if (Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
                Projectile.localAI[0] += 1f;
            }
            bool xflag = false;
            bool yflag = false;
            if (Projectile.velocity.X < 0f && Projectile.position.X < Projectile.ai[0])
            {
                xflag = true;
            }
            if (Projectile.velocity.X > 0f && Projectile.position.X > Projectile.ai[0])
            {
                xflag = true;
            }
            if (Projectile.velocity.Y < 0f && Projectile.position.Y < Projectile.ai[1])
            {
                yflag = true;
            }
            if (Projectile.velocity.Y > 0f && Projectile.position.Y > Projectile.ai[1])
            {
                yflag = true;
            }
            if (xflag && yflag)
            {
                Projectile.Kill();
            }
            float projTimer = 25f;
            if (Projectile.ai[0] > 180f)
            {
                projTimer -= (Projectile.ai[0] - 180f) / 2f;
            }
            if (projTimer <= 0f)
            {
                projTimer = 0f;
                Projectile.Kill();
            }
            projTimer *= 0.7f;
            Projectile.ai[0] += 4f;
            int timerCounter = 0;
            while ((float)timerCounter < projTimer)
            {
                float rando1 = (float)Main.rand.Next(-10, 11);
                float rando2 = (float)Main.rand.Next(-10, 11);
                float rando3 = (float)Main.rand.Next(3, 9);
                float randoAdjuster = (float)Math.Sqrt((double)(rando1 * rando1 + rando2 * rando2));
                randoAdjuster = rando3 / randoAdjuster;
                rando1 *= randoAdjuster;
                rando2 *= randoAdjuster;
                int brimDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 1.5f);
                Main.dust[brimDust].noGravity = true;
                Main.dust[brimDust].position.X = Projectile.Center.X;
                Main.dust[brimDust].position.Y = Projectile.Center.Y;
                Dust expr_149DF_cp_0 = Main.dust[brimDust];
                expr_149DF_cp_0.position.X += (float)Main.rand.Next(-10, 11);
                Dust expr_14A09_cp_0 = Main.dust[brimDust];
                expr_14A09_cp_0.position.Y += (float)Main.rand.Next(-10, 11);
                Main.dust[brimDust].velocity.X = rando1;
                Main.dust[brimDust].velocity.Y = rando2;
                timerCounter++;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 150);
        }
    }
}
