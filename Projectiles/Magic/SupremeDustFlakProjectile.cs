using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class SupremeDustFlakProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.MaxUpdates = 4;
            Projectile.timeLeft = 25 * Projectile.MaxUpdates; // 25 effective, 100 total
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.35f / 255f, (255 - Projectile.alpha) * 0.3f / 255f, (255 - Projectile.alpha) * 0.01f / 255f);
            if (Projectile.ai[0] > 7f)
            {
                float dustScale = 1f;
                if (Projectile.ai[0] == 8f)
                {
                    dustScale = 0.25f;
                }
                else if (Projectile.ai[0] == 9f)
                {
                    dustScale = 0.5f;
                }
                else if (Projectile.ai[0] == 10f)
                {
                    dustScale = 0.75f;
                }
                Projectile.ai[0] += 1f;
                int dustType = 32;
                for (int i = 0; i < 2; i++)
                {
                    int earthyDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustType, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, default, 1f);
                    Dust dust = Main.dust[earthyDust];
                    if (Main.rand.NextBool(3))
                    {
                        dust.noGravity = false;
                        dust.scale *= 1f;
                        dust.velocity.X *= 2f;
                        dust.velocity.Y *= 2f;
                    }
                    else
                    {
                        dust.scale *= 0.5f;
                    }
                    dust.velocity.X *= 1.2f;
                    dust.velocity.Y *= 1.2f;
                    dust.scale *= dustScale;
                }
                for (int i = 0; i < 2; i++)
                {
                    int earthyDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustType, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, default, 1f);
                    Dust dust = Main.dust[earthyDust];
                    if (Main.rand.NextBool(3))
                    {
                        dust.noGravity = false;
                        dust.scale *= 1.5f;
                        dust.velocity.X *= 2f;
                        dust.velocity.Y *= 2f;
                    }
                    else
                    {
                        dust.scale *= 0.25f;
                    }
                    dust.velocity.X *= 1.2f;
                    dust.velocity.Y *= 1.2f;
                    dust.scale *= dustScale;
                }
            }
            else
            {
                Projectile.ai[0] += 1f;
            }
            Projectile.rotation += 0.3f * (float)Projectile.direction;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 180);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 180);
    }
}
