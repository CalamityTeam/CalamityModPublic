using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class BrimlanceStandingFire : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 240;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
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
            if (Projectile.ai[1] == 0f && Projectile.type >= 326 && Projectile.type <= 328)
            {
                Projectile.ai[1] = 1f;
                SoundEngine.PlaySound(SoundID.Item13, Projectile.position);
            }
            int brimDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 1f);
            Dust expr_8976_cp_0 = Main.dust[brimDust];
            expr_8976_cp_0.position.X -= 2f;
            Dust expr_8994_cp_0 = Main.dust[brimDust];
            expr_8994_cp_0.position.Y += 2f;
            Main.dust[brimDust].scale += (float)Main.rand.Next(50) * 0.01f;
            Main.dust[brimDust].noGravity = true;
            Dust expr_89E7_cp_0 = Main.dust[brimDust];
            expr_89E7_cp_0.velocity.Y -= 2f;
            if (Main.rand.NextBool())
            {
                int brimDust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 1f);
                Dust expr_8A4E_cp_0 = Main.dust[brimDust2];
                expr_8A4E_cp_0.position.X -= 2f;
                Dust expr_8A6C_cp_0 = Main.dust[brimDust2];
                expr_8A6C_cp_0.position.Y += 2f;
                Main.dust[brimDust2].scale += 0.3f + (float)Main.rand.Next(50) * 0.01f;
                Main.dust[brimDust2].noGravity = true;
                Main.dust[brimDust2].velocity *= 0.1f;
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
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 240);
    }
}
