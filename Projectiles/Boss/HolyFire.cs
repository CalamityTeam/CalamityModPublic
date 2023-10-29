using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.NPCs;
using CalamityMod.NPCs.Providence;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Boss
{
    public class HolyFire : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.tileCollide = false;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.3f, 0.225f, 0f);

            // Day mode by default but syncs with the boss
            if (CalamityGlobalNPC.holyBoss != -1)
            {
                if (Main.npc[CalamityGlobalNPC.holyBoss].active)
                    Projectile.maxPenetrate = (int)Main.npc[CalamityGlobalNPC.holyBoss].localAI[1];
            }
            else
                Projectile.maxPenetrate = (int)Providence.BossMode.Day;

            if (Projectile.ai[1] == 0f)
            {
                Projectile.ai[1] = 1f;
                SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
            }

            Projectile.alpha -= (Projectile.maxPenetrate != (int)Providence.BossMode.Day) ? 10 : 5;
            if (Projectile.alpha <= 0)
                Projectile.Kill();

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
            Texture2D texture = (Projectile.maxPenetrate == (int)Providence.BossMode.Day) ? ModContent.Request<Texture2D>(Texture).Value : ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/HolyFireNight").Value;
            int framing = texture.Height / Main.projFrames[Projectile.type];
            int y6 = framing * Projectile.frame;
            Projectile.DrawBackglow(ProvUtils.GetProjectileColor(Projectile.maxPenetrate, Projectile.alpha, true), 4f, texture);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture.Width, framing)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(texture.Width / 2f, framing / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.rand.NextBool())
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    Vector2 velocity = new Vector2(0.01f, 0f);
                    if (CalamityWorld.LegendaryMode && CalamityWorld.revenge)
                        velocity *= Main.rand.NextFloat(1f, 2f);

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<HolyFire2>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

                    if (CalamityWorld.LegendaryMode && CalamityWorld.revenge)
                        velocity *= Main.rand.NextFloat(1f, 2f);

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, -velocity, ModContent.ProjectileType<HolyFire2>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
            else
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    Vector2 velocity = new Vector2(0.05f, 0f);
                    if (CalamityWorld.LegendaryMode && CalamityWorld.revenge)
                        velocity *= Main.rand.NextFloat(1f, 2f);

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<HolyFire2>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

                    if (CalamityWorld.LegendaryMode && CalamityWorld.revenge)
                        velocity *= Main.rand.NextFloat(1f, 2f);

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, -velocity, ModContent.ProjectileType<HolyFire2>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
            SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
            Projectile.position.X = Projectile.position.X + (Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y + (Projectile.height / 2);
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.position.X = Projectile.position.X - (Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (Projectile.height / 2);
            int dustType = ProvUtils.GetDustID(Projectile.maxPenetrate);
            for (int i = 0; i < 5; i++)
            {
                int holyDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustType, 0f, 0f, 100, default, 2f);
                Main.dust[holyDust].noGravity = true;
                if (Main.rand.NextBool())
                {
                    Main.dust[holyDust].scale = 0.5f;
                    Main.dust[holyDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            for (int j = 0; j < 10; j++)
            {
                int holyDust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustType, 0f, 0f, 100, default, 3f);
                Main.dust[holyDust2].noGravity = true;
                holyDust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustType, 0f, 0f, 100, default, 2f);
                Main.dust[holyDust2].noGravity = true;
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

            ProvUtils.ApplyHitEffects(target, Projectile.maxPenetrate, 80, 20);
        }
    }
}
