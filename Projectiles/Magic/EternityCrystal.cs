using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class EternityCrystal : ModProjectile
    {
        public float Radius = 480f;
        public float DegreesToSpin = 2f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal");
            Main.projFrames[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 34;
            projectile.height = 36;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = EternityHex.trueTimeLeft;
            projectile.alpha = 0;
            projectile.magic = true;
        }

        public override void AI()
        {
			Player player = Main.player[projectile.owner];
            if (projectile.localAI[1] >= Main.projectile.Length || projectile.localAI[0] < 0)
            {
                DeathDust();
                projectile.Kill();
                return;
            }

            Projectile book = Main.projectile[(int)projectile.localAI[1]];

            if (!book.active)
            {
                DeathDust();
                projectile.Kill();
                return;
            }
            
            if (projectile.ai[0] >= Main.npc.Length || projectile.ai[0] < 0)
            {
                DeathDust();
                projectile.Kill();
                return;
            }

            if (!Main.npc[(int)projectile.ai[0]].active)
            {
                DeathDust();
                projectile.Kill();
                return;
            }

            projectile.localAI[0] += 1f;
            if (projectile.timeLeft == 1)
            {
                projectile.velocity = projectile.DirectionTo(Main.npc[(int)projectile.ai[0]].Center) * 2f;
                projectile.timeLeft = 1000;
            }
            if (projectile.timeLeft < 920 && projectile.timeLeft > EternityHex.trueTimeLeft ||
                CalamityUtils.CountProjectiles(ModContent.ProjectileType<EternityCrystal>()) > 10)
            {
                projectile.Kill();
            }
            else
            {
                projectile.damage = 0;
                projectile.rotation = projectile.AngleTo(Main.npc[(int)projectile.ai[0]].Center) - MathHelper.PiOver2;
                projectile.position = Main.npc[(int)projectile.ai[0]].Center + projectile.ai[1].ToRotationVector2() * Radius;
                projectile.ai[1] -= MathHelper.ToRadians(DegreesToSpin);

                if (projectile.timeLeft > 920)
                {
                    Radius *= 0.95f;
                    if (projectile.alpha < 255)
                    {
                        projectile.alpha += 3;
                    }
                    DegreesToSpin *= 1.035f;
                }
            }
            if (projectile.Hitbox.Intersects(Main.npc[(int)projectile.ai[0]].Hitbox) && !Main.npc[(int)projectile.ai[0]].dontTakeDamage)
            {
                int damage = (int)(Eternity.ExplosionDamage * player.MagicDamage() * Main.rand.NextFloat(0.9f, 1.1f));
                player.addDPS(damage);
                Main.npc[(int)projectile.ai[0]].StrikeNPC(damage, 0f, 0, false);
                Vector2 randomCirclePointVector = Vector2.One.RotatedBy(projectile.rotation);
                float lerpStart = Main.rand.Next(12, 17);
                float lerpEnd = Main.rand.Next(3, 7);
                NPC target = Main.npc[(int)projectile.ai[0]];
                Main.PlaySound(ModLoader.GetMod("CalamityMod").GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LargeWeaponFire"), (int)target.Center.X, (int)target.Center.Y);
                for (float i = 0; i < 9f; ++i)
                {
                    for (int j = 0; j < 2; ++j)
                    {
                        Vector2 randomCirclePointRotated = randomCirclePointVector.RotatedBy((j == 0 ? 1 : -1) * MathHelper.TwoPi / 18f);
                        for (float k = 0f; k < 20f; ++k)
                        {
                            Vector2 randomCirclePointLerped = Vector2.Lerp(randomCirclePointVector, randomCirclePointRotated, k / 20f);
                            float lerpMultiplier = MathHelper.Lerp(lerpStart, lerpEnd, k / 20f) * 4f;
                            int dustIndex = Dust.NewDust(new Vector2(target.Center.X, target.Center.Y), 0, 0,
                                Main.rand.Next(132, 133 + 1),
                                0f, 0f, 100, default, 1.3f);
                            Main.dust[dustIndex].velocity *= 0.1f;
                            Main.dust[dustIndex].noGravity = true;
                            Main.dust[dustIndex].color = Utils.SelectRandom(Main.rand, new Color[]
                                {
                                    new Color(61, 141, 235),
                                    new Color(229, 52, 220),
                                });
                            Main.dust[dustIndex].velocity += randomCirclePointLerped * lerpMultiplier;
                        }
                    }

                    randomCirclePointVector = randomCirclePointVector.RotatedBy(MathHelper.TwoPi / 9f);
                }
                projectile.Kill();
            }
        }
        public void DeathDust()
        {
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustPerfect(projectile.Center, Eternity.dustID, newColor: new Color(245, 112, 218));
                dust.velocity = Utils.NextVector2Unit(Main.rand) * Main.rand.NextFloat(2f, 6f);
                dust.noGravity = true;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            //Variables/parameters to be used in the spriteBatch.Draw method later.
            Color colorAtCenter = Lighting.GetColor((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16);
            Texture2D myTexture = Main.projectileTexture[projectile.type];
            Rectangle frame = myTexture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
            Color alphaFromCenter = projectile.GetAlpha(colorAtCenter);
            Vector2 origin = frame.Size() / 2f;
            //This is for wrapping back so that we don't just blip back to 0 after going above pi/2
            const float maxTheta = 60f;
            const float pixelOutwardMin = 5f;
            const float pixelOutwardMax = 10f - pixelOutwardMin;
            float angleWrapped = projectile.localAI[0];

            //The modulo operator is used so that we never go below 0 or above pi/2 in the given calculations
            float theta = MathHelper.Pi * angleWrapped / maxTheta;

            //Cos(0)/2 + 1/2 is 1, Cos(pi)/2 + 1/2 is 0. So, we can use this cosine to iterate down from 1 to 0 in strength
            //in a wave pattern
            float rotationFactor = ((float)Math.Cos(theta) * 0.5f + 0.5f) * pixelOutwardMax + pixelOutwardMin;

            //Iterate through 2pi in 2 operations. We want to show two of the transparent crystals right next to the main one
            for (float i = 0f; i < 2f; i += 1f)
            {
                double angle = MathHelper.TwoPi / 2f * i + MathHelper.PiOver2;
                Vector2 drawPosition = projectile.Center + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition +
                    Vector2.UnitY.RotatedBy(angle).RotatedBy(projectile.rotation) * rotationFactor;
                Main.spriteBatch.Draw(myTexture, drawPosition, new Rectangle?(frame),
                    alphaFromCenter * 0.6f, projectile.rotation, origin,
                    projectile.scale, SpriteEffects.None, 0f);
            }
            return true;
        }
    }
}