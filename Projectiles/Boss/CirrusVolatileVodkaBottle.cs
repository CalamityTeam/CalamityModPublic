using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Buffs.Alcohol;
using CalamityMod.Dusts;
using CalamityMod.Events;
using System;

namespace CalamityMod.Projectiles.Boss
{
    public class CirrusVolatileVodkaBottle : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";

        public override string Texture => "CalamityMod/Items/Potions/Alcohol/FabsolsVodka";

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.aiStyle = ProjAIStyleID.ThrownProjectile;
            AIType = ProjectileID.ThrowingKnife;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item106, Projectile.Center);
                Projectile.ai[0] = 1f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Shatter, Projectile.Center);

            for (int i = 0; i < 10; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 0, default, 1.2f);
                Main.dust[dust].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[dust].scale = 0.5f;
                    Main.dust[dust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }

            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, (int)CalamityDusts.BlueCosmilite, 0f, 0f, 0, default, 1.7f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 5f;
                dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 0, default, 1f);
                Main.dust[dust].velocity *= 2f;
            }

            if (Projectile.owner == Main.myPlayer)
            {
                int totalProjectiles = 3;
                float radians = MathHelper.TwoPi / totalProjectiles;
                int type = ModContent.ProjectileType<FabRay>();
                float velocity = 8f;
                double angleA = radians * 0.5;
                double angleB = MathHelper.ToRadians(90f) - angleA;
                float velocityX2 = (float)(velocity * Math.Sin(angleA) / Math.Sin(angleB));
                Vector2 spinningPoint = Main.rand.NextBool() ? new Vector2(0f, -velocity) : new Vector2(-velocityX2, -velocity);
                for (int k = 0; k < totalProjectiles; k++)
                {
                    Vector2 velocity2 = spinningPoint.RotatedBy(radians * k);
                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Vector2.Normalize(velocity2) * 10f, velocity2, type, (int)Math.Round(Projectile.damage * 0.8), 0f, Main.myPlayer);
                    if (proj.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[proj].DamageType = DamageClass.Default;
                        Main.projectile[proj].friendly = false;
                        Main.projectile[proj].hostile = true;
                    }
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Projectile.Kill();
            target.AddBuff(ModContent.BuffType<FabsolVodkaBuff>(), 54000);
        }
    }
}
