using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using System;
using CalamityMod.NPCs.TownNPCs;

namespace CalamityMod.Projectiles.Boss
{
    public class HomingGasBulb : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                SoundEngine.PlaySound(SoundID.Item17, Projectile.Center);
            }

            Lighting.AddLight(Projectile.Center, 0.4f, 0f, 0.4f);

            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathHelper.PiOver2;

            int closestPlayer = (int)Player.FindClosest(Projectile.Center, 1, 1);
            Vector2 velocity = Main.player[closestPlayer].Center - Projectile.Center;
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] >= 30f)
            {
                if (Projectile.ai[0] < 150f)
                {
                    float scaleFactor2 = Projectile.velocity.Length();
                    velocity.Normalize();
                    velocity *= scaleFactor2;
                    Projectile.velocity = (Projectile.velocity * 24f + velocity) / 25f;
                    Projectile.velocity.Normalize();
                    Projectile.velocity *= scaleFactor2;
                }
                else if (Projectile.velocity.Length() < 18f)
                {
                    Projectile.tileCollide = true;
                    Projectile.velocity *= 1.02f;
                }
            }

            if (Projectile.ai[0] % 20f == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
                if (Projectile.owner == Main.myPlayer)
                {
                    int type = ModContent.ProjectileType<HomingGasBulbSporeGas>();
                    float ai0 = Main.rand.Next(3);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Normalize(Projectile.velocity) * 0.2f, type, (int)Math.Round(Projectile.damage * 0.8), 0f, Main.myPlayer, ai0);
                }
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, Projectile.alpha);

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.Center);
            for (int i = 0; i < 15; i++)
            {
                if (Main.rand.Next(3) != 0)
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 166);
                else
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 167);
            }
        }
    }
}
