using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class EndoFire : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public bool speedXChoice = false;
        public bool speedYChoice = false;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 10;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 5;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.coldDamage = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            float speedX = 1f;
            float speedY = 1f;
            if (!speedXChoice)
            {
                speedX = Main.rand.NextBool() ? 1.03f : 0.97f;
                speedXChoice = true;
            }
            if (!speedYChoice)
            {
                speedY = Main.rand.NextBool() ? 1.03f : 0.97f;
                speedYChoice = true;
            }
            Projectile.velocity.X *= speedX;
            Projectile.velocity.X *= speedY;
            if (Projectile.ai[0] > 20f)
            {
                float dustScale = 1f;
                if (Projectile.ai[0] == 21f)
                {
                    dustScale = 0.25f;
                }
                else if (Projectile.ai[0] == 22f)
                {
                    dustScale = 0.5f;
                }
                else if (Projectile.ai[0] == 23f)
                {
                    dustScale = 0.75f;
                }
                Projectile.ai[0] += 1f;
                int dustType = Main.rand.NextBool() ? 68 : 67;
                if (Main.rand.NextBool(4))
                {
                    dustType = 80;
                }
                if (Main.rand.NextBool())
                {

                    int endoDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustType, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, default, 0.8f);
                    Dust dust = Main.dust[endoDust];
                    if (Main.rand.NextBool(3))
                    {
                        dust.scale *= 1.5f;
                        dust.velocity.X *= 1.2f;
                        dust.velocity.Y *= 1.2f;
                    }
                    else
                    {
                        dust.scale *= 0.85f;
                    }
                    dust.noGravity = true;
                    dust.velocity.X *= 0.8f;
                    dust.velocity.Y *= 0.8f;
                    dust.scale *= dustScale;
                    dust.velocity += Projectile.velocity;
                    dust.noLight = true;

                }
            }
            else
            {
                Projectile.ai[0] += 1f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<GlacialState>(), 60);
            target.AddBuff(BuffID.Frostburn, 180);
        }
    }
}
