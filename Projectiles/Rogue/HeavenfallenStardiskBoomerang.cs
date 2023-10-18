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
    public class HeavenfallenStardiskBoomerang : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/HeavenfallenStardisk";
        public Player Owner => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
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
                    float constant = 20f;
                    float xfactor = (float)Main.mouseX + Main.screenPosition.X - Projectile.Center.X;
                    float yfactor = (float)Main.mouseY + Main.screenPosition.Y - Projectile.Center.Y;
                    if (Owner.gravDir == -1f)
                    {
                        yfactor = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - Projectile.Center.Y;
                    }
                    float factorAdjust = (float)Math.Sqrt((double)(xfactor * xfactor + yfactor * yfactor));
                    if (factorAdjust > constant)
                    {
                        factorAdjust = constant / factorAdjust;
                        xfactor *= factorAdjust;
                        yfactor *= factorAdjust;
                        int scaledX = (int)(xfactor * 1000f);
                        int scaledXVel = (int)(Projectile.velocity.X * 1000f);
                        int scaledY = (int)(yfactor * 1000f);
                        int scaledYVel = (int)(Projectile.velocity.Y * 1000f);
                        if (scaledX != scaledXVel || scaledY != scaledYVel)
                        {
                            Projectile.netUpdate = true;
                        }
                        Projectile.velocity.X = xfactor;
                        Projectile.velocity.Y = yfactor;
                    }
                    else
                    {
                        int scaledX2 = (int)(xfactor * 1000f);
                        int scaledXVel2 = (int)(Projectile.velocity.X * 1000f);
                        int scaledY2 = (int)(yfactor * 1000f);
                        int scaledYVel2 = (int)(Projectile.velocity.Y * 1000f);
                        if (scaledX2 != scaledXVel2 || scaledY2 != scaledYVel2)
                        {
                            Projectile.netUpdate = true;
                        }
                        Projectile.velocity.X = xfactor;
                        Projectile.velocity.Y = yfactor;
                    }
                }
                else if (Projectile.ai[0] == 20f)
                {
                    Projectile.netUpdate = true;
                    float sameConstant = 20f;
                    Vector2 centerPoint = Projectile.Center;
                    float xfactor2 = (float)Main.mouseX + Main.screenPosition.X - centerPoint.X;
                    float yfactor2 = (float)Main.mouseY + Main.screenPosition.Y - centerPoint.Y;
                    if (Owner.gravDir == -1f)
                    {
                        yfactor2 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - centerPoint.Y;
                    }
                    float factorAdjust2 = (float)Math.Sqrt((double)(xfactor2 * xfactor2 + yfactor2 * yfactor2));
                    if (factorAdjust2 == 0f || Projectile.ai[0] < 0f)
                    {
                        centerPoint = Owner.Center;
                        xfactor2 = Projectile.Center.X - centerPoint.X;
                        yfactor2 = Projectile.Center.Y - centerPoint.Y;
                        factorAdjust2 = (float)Math.Sqrt((double)(xfactor2 * xfactor2 + yfactor2 * yfactor2));
                    }
                    factorAdjust2 = sameConstant / factorAdjust2;
                    xfactor2 *= factorAdjust2;
                    yfactor2 *= factorAdjust2;
                    Projectile.velocity.X = xfactor2;
                    Projectile.velocity.Y = yfactor2;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 240);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 240);

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int i = 0; i < 10; i++)
            {
                int dusty = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, ModContent.DustType<AstralBlue>(), 0f, 0f, 100, default, 1.5f);
                Main.dust[dusty].noGravity = true;
                Main.dust[dusty].velocity *= 0f;
            }
            for (int i = 0; i < 10; i++)
            {
                int dusty = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 1.5f);
                Main.dust[dusty].noGravity = true;
                Main.dust[dusty].velocity *= 0f;
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
