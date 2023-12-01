using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Fishing.BrimstoneCragCatches;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class DrizzlefishFire : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        private int splitTimer = 45;
        public int Time = 0;

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 90;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
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

        public override void AI()
        {
            Time++;
            Player Owner = Main.player[Projectile.owner];
            if (Main.zenithWorld && Time == 1 && Owner.Calamity().dragoonDrizzlefishGelBoost > 1)
                Projectile.damage = (int)(Projectile.damage * Owner.Calamity().dragoonDrizzlefishGelBoost);
            Projectile.scale = 1.5f;
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
                for (int i = 0; i < 5; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(9, 9) - Projectile.velocity * 1.5f, dustType, -Projectile.velocity);
                    dust.noGravity = true;
                    dust.velocity *= 0f;
                    dust.scale = Owner.Calamity().dragoonDrizzlefishGelBoost > 1 ? Main.rand.NextFloat(0.7f + Owner.Calamity().dragoonDrizzlefishGelBoost * 0.5f, 1.4f + Owner.Calamity().dragoonDrizzlefishGelBoost * 0.5f) : Main.rand.NextFloat(1.2f, 1.9f);
                }
            }
            else
                Projectile.alpha = 255;
            if (Time == 4)
            {
                for (int i = 0; i <= 16; i++)
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
                    dust.scale = Main.rand.NextFloat(1.8f, 2.3f);
                    dust.velocity = Projectile.velocity.RotatedByRandom(1.1f) * Main.rand.NextFloat(0.6f, 1.9f);
                    dust.noGravity = true;
                }
            }
            splitTimer--;
            if (splitTimer <= 0)
            {
                int numProj = 2;
                float rotation = MathHelper.ToRadians(Main.rand.Next(15,26));
                if (Projectile.owner == Main.myPlayer)
                {
                    if (Projectile.ai[1] == 1f)
                    {
                        for (int i = 0; i < numProj + 1; i++)
                        {
                            Vector2 perturbedSpeed = new Vector2(Projectile.velocity.X, Projectile.velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<DrizzlefishFireSplit>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 1f);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < numProj + 1; i++)
                        {
                            Vector2 perturbedSpeed = new Vector2(Projectile.velocity.X, Projectile.velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<DrizzlefishFireSplit>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
                        }
                    }
                }
                Projectile.Kill();
            }
            Lighting.AddLight(Projectile.Center, 0.25f, 0f, 0f);
            if (Projectile.timeLeft > 90)
            {
                Projectile.timeLeft = 90;
            }
            Projectile.rotation += 0.5f * (float)Projectile.direction;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[1] == 1f)
            {
                target.AddBuff(BuffID.OnFire3, 120);
            }
            else
            {
                target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 60);
            }
        }
    }
}
