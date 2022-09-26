using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class SporeGasPlantera : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spore Gas");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.Calamity().DealsDefenseDamage = true;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
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
            Projectile.ai[1] += 1f;
            if (Projectile.ai[1] > 1800f)
            {
                Projectile.localAI[0] += 10f;
                Projectile.damage = 0;
            }

            if (Projectile.localAI[0] > 255f)
            {
                Projectile.Kill();
                Projectile.localAI[0] = 255f;
            }

            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.16f / 255f, (255 - Projectile.alpha) * 0.2f / 255f, (255 - Projectile.alpha) * 0.04f / 255f);

            Projectile.alpha = (int)(100.0 + Projectile.localAI[0] * 0.7);
            Projectile.rotation += Projectile.velocity.X * 0.02f;
            Projectile.rotation += Projectile.direction * 0.002f;

            if (Projectile.velocity.Length() > 0.5f)
                Projectile.velocity *= 0.98f;
        }

        public override bool CanHitPlayer(Player target) => Projectile.ai[1] <= 1800f && Projectile.ai[1] > 120f;

        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.ai[1] > 1800f)
            {
                byte b2 = (byte)((26f - (Projectile.ai[1] - 1800f)) * 10f);
                byte a2 = (byte)(Projectile.alpha * (b2 / 255f));
                return new Color(b2, b2, b2, a2);
            }
            return new Color(255, 255, 255, Projectile.alpha);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Changes the texture of the projectile
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            switch ((int)Projectile.ai[0])
            {
                case 0:
                    break;
                case 1:
                    texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/SporeGasPlantera2").Value;
                    break;
                case 2:
                    texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/SporeGasPlantera3").Value;
                    break;
                default:
                    break;
            }
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1, texture);
            return false;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (damage <= 0)
                return;

            if (Projectile.ai[1] <= 1800f && Projectile.ai[1] > 120f)
                target.AddBuff(BuffID.Poisoned, 480);
        }
    }
}
