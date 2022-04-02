using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.NPCs.Providence;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class HolyBomb : ModProjectile
    {
        private int flareShootTimer = 120;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Bomb");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 80;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 250;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            cooldownSlot = 1;
            projectile.Calamity().affectedByMaliceModeVelocityMultiplier = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(flareShootTimer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            flareShootTimer = reader.ReadInt32();
        }

        public override void AI()
        {
            flareShootTimer--;
            if (flareShootTimer <= 0)
            {
                Main.PlaySound(SoundID.Item20, projectile.Center);
                projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
                projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
                projectile.width = 50;
                projectile.height = 50;
                projectile.position.X = projectile.position.X - (projectile.width / 2);
                projectile.position.Y = projectile.position.Y - (projectile.height / 2);
                if (projectile.owner == Main.myPlayer)
                {
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, -2f, ModContent.ProjectileType<HolyFlare>(), (int)Math.Round(projectile.damage * 0.75), projectile.knockBack, projectile.owner, 0f, 0f);
                }
                flareShootTimer = 60;
            }
            if (projectile.ai[1] == 0f)
            {
                projectile.ai[1] = 1f;
                Main.PlaySound(SoundID.Item20, projectile.Center);
            }
            projectile.velocity *= 0.975f;
            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
            {
                projectile.frame = 0;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return ((Main.dayTime && !CalamityWorld.malice) || !NPC.AnyNPCs(ModContent.NPCType<Providence>())) ? new Color(250, 150, 0, projectile.alpha) : new Color(100, 200, 250, projectile.alpha);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = ((Main.dayTime && !CalamityWorld.malice) || !NPC.AnyNPCs(ModContent.NPCType<Providence>())) ? Main.projectileTexture[projectile.type] : ModContent.GetTexture("CalamityMod/Projectiles/Boss/HolyBombNight");
            int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y6 = num214 * projectile.frame;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, num214)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2(texture2D13.Width / 2f, num214 / 2f), projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item20, projectile.Center);
            projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
            projectile.width = 150;
            projectile.height = 150;
            projectile.position.X = projectile.position.X - (projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (projectile.height / 2);
            int dustType = ((Main.dayTime && !CalamityWorld.malice) || !NPC.AnyNPCs(ModContent.NPCType<Providence>())) ? (int)CalamityDusts.ProfanedFire : (int)CalamityDusts.Nightwither;
            for (int num193 = 0; num193 < 2; num193++)
            {
                Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dustType, 0f, 0f, 50, default, 1.5f);
            }
            for (int num194 = 0; num194 < 20; num194++)
            {
                int num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dustType, 0f, 0f, 0, default, 2.5f);
                Main.dust[num195].noGravity = true;
                Main.dust[num195].velocity *= 3f;
                num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dustType, 0f, 0f, 50, default, 1.5f);
                Main.dust[num195].velocity *= 2f;
                Main.dust[num195].noGravity = true;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            int buffType = ((Main.dayTime && !CalamityWorld.malice) || !NPC.AnyNPCs(ModContent.NPCType<Providence>())) ? ModContent.BuffType<HolyFlames>() : ModContent.BuffType<Nightwither>();
            target.AddBuff(buffType, 180);
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)    
        {
            target.Calamity().lastProjectileHit = projectile;
        }
    }
}
