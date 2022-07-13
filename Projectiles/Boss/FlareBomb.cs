using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Boss
{
    public class FlareBomb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flare Bomb");
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.Calamity().DealsDefenseDamage = true;
            Projectile.width = 64;
            Projectile.height = 66;
            Projectile.hostile = true;
            Projectile.scale = 1.5f;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.alpha = 50;
            Projectile.timeLeft = 180;
            CooldownSlot = 1;
        }

        public override void AI()
        {
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
                Projectile.frame = 0;

            Lighting.AddLight(Projectile.Center, 0.5f, 0.25f, 0f);

            if (Projectile.timeLeft > 30 && Projectile.alpha > 0)
                Projectile.alpha -= 25;
            if (Projectile.timeLeft > 30 && Projectile.alpha < 128 && Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                Projectile.alpha = 128;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;

            if (Projectile.ai[0] == -1f || (Projectile.timeLeft > 135 && Projectile.ai[1] == 1f))
                return;

            float inertia = revenge ? 70f : 77f;
            float num954 = 40f;
            float scaleFactor12 = revenge ? 35f : 28f;
            int num959 = (int)Projectile.ai[0];
            if (num959 >= 0 && Main.player[num959].active && !Main.player[num959].dead)
            {
                if (Projectile.Distance(Main.player[num959].Center) > num954)
                {
                    Vector2 moveDirection = Projectile.SafeDirectionTo(Main.player[num959].Center, Vector2.UnitY);
                    Projectile.velocity = (Projectile.velocity * (inertia - 1f) + moveDirection * scaleFactor12) / inertia;
                }
            }
            else
            {
                if (Projectile.ai[0] != -1f)
                {
                    Projectile.ai[0] = -1f;
                    Projectile.netUpdate = true;
                }
            }

            if (Projectile.timeLeft < 60)
                return;

            float num1247 = 0.5f;
            for (int num1248 = 0; num1248 < Main.maxProjectiles; num1248++)
            {
                if (Main.projectile[num1248].active)
                {
                    if (num1248 != Projectile.whoAmI && Main.projectile[num1248].type == Projectile.type)
                    {
                        if (Vector2.Distance(Projectile.Center, Main.projectile[num1248].Center) < 24f)
                        {
                            if (Projectile.position.X < Main.projectile[num1248].position.X)
                                Projectile.velocity.X -= num1247;
                            else
                                Projectile.velocity.X += num1247;

                            if (Projectile.position.Y < Main.projectile[num1248].position.Y)
                                Projectile.velocity.Y -= num1247;
                            else
                                Projectile.velocity.Y += num1247;
                        }
                    }
                }
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, Projectile.alpha);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            int num214 = texture.Height / Main.projFrames[Projectile.type];
            int y6 = num214 * Projectile.frame;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle(0, y6, texture.Width, num214), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(texture.Width / 2f, num214 / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            Projectile.ExpandHitboxBy(48);
            for (int d = 0; d < 2; d++)
            {
                int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 1f);
                Main.dust[idx].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[idx].scale = 0.5f;
                    Main.dust[idx].fadeIn = 1f + Main.rand.NextFloat(0.1f, 1f);
                }
            }
            for (int d = 0; d < 4; d++)
            {
                int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 2f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 5f;
                idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 1f);
                Main.dust[idx].velocity *= 2f;
            }

            if (Main.netMode != NetmodeID.Server)
            {
                Vector2 goreSource = Projectile.Center;
                int goreAmt = 3;
                Vector2 source = new Vector2(goreSource.X - 24f, goreSource.Y - 24f);
                for (int goreIndex = 0; goreIndex < goreAmt; goreIndex++)
                {
                    float velocityMult = 0.33f;
                    if (goreIndex < (goreAmt / 3))
                    {
                        velocityMult = 0.66f;
                    }
                    if (goreIndex >= (2 * goreAmt / 3))
                    {
                        velocityMult = 1f;
                    }
                    Mod mod = ModContent.GetInstance<CalamityMod>();
                    int type = Main.rand.Next(61, 64);
                    int smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                    Gore gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X += 1f;
                    gore.velocity.Y += 1f;
                    type = Main.rand.Next(61, 64);
                    smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                    gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X -= 1f;
                    gore.velocity.Y += 1f;
                    type = Main.rand.Next(61, 64);
                    smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                    gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X += 1f;
                    gore.velocity.Y -= 1f;
                    type = Main.rand.Next(61, 64);
                    smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                    gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X -= 1f;
                    gore.velocity.Y -= 1f;
                }
            }

            Projectile.Damage();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, 16f * Projectile.scale, targetHitbox);

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Dragonfire>(), 240);
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            target.Calamity().lastProjectileHit = Projectile;
        }
    }
}
