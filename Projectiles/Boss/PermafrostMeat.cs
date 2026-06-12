using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class PermafrostMeat : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";

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
                SoundEngine.PlaySound(SoundID.NPCDeath43 with { Volume = SoundID.NPCDeath43.Volume * 0.35f, Pitch = 0.3f }, Projectile.position);
                Projectile.ai[0] = 1f;
            }
            if (Projectile.ai[1] == 0)
            {
                Projectile.aiStyle = -1;
                Projectile.tileCollide = false;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath43 with { Volume = SoundID.NPCDeath43.Volume * 0.35f }, Projectile.position);

            for (int i = 0; i < 10; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.IceGolem, 0f, 0f, 0, default, 1.2f);
                Main.dust[dust].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[dust].scale = 0.5f;
                    Main.dust[dust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }

            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.IceGolem, 0f, 0f, 0, default, 1.7f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 5f;
                dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.IceGolem, 0f, 0f, 0, default, 1f);
                Main.dust[dust].velocity *= 2f;
            }

            if (Projectile.ai[2] == 0)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    int totalProjectiles = 3;
                    float radians = MathHelper.TwoPi / totalProjectiles;
                    float velocity = 8f;
                    double angleA = radians * 0.5;
                    double angleB = MathHelper.ToRadians(90f) - angleA;
                    float velocityX2 = (float)(velocity * Math.Sin(angleA) / Math.Sin(angleB));
                    Vector2 spinningPoint = Main.rand.NextBool() ? new Vector2(0f, -velocity) : new Vector2(-velocityX2, -velocity);
                    for (int k = 0; k < totalProjectiles; k++)
                    {
                        Vector2 velocity2 = spinningPoint.RotatedBy(radians * k);
                        int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Vector2.Normalize(velocity2) * 10f, velocity2, Type, (int)Math.Round(Projectile.damage * 0.8), 0f, Main.myPlayer, ai2: 1);
                        if (proj.WithinBounds(Main.maxProjectiles))
                        {
                            Main.projectile[proj].tileCollide = false;
                            Main.projectile[proj].aiStyle = -1;
                        }
                    }
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Projectile.Kill();
        }
    }
}
