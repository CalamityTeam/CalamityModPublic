using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class SoulScytheExplosion : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 5;
        }

        public override void AI()
        {
            if (Projectile.timeLeft == 5)
            {
                for (int i = 0; i < 30; i++)
                {
                    int soulDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 89, 0f, 0f, 100, default, 2f);
                    Main.dust[soulDust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[soulDust].scale = 0.5f;
                        Main.dust[soulDust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int j = 0; j < 50; j++)
                {
                    int soulDust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 89, 0f, 0f, 100, default, 3f);
                    Main.dust[soulDust2].noGravity = true;
                    Main.dust[soulDust2].velocity *= 5f;
                    soulDust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 89, 0f, 0f, 100, default, 2f);
                    Main.dust[soulDust2].velocity *= 2f;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 180);
            target.AddBuff(BuffID.CursedInferno, 90);
        }
    }
}
