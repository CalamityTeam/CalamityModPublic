using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class SupremeDustProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override void SetDefaults()
        {
            Projectile.width = 200;
            Projectile.height = 200;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 3;
            Projectile.MaxUpdates = 4;
            Projectile.timeLeft = 50 * Projectile.MaxUpdates; // 50 effective, 200 total
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10 * Projectile.MaxUpdates; // 10 effective, 40 total
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.95f / 255f, (255 - Projectile.alpha) * 0.85f / 255f, (255 - Projectile.alpha) * 0.01f / 255f);
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
                int earthyDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustType, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, default, 1f);
                Dust dust = Main.dust[earthyDust];
                if (Main.rand.NextBool())
                {
                    dust.noGravity = true;
                    dust.scale *= 4f;
                    dust.velocity.X *= 2f;
                    dust.velocity.Y *= 2f;
                }
                else
                {
                    dust.scale *= 2.5f;
                }
                dust.velocity.X *= 1.2f;
                dust.velocity.Y *= 1.2f;
                dust.scale *= dustScale;
                int earthyDust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustType, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, default, 1f);
                Dust dust2 = Main.dust[earthyDust2];
                if (Main.rand.NextBool(3))
                {
                    dust2.noGravity = true;
                    dust2.scale *= 6f;
                    dust2.velocity.X *= 2f;
                    dust2.velocity.Y *= 2f;
                }
                else
                {
                    dust2.scale *= 2.5f;
                }
                dust2.velocity.X *= 1.2f;
                dust2.velocity.Y *= 1.2f;
                dust2.scale *= dustScale;
            }
            else
            {
                Projectile.ai[0] += 1f;
            }
            Projectile.rotation += 0.3f * (float)Projectile.direction;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 180);
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<SupremeDustFlakProjectile>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 180);
    }
}
