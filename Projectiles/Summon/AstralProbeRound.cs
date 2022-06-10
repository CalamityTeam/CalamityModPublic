using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using CalamityMod.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Summon
{
    public class AstralProbeRound : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Ranged/AstralRound";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blast");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.minionSlots = 0f;
            Projectile.minion = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 2;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.3f, 0.5f, 0.1f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Main.rand.NextBool(3))
            {
                int randomDust = Utils.SelectRandom(Main.rand, new int[]
                {
                    ModContent.DustType<AstralOrange>(),
                    ModContent.DustType<AstralBlue>()
                });
                int astral = Dust.NewDust(Projectile.position, 1, 1, randomDust, 0f, 0f, 0, default, 0.5f);
                Main.dust[astral].alpha = Projectile.alpha;
                Main.dust[astral].velocity *= 0f;
                Main.dust[astral].noGravity = true;
            }
            NPC potentialTarget = Projectile.Center.MinionHoming(1200f, Main.player[Projectile.owner]);
            if (potentialTarget != null)
                Projectile.velocity = (Projectile.velocity * 20f + Projectile.SafeDirectionTo(potentialTarget.Center) * 17f) / 21f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int k = 0; k < 5; k++)
            {
                int randomDust = Main.rand.Next(2);
                if (randomDust == 0)
                {
                    randomDust = ModContent.DustType<AstralOrange>();
                }
                else
                {
                    randomDust = ModContent.DustType<AstralBlue>();
                }
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, randomDust, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}
