using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.NPCs.Providence;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class HolyFlare : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Flare");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 50;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.extraUpdates = 1;
            projectile.timeLeft = 840;
            cooldownSlot = 1;
            projectile.Calamity().affectedByMaliceModeVelocityMultiplier = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];

            projectile.frameCounter++;
            if (projectile.frameCounter > 8)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
                projectile.frame = 0;

            float velocityYCap = projectile.position.Y + projectile.height < player.position.Y ? 1f : 0.25f;
            projectile.velocity.Y += 0.01f;
            if (projectile.velocity.Y > velocityYCap)
                projectile.velocity.Y = velocityYCap;

            float velocityX = (!Main.dayTime || CalamityWorld.malice || BossRushEvent.BossRushActive) ? 0.025f : 0.02f;
            if (projectile.position.X + projectile.width < player.position.X)
            {
                if (projectile.velocity.X < 0f)
                    projectile.velocity.X *= 0.99f;
                projectile.velocity.X += velocityX;
            }
            else if (projectile.position.X > player.position.X + player.width)
            {
                if (projectile.velocity.X > 0f)
                    projectile.velocity.X *= 0.99f;
                projectile.velocity.X -= velocityX;
            }

            float velocityXCap = (!Main.dayTime || CalamityWorld.malice || BossRushEvent.BossRushActive) ? 10f : 8f;
            if (projectile.velocity.X > velocityXCap || projectile.velocity.X < -velocityXCap)
                projectile.velocity.X *= 0.97f;

            projectile.rotation = projectile.velocity.X * 0.025f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return ((Main.dayTime && !CalamityWorld.malice) || !NPC.AnyNPCs(ModContent.NPCType<Providence>())) ? new Color(250, 150, 0, projectile.alpha) : new Color(100, 200, 250, projectile.alpha);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = ((Main.dayTime && !CalamityWorld.malice) || !NPC.AnyNPCs(ModContent.NPCType<Providence>())) ? Main.projectileTexture[projectile.type] : ModContent.GetTexture("CalamityMod/Projectiles/Boss/HolyFlareNight");
            int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y6 = num214 * projectile.frame;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, num214)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2(texture2D13.Width / 2f, num214 / 2f), projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item20, projectile.Center);
            projectile.position.X = projectile.position.X + (projectile.width / 2);
            projectile.position.Y = projectile.position.Y + (projectile.height / 2);
            projectile.width = 50;
            projectile.height = 50;
            projectile.position.X = projectile.position.X - (projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (projectile.height / 2);
            int dustType = ((Main.dayTime && !CalamityWorld.malice) || !NPC.AnyNPCs(ModContent.NPCType<Providence>())) ? (int)CalamityDusts.ProfanedFire : (int)CalamityDusts.Nightwither;
            for (int num621 = 0; num621 < 5; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dustType, 0f, 0f, 100, default, 2f);
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 10; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dustType, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dustType, 0f, 0f, 100, default, 2f);
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            int buffType = ((Main.dayTime && !CalamityWorld.malice) || !NPC.AnyNPCs(ModContent.NPCType<Providence>())) ? ModContent.BuffType<HolyFlames>() : ModContent.BuffType<Nightwither>();
            target.AddBuff(buffType, 120);
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            target.Calamity().lastProjectileHit = projectile;
        }
    }
}
