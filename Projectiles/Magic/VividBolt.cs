using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class VividBolt : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.extraUpdates = 100;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.timeLeft = 30;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
        }

        public override void AI()
        {
            Vector2 rotateVector = new Vector2(5f, 10f);
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] == 48f)
            {
                Projectile.ai[0] = 0f;
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 dustRotation = Vector2.UnitX * -12f;
                    dustRotation = -Vector2.UnitY.RotatedBy((double)(Projectile.ai[0] * 0.1308997f + (float)i * 3.14159274f), default) * rotateVector - Projectile.rotation.ToRotationVector2() * 10f;
                    int exo = Dust.NewDust(Projectile.Center, 0, 0, 66, 0f, 0f, 160, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1f);
                    Main.dust[exo].scale = 0.33f;
                    Main.dust[exo].noGravity = true;
                    Main.dust[exo].position = Projectile.Center + dustRotation;
                    Main.dust[exo].velocity = Projectile.velocity;
                }
            }
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 4f)
            {
                for (int j = 0; j < 2; j++)
                {
                    Vector2 projPos = Projectile.position;
                    projPos -= Projectile.velocity * ((float)j * 0.25f);
                    int exod = Dust.NewDust(projPos, 1, 1, 66, 0f, 0f, 0, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1f);
                    Main.dust[exod].noGravity = true;
                    Main.dust[exod].position = projPos;
                    Main.dust[exod].scale = (float)Main.rand.Next(70, 110) * 0.01f;
                    Main.dust[exod].velocity *= 0.1f;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<MiracleBlight>(), 180);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<MiracleBlight>(), 180);
        }
    }
}
