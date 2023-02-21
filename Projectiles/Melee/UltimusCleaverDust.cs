using CalamityMod.Buffs.DamageOverTime;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class UltimusCleaverDust : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ultimus Flame");
        }

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 90;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void AI()
        {
            if (Projectile.velocity.X != Projectile.velocity.X)
            {
                Projectile.velocity.X = Projectile.velocity.X * -0.1f;
            }
            if (Projectile.velocity.X != Projectile.velocity.X)
            {
                Projectile.velocity.X = Projectile.velocity.X * -0.5f;
            }
            if (Projectile.velocity.Y != Projectile.velocity.Y && Projectile.velocity.Y > 1f)
            {
                Projectile.velocity.Y = Projectile.velocity.Y * -0.5f;
            }

            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] > 5f)
            {
                Projectile.ai[0] = 5f;
                if (Projectile.velocity.Y == 0f && Projectile.velocity.X != 0f)
                {
                    Projectile.velocity.X = Projectile.velocity.X * 0.97f;
                    if ((double)Projectile.velocity.X > -0.01 && (double)Projectile.velocity.X < 0.01)
                    {
                        Projectile.velocity.X = 0f;
                        Projectile.netUpdate = true;
                    }
                }
                Projectile.velocity.Y = Projectile.velocity.Y + 0.2f;
            }

            Projectile.rotation += Projectile.velocity.X * 0.1f;

            int num199 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 135, 0f, 0f, 100, default, 1f);
            Dust expr_8976_cp_0 = Main.dust[num199];
            expr_8976_cp_0.position.X -= 2f;
            Dust expr_8994_cp_0 = Main.dust[num199];
            expr_8994_cp_0.position.Y += 2f;
            Main.dust[num199].scale += (float)Main.rand.Next(50) * 0.01f;
            Main.dust[num199].noGravity = true;
            Dust expr_89E7_cp_0 = Main.dust[num199];
            expr_89E7_cp_0.velocity.Y -= 2f;

            if (Main.rand.NextBool(2))
            {
                int num200 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 135, 0f, 0f, 100, default, 1f);
                Dust expr_8A4E_cp_0 = Main.dust[num200];
                expr_8A4E_cp_0.position.X -= 2f;
                Dust expr_8A6C_cp_0 = Main.dust[num200];
                expr_8A6C_cp_0.position.Y += 2f;
                Main.dust[num200].scale += 0.3f + (float)Main.rand.Next(50) * 0.01f;
                Main.dust[num200].noGravity = true;
                Main.dust[num200].velocity *= 0.1f;
            }

            if ((double)Projectile.velocity.Y < 0.25 && (double)Projectile.velocity.Y > 0.15)
            {
                Projectile.velocity.X = Projectile.velocity.X * 0.8f;
            }

            Projectile.rotation = -Projectile.velocity.X * 0.05f;

            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }

            CalamityUtils.HomeInOnNPC(Projectile, true, 150f, 12f, 20f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Projectile.DamageType == RogueDamageClass.Instance)
                target.AddBuff(BuffID.Electrified, 120);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            if (Projectile.DamageType == RogueDamageClass.Instance)
                target.AddBuff(BuffID.Electrified, 120);
        }
    }
}
