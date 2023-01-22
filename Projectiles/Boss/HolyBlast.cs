using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.NPCs;
using CalamityMod.NPCs.Providence;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Boss
{
    public class HolyBlast : ModProjectile
    {
        public static readonly SoundStyle ShootSound = new("CalamityMod/Sounds/Custom/Providence/ProvidenceHolyBlastShoot");
        public static readonly SoundStyle ImpactSound = new("CalamityMod/Sounds/Custom/Providence/ProvidenceHolyBlastImpact");

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Blast");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.Calamity().DealsDefenseDamage = true;
            Projectile.width = 180;
            Projectile.height = 180;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            //Day mode by default but syncs with the boss
            if (CalamityGlobalNPC.holyBoss != -1)
            {
                if (Main.npc[CalamityGlobalNPC.holyBoss].active)
                    Projectile.maxPenetrate = (int)Main.npc[CalamityGlobalNPC.holyBoss].localAI[1];
            }
            else
                Projectile.maxPenetrate = (int)Providence.BossMode.Day;

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }
            if (Projectile.wet || Projectile.lavaWet)
            {
                Projectile.Kill();
            }
            if (Projectile.ai[1] == 0f)
            {
                int dustType = ProvUtils.GetDustID(Projectile.maxPenetrate);
                for (int num621 = 0; num621 < 10; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustType, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    Main.dust[num622].noGravity = true;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                Projectile.ai[1] = 1f;
                SoundEngine.PlaySound(ShootSound, Projectile.position);
            }
            if (Math.Abs(Projectile.velocity.X) > 0.2)
            {
                Projectile.spriteDirection = -Projectile.direction;
            }
            if (Projectile.velocity.X < 0f)
            {
                Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
            }
            else
            {
                Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return ProvUtils.GetProjectileColor(Projectile.maxPenetrate, Projectile.alpha);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = (Projectile.maxPenetrate != (int)Providence.BossMode.Day) ? ModContent.Request<Texture2D>(Texture).Value : ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/HolyBlastNight").Value;
            int num214 = texture.Height / Main.projFrames[Projectile.type];
            int y6 = num214 * Projectile.frame;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture.Width, num214)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(texture.Width / 2f, num214 / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                int totalProjectiles = (Projectile.maxPenetrate != (int)Providence.BossMode.Day) ? 8 : 6;
                float radians = MathHelper.TwoPi / totalProjectiles;
                int type = ModContent.ProjectileType<HolyFire2>();
                float velocity = 5f;
                Vector2 spinningPoint = new Vector2(0f, -velocity);
                for (int k = 0; k < totalProjectiles; k++)
                {
                    Vector2 velocity2 = spinningPoint.RotatedBy(radians * k);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity2 + Projectile.velocity * 0.25f, type, (int)Math.Round(Projectile.damage * 0.75), 0f, Projectile.owner);
                }
            }

            SoundEngine.PlaySound(ImpactSound, Projectile.Center);

            int dustType = ProvUtils.GetDustID(Projectile.maxPenetrate);
            for (int num193 = 0; num193 < 4; num193++)
            {
                int dust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustType, 0f, 0f, 50, default, 1.5f);
                Main.dust[dust].noGravity = true;
            }
            for (int num194 = 0; num194 < 40; num194++)
            {
                int num195 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustType, 0f, 0f, 0, default, 2.5f);
                Main.dust[num195].noGravity = true;
                Main.dust[num195].velocity *= 3f;
                num195 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustType, 0f, 0f, 50, default, 1.5f);
                Main.dust[num195].velocity *= 2f;
                Main.dust[num195].noGravity = true;
            }
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            //In GFB, "real damage" is replaced with negative healing
            if (Projectile.maxPenetrate >= (int)Providence.BossMode.Red)
                damage = 0;

            //If the player is dodging, don't apply debuffs
            if (damage <= 0 && Projectile.maxPenetrate < (int)Providence.BossMode.Red || target.creativeGodMode)
                return;

            ProvUtils.ApplyHitEffects(target, Projectile.maxPenetrate, 480, 20);
        }
    }
}
