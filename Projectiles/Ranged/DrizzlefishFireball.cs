using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class DrizzlefishFireball : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/Ranged/DrizzlefishFire";
        public int Time = 0;
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 5;
            Projectile.aiStyle = ProjAIStyleID.GroundProjectile;
            Projectile.timeLeft = 300;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void AI()
        {
            Time++;
            Player Owner = Main.player[Projectile.owner];
            if (Main.zenithWorld && Time == 1 && Owner.Calamity().dragoonDrizzlefishGelBoost > 1)
                Projectile.damage = (int)(Projectile.damage * Owner.Calamity().dragoonDrizzlefishGelBoost);
            Projectile.velocity.X *= 0.995f;
            Projectile.velocity.Y -= 0.065f;
            Lighting.AddLight(Projectile.Center, 0.25f, 0f, 0f);
            int dustType = 235;
            int dustType2 = 235;
            if (Projectile.ai[1] == 1f)
            {
                if (Main.rand.NextBool())
                {
                    dustType = 174;
                }
                else
                {
                    dustType = 162;
                }
            }
            else
            {
                if (Main.rand.NextBool())
                {
                    dustType = 183;
                }
                else
                {
                    dustType = 90;
                }
            }
            if (Time > 7)
            {
                Projectile.alpha = 0;
                for (int i = 0; i < 2; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5, 5) - Projectile.velocity * 1.5f, dustType, -Projectile.velocity);
                    dust.noGravity = true;
                    dust.velocity *= 0f;
                    dust.scale = Owner.Calamity().dragoonDrizzlefishGelBoost > 1 ? Main.rand.NextFloat(0.4f + Owner.Calamity().dragoonDrizzlefishGelBoost * 0.5f, 1f + Owner.Calamity().dragoonDrizzlefishGelBoost * 0.5f) : Main.rand.NextFloat(0.9f, 1.5f);
                }
            }
            else
                Projectile.alpha = 255;
            if (Time == 4)
            {
                for (int i = 0; i <= 8; i++)
                {
                    if (Projectile.ai[1] == 1f)
                    {
                        if (Main.rand.NextBool())
                        {
                            dustType2 = 174;
                        }
                        else
                        {
                            dustType2 = 162;
                        }
                    }
                    else
                    {
                        if (Main.rand.NextBool())
                        {
                            dustType2 = 183;
                        }
                        else
                        {
                            dustType2 = 90;
                        }
                    }
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, dustType2, Projectile.velocity);
                    dust.scale = Main.rand.NextFloat(1.1f, 1.9f);
                    dust.velocity = Projectile.velocity.RotatedByRandom(0.8f) * Main.rand.NextFloat(0.3f, 1.3f);
                    dust.noGravity = true;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity *= 0.98f;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Time < 7)
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1, ModContent.Request<Texture2D>("CalamityMod/Projectiles/InvisibleProj").Value);
            else
            {
                if (Projectile.ai[1] == 1f)
                    CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1, ModContent.Request<Texture2D>("CalamityMod/Projectiles/Ranged/DrizzlefishFire2").Value);
                else
                    CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1, ModContent.Request<Texture2D>("CalamityMod/Projectiles/Ranged/DrizzlefishFire").Value);
            }
            //Changes the texture of the projectile
            if (Projectile.ai[1] == 1f)
            {
                Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Ranged/DrizzlefishFire2").Value;
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(new Rectangle(0, 0, 16, 16)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(texture.Width / 2f, 20 / 2f), Projectile.scale, SpriteEffects.None, 0);
                return false;
            }
            return true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[1] == 1f)
            {
                target.AddBuff(BuffID.OnFire3, 40);
            }
            else
            {
                target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 20);
            }
        }
    }
}
