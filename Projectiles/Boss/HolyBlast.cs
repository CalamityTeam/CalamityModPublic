using CalamityMod.NPCs;
using CalamityMod.NPCs.Providence;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using System.IO;
using CalamityMod.World;

namespace CalamityMod.Projectiles.Boss
{
    public class HolyBlast : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public static readonly SoundStyle ShootSound = new("CalamityMod/Sounds/Custom/Providence/ProvidenceHolyBlastShoot");
        public static readonly SoundStyle ImpactSound = new("CalamityMod/Sounds/Custom/Providence/ProvidenceHolyBlastImpact");

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.Calamity().DealsDefenseDamage = true;
            Projectile.width = 180;
            Projectile.height = 180;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.9f, 0.7f, 0f);

            if (Projectile.Hitbox.Intersects(new Rectangle((int)Projectile.ai[0], (int)Projectile.ai[1], Player.defaultWidth, Player.defaultHeight)))
                Projectile.tileCollide = true;

            // Day mode by default but syncs with the boss
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
                Projectile.frame = 0;

            if (Projectile.localAI[0] == 0f)
            {
                int dustType = ProvUtils.GetDustID(Projectile.maxPenetrate);
                for (int i = 0; i < 10; i++)
                {
                    int holyDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustType, 0f, 0f, 100, default, 2f);
                    Main.dust[holyDust].velocity *= 3f;
                    Main.dust[holyDust].noGravity = true;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[holyDust].scale = 0.5f;
                        Main.dust[holyDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                Projectile.localAI[0] = 1f;
                SoundEngine.PlaySound(ShootSound, Projectile.Center);
            }

            if (Math.Abs(Projectile.velocity.X) > 0.2)
                Projectile.spriteDirection = -Projectile.direction;

            if (Projectile.velocity.X < 0f)
                Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
            else
                Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return ProvUtils.GetProjectileColor(Projectile.maxPenetrate, Projectile.alpha);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = (Projectile.maxPenetrate != (int)Providence.BossMode.Day) ? ModContent.Request<Texture2D>(Texture).Value : ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/HolyBlastNight").Value;
            int framing = texture.Height / Main.projFrames[Projectile.type];
            int y6 = framing * Projectile.frame;
            Projectile.DrawBackglow(ProvUtils.GetProjectileColor(Projectile.maxPenetrate, Projectile.alpha, true), 4f, texture);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture.Width, framing)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(texture.Width / 2f, framing / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                int totalProjectiles = (Projectile.maxPenetrate != (int)Providence.BossMode.Day) ? 8 : 6;
                if (CalamityWorld.LegendaryMode && CalamityWorld.revenge)
                    totalProjectiles *= 2;

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
            for (int j = 0; j < 4; j++)
            {
                int dust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustType, 0f, 0f, 50, default, 2f);
                Main.dust[dust].noGravity = true;
            }
            for (int k = 0; k < 40; k++)
            {
                int profaned = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustType, 0f, 0f, 0, default, 4f);
                Main.dust[profaned].noGravity = true;
                Main.dust[profaned].velocity *= 3f;
                profaned = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustType, 0f, 0f, 50, default, 2f);
                Main.dust[profaned].velocity *= 2f;
                Main.dust[profaned].noGravity = true;
            }
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            //In GFB, "real damage" is replaced with negative healing
            if (Projectile.maxPenetrate >= (int)Providence.BossMode.Red)
                modifiers.SourceDamage *= 0f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            // If the player is dodging, don't apply debuffs
            if ((info.Damage <= 0 && Projectile.maxPenetrate < (int)Providence.BossMode.Red) || target.creativeGodMode)
                return;

            ProvUtils.ApplyHitEffects(target, Projectile.maxPenetrate, 320, 20);
        }
    }
}
