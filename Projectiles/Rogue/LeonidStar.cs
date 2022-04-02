using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class LeonidStar : ModProjectile
    {
        private bool hasHit = false;
        private bool initialized = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Leonid Star");
        }

        public override void SetDefaults()
        {
            projectile.width = 22;
            projectile.height = 24;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.Calamity().rogue = true;
            projectile.tileCollide = false;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -2;
        }

        public override void AI()
        {
            if (!initialized)
            {
                projectile.rotation += Main.rand.NextFloat();
                initialized = true;
            }
            projectile.ai[0]++;
            if (projectile.ai[0] > 120f)
            {
                projectile.scale *= 0.98f;
                CalamityGlobalProjectile.ExpandHitboxBy(projectile, projectile.scale);
                if (projectile.scale <= 0.05f)
                    projectile.Kill();
            }
            projectile.rotation += projectile.direction * 0.05f;
            if (projectile.soundDelay == 0)
            {
                projectile.soundDelay = 20 + Main.rand.Next(40);
                if (Main.rand.NextBool(9))
                {
                    Main.PlaySound(SoundID.Item9, projectile.position);
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120);
            hasHit = true;
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120);
            hasHit = true;
        }

        public override bool CanDamage() => !hasHit;

        public override Color? GetAlpha(Color lightColor) => CalamityUtils.ColorSwap(LeonidProgenitor.blueColor, LeonidProgenitor.purpleColor, 2.5f);

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                int dustType = Utils.SelectRandom(Main.rand, new int[]
                {
                    ModContent.DustType<AstralOrange>(),
                    ModContent.DustType<AstralBlue>()
                });

                int dust = Dust.NewDust(projectile.Center, 1, 1, dustType, projectile.velocity.X, projectile.velocity.Y, 0, CalamityUtils.ColorSwap(LeonidProgenitor.blueColor, LeonidProgenitor.purpleColor, 1f), 1.5f);
                Main.dust[dust].noGravity = true;
            }
            for (int num480 = 0; num480 < 3; num480++)
            {
                Vector2 velocity = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                velocity.SafeNormalize(default);
                velocity *= Main.rand.Next(1, 6) * 0.01f;
                Gore.NewGore(projectile.position, velocity, Main.rand.Next(16, 18), 1f);
            }
        }
    }
}
