using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.NPCs;
using CalamityMod.NPCs.Providence;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Boss
{
    public class HolyBomb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Bomb");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 80;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 250;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.45f, 0.35f, 0f);

            /*
             * Day mode by default but syncs with the boss
             * Uses maxPenetrate because it's kinda pointless for a boss projectile
             * and it's convenient to copypaste without having to do adjustments
             * If someone really feels like making a proper int for this purpose they can do it themselves - Iris
            */
            if (CalamityGlobalNPC.holyBoss != -1)
            {
                if (Main.npc[CalamityGlobalNPC.holyBoss].active)
                    Projectile.maxPenetrate = (int)Main.npc[CalamityGlobalNPC.holyBoss].localAI[1];
            }
            else
                Projectile.maxPenetrate = (int)Providence.BossMode.Day;

            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] % 120f == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);

                float velocityY = -2f;
                int dustType = ProvUtils.GetDustID(Projectile.maxPenetrate);
                for (int num193 = 0; num193 < 2; num193++)
                {
                    int dust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustType, 0f, velocityY, 50, default, 1.5f);
                    Main.dust[dust].noGravity = true;
                }
                for (int num194 = 0; num194 < 20; num194++)
                {
                    int num195 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustType, 0f, velocityY, 0, default, 2.5f);
                    Main.dust[num195].noGravity = true;
                    Main.dust[num195].velocity *= 2f;
                    num195 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustType, 0f, velocityY, 50, default, 1.5f);
                    Main.dust[num195].velocity *= 1.5f;
                    Main.dust[num195].noGravity = true;
                }

                if (Projectile.owner == Main.myPlayer)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, 0f, velocityY, ModContent.ProjectileType<HolyFlare>(), (int)Math.Round(Projectile.damage * 0.75), Projectile.knockBack, Projectile.owner, 0f, 0f);
            }

            if (Projectile.ai[1] == 0f)
            {
                Projectile.ai[1] = 1f;
                SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
            }

            Projectile.velocity *= 0.975f;

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
                Projectile.frame = 0;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return ProvUtils.GetProjectileColor(Projectile.maxPenetrate, Projectile.alpha);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawBackglow(Providence.BackglowColor, 4f);
            
            Texture2D texture = (Projectile.maxPenetrate == (int)Providence.BossMode.Day) ? ModContent.Request<Texture2D>(Texture).Value : ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/HolyBombNight").Value;
            int num214 = texture.Height / Main.projFrames[Projectile.type];
            int y6 = num214 * Projectile.frame;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture.Width, num214)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(texture.Width / 2f, num214 / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
            Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
            Projectile.width = 50;
            Projectile.height = 100;
            Projectile.position.X = Projectile.position.X - (Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (Projectile.height / 2);
            int dustType = ProvUtils.GetDustID(Projectile.maxPenetrate);
            for (int num193 = 0; num193 < 2; num193++)
            {
                int dust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustType, 0f, 0f, 50, default, 1.5f);
                Main.dust[dust].noGravity = true;
            }
            for (int num194 = 0; num194 < 20; num194++)
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

            ProvUtils.ApplyHitEffects(target, Projectile.maxPenetrate, 240, 20);
        }
    }
}
