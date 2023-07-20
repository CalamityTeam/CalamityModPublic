using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class HomingGasBulbSporeGas : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
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
            if (Projectile.ai[1] > (CalamityWorld.LegendaryMode ? 600f : 900f))
            {
                Projectile.localAI[0] += 10f;
                Projectile.damage = 0;
            }

            if (Projectile.localAI[0] > 255f)
            {
                Projectile.Kill();
                Projectile.localAI[0] = 255f;
            }

            float lightValues = (255 - Projectile.alpha) * 0.3f / 255f;
            Lighting.AddLight(Projectile.Center, lightValues, 0f, lightValues);

            Projectile.alpha = (int)(100.0 + Projectile.localAI[0] * 0.7);
            Projectile.rotation += Projectile.velocity.X * 0.02f;
            Projectile.rotation += Projectile.direction * 0.002f;

            if (Projectile.velocity.Length() > (CalamityWorld.LegendaryMode ? 2f : 0.5f))
                Projectile.velocity *= 0.985f;
        }

        public override bool CanHitPlayer(Player target) => Projectile.ai[1] <= (CalamityWorld.LegendaryMode ? 600f : 900f) && Projectile.ai[1] > 120f;

        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.ai[1] > (CalamityWorld.LegendaryMode ? 600f : 900f))
            {
                byte b2 = (byte)((26f - (Projectile.ai[1] - (CalamityWorld.LegendaryMode ? 600f : 900f))) * 10f);
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
                    texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/HomingGasBulbSporeGas2").Value;
                    break;
                case 2:
                    texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/HomingGasBulbSporeGas3").Value;
                    break;
                default:
                    break;
            }
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1, texture);
            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            if (Projectile.ai[1] <= (CalamityWorld.LegendaryMode ? 600f : 900f) && Projectile.ai[1] > 120f)
                target.AddBuff(BuffID.Poisoned, 240);
        }
    }
}
