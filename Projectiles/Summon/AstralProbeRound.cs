using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class AstralProbeRound : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Ranged/AstralRound";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blast");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.minionSlots = 0f;
            projectile.minion = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
            projectile.extraUpdates = 2;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0.3f, 0.5f, 0.1f);
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Main.rand.NextBool(3))
            {
                int randomDust = Utils.SelectRandom(Main.rand, new int[]
                {
                    ModContent.DustType<AstralOrange>(),
                    ModContent.DustType<AstralBlue>()
                });
                int astral = Dust.NewDust(projectile.position, 1, 1, randomDust, 0f, 0f, 0, default, 0.5f);
                Main.dust[astral].alpha = projectile.alpha;
                Main.dust[astral].velocity *= 0f;
                Main.dust[astral].noGravity = true;
            }
            NPC potentialTarget = projectile.Center.MinionHoming(1200f, Main.player[projectile.owner]);
            if (potentialTarget != null)
                projectile.velocity = (projectile.velocity * 20f + projectile.SafeDirectionTo(potentialTarget.Center) * 17f) / 21f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 10);
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
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, randomDust, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}
