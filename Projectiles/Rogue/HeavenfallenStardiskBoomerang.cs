using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class HeavenfallenStardiskBoomerang : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/HeavenfallenStardisk";

        private bool explode = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heavenfallen Stardisk");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (Projectile.Calamity().stealthStrike)
            {
                if (Projectile.timeLeft % 5f == 0f) //every 5 ticks
                {
                    var source = Projectile.GetProjectileSource_FromThis();
                    if (Main.rand.NextBool(2))
                    {
                        int energyAmt = Main.rand.Next(1, 4); //1 to 3 energy
                        for (int n = 0; n < energyAmt; n++)
                        {
                            CalamityUtils.ProjectileRain(source, Projectile.Center, 400f, 100f, 500f, 800f, 29f, ModContent.ProjectileType<HeavenfallenEnergy>(), (int)(Projectile.damage * 0.4), Projectile.knockBack * 0.4f, Projectile.owner);
                        }
                    }
                }
            }

            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 20;
            }
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }

            for (int i = 0; i < 2; i++)
            {
                int blueDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, ModContent.DustType<AstralBlue>(), 0f, 0f, 100, default, 1f);
                Main.dust[blueDust].noGravity = true;
                Main.dust[blueDust].velocity *= 0f;
            }
            for (int i = 0; i < 2; i++)
            {
                int orangeDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 1f);
                Main.dust[orangeDust].noGravity = true;
                Main.dust[orangeDust].velocity *= 0f;
            }

            Projectile.rotation += 0.5f;

            if (player.position.Y != player.oldPosition.Y && Projectile.ai[0] == 0f)
            {
                explode = true;
            }

            Projectile.ai[0] += 1f;

            if (Main.myPlayer == Projectile.owner && Projectile.ai[0] == 20f)
            {
                if (player.channel)
                {
                    float num115 = 20f;
                    float num116 = (float)Main.mouseX + Main.screenPosition.X - Projectile.Center.X;
                    float num117 = (float)Main.mouseY + Main.screenPosition.Y - Projectile.Center.Y;
                    if (player.gravDir == -1f)
                    {
                        num117 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - Projectile.Center.Y;
                    }
                    float num118 = (float)Math.Sqrt((double)(num116 * num116 + num117 * num117));
                    if (num118 > num115)
                    {
                        num118 = num115 / num118;
                        num116 *= num118;
                        num117 *= num118;
                        int num119 = (int)(num116 * 1000f);
                        int num120 = (int)(Projectile.velocity.X * 1000f);
                        int num121 = (int)(num117 * 1000f);
                        int num122 = (int)(Projectile.velocity.Y * 1000f);
                        if (num119 != num120 || num121 != num122)
                        {
                            Projectile.netUpdate = true;
                        }
                        Projectile.velocity.X = num116;
                        Projectile.velocity.Y = num117;
                    }
                    else
                    {
                        int num123 = (int)(num116 * 1000f);
                        int num124 = (int)(Projectile.velocity.X * 1000f);
                        int num125 = (int)(num117 * 1000f);
                        int num126 = (int)(Projectile.velocity.Y * 1000f);
                        if (num123 != num124 || num125 != num126)
                        {
                            Projectile.netUpdate = true;
                        }
                        Projectile.velocity.X = num116;
                        Projectile.velocity.Y = num117;
                    }
                }
                else if (Projectile.ai[0] == 20f)
                {
                    Projectile.netUpdate = true;
                    float num127 = 20f;
                    Vector2 vector11 = Projectile.Center;
                    float num128 = (float)Main.mouseX + Main.screenPosition.X - vector11.X;
                    float num129 = (float)Main.mouseY + Main.screenPosition.Y - vector11.Y;
                    if (player.gravDir == -1f)
                    {
                        num129 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector11.Y;
                    }
                    float num130 = (float)Math.Sqrt((double)(num128 * num128 + num129 * num129));
                    if (num130 == 0f || Projectile.ai[0] < 0f)
                    {
                        vector11 = player.Center;
                        num128 = Projectile.Center.X - vector11.X;
                        num129 = Projectile.Center.Y - vector11.Y;
                        num130 = (float)Math.Sqrt((double)(num128 * num128 + num129 * num129));
                    }
                    num130 = num127 / num130;
                    num128 *= num130;
                    num129 *= num130;
                    Projectile.velocity.X = num128;
                    Projectile.velocity.Y = num129;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 240);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 240);
        }

        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];

            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int i = 0; i < 10; i++)
            {
                int num469 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, ModContent.DustType<AstralBlue>(), 0f, 0f, 100, default, 1.5f);
                Main.dust[num469].noGravity = true;
                Main.dust[num469].velocity *= 0f;
            }
            for (int i = 0; i < 10; i++)
            {
                int num469 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 1.5f);
                Main.dust[num469].noGravity = true;
                Main.dust[num469].velocity *= 0f;
            }

            if (explode && player.position.Y != player.oldPosition.Y)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    float spread = 45f * 0.0174f;
                    double startAngle = Math.Atan2(Projectile.velocity.X, Projectile.velocity.Y) - spread / 2;
                    double deltaAngle = spread / 8f;
                    double offsetAngle;
                    for (int i = 0; i < 4; i++)
                    {
                        offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                        Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 2f), (float)(Math.Cos(offsetAngle) * 2f), ModContent.ProjectileType<HeavenfallenEnergy>(), (int)(Projectile.damage * 0.4), Projectile.knockBack * 0.4f, Projectile.owner, 0f, 0f);
                        Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 2f), (float)(-Math.Cos(offsetAngle) * 2f), ModContent.ProjectileType<HeavenfallenEnergy>(), (int)(Projectile.damage * 0.4), Projectile.knockBack * 0.4f, Projectile.owner, 0f, 0f);
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 2);
            return false;
        }
    }
}
