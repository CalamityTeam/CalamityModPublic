using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
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
        public Player Owner => Main.player[Projectile.owner];

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
            Projectile.timeLeft = 300;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            if (Projectile.Calamity().stealthStrike && Projectile.timeLeft % 5f == 4f) // every 5 frames
                CalamityUtils.ProjectileRain(Projectile.GetSource_FromThis(), Projectile.Center, 400f, 100f, 500f, 800f, 29f, ModContent.ProjectileType<HeavenfallenEnergy>(), Projectile.damage / 2, Projectile.knockBack * 0.5f, Projectile.owner);            

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

            if (Owner.position.Y != Owner.oldPosition.Y && Projectile.ai[0] == 0f)
                Projectile.ai[1]++;            

            Projectile.ai[0]++;

            if (Main.myPlayer == Projectile.owner && Projectile.ai[0] == 20f)
            {
                if (Owner.channel)
                {
                    float num115 = 20f;
                    float num116 = (float)Main.mouseX + Main.screenPosition.X - Projectile.Center.X;
                    float num117 = (float)Main.mouseY + Main.screenPosition.Y - Projectile.Center.Y;
                    if (Owner.gravDir == -1f)
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
                    if (Owner.gravDir == -1f)
                    {
                        num129 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector11.Y;
                    }
                    float num130 = (float)Math.Sqrt((double)(num128 * num128 + num129 * num129));
                    if (num130 == 0f || Projectile.ai[0] < 0f)
                    {
                        vector11 = Owner.Center;
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
            
            if (Projectile.owner == Main.myPlayer && Projectile.ai[1] > 0)
            {
                for (int i = 0; i < 5; i++)
                {
                    Vector2 velocity = ((MathHelper.TwoPi * i / 5f) - MathHelper.PiOver2).ToRotationVector2() * 4f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<HeavenfallenEnergy>(), Projectile.damage / 2, Projectile.knockBack * 0.5f, Projectile.owner, 0f, 1f);
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
