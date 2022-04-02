using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class PristineSecondary : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private int dust1 = (int)CalamityDusts.ProfanedFire;
        private int dust2 = ModContent.DustType<HolyFireDust>();

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fire");
        }
        
        public override void SetDefaults()
        {
            projectile.width = 50;
            projectile.height = 50;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.ranged = true;
            projectile.penetrate = -1;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 3;
            projectile.timeLeft = 150;
        }

        public override void AI()
        {
            projectile.velocity *= 0.98f;
            int dustType = Utils.SelectRandom(Main.rand, new int[]
            {
                dust1,
                dust2
            });
            Lighting.AddLight(projectile.Center, 1f, 1f, 0.25f);
            if (projectile.ai[0] > 7f)
            {
                float num296 = 1f;
                if (projectile.ai[0] == 8f)
                {
                    num296 = 0.25f;
                }
                else if (projectile.ai[0] == 9f)
                {
                    num296 = 0.5f;
                }
                else if (projectile.ai[0] == 10f)
                {
                    num296 = 0.75f;
                }
                projectile.ai[0] += 1f;
                if (Main.rand.NextBool(2))
                {
                    for (int num298 = 0; num298 < 1; num298++)
                    {
                        int num299 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dustType, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100, default, 1f);
                        Dust dust = Main.dust[num299];
                        if (Main.rand.NextBool(3))
                        {
                            dust.noGravity = true;
                            dust.scale *= 1.25f;
                            dust.velocity.X *= 2f;
                            dust.velocity.Y *= 2f;
                        }
                        if (Main.rand.NextBool(6))
                        {
                            dust.noGravity = true;
                            dust.scale *= 1.5f;
                            dust.velocity.X *= 2f;
                            dust.velocity.Y *= 2f;
                        }
                        else
                        {
                            dust.scale *= 1f;
                        }
                        dust.velocity.X *= 1.2f;
                        dust.velocity.Y *= 1.2f;
                        dust.scale *= num296;
                        dust.noLight = true;
                        dust.color = CalamityUtils.ColorSwap(new Color(255, 168, 53), new Color(255, 249, 0), 2f);
                    }
                }
            }
            else
            {
                projectile.ai[0] += 1f;
            }
            projectile.rotation += 0.3f * (float)projectile.direction;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 240);
        }
    }
}