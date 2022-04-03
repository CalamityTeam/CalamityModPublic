using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Calamitas
{
    public class SoulSeeker : ModNPC
    {
        private int timer = 0;
        private bool start = true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soul Seeker");
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.width = 40;
            NPC.height = 40;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.canGhostHeal = false;
            NPC.damage = 40;
            NPC.defense = 10;
            NPC.DR_NERD(0.1f);
            NPC.lifeMax = CalamityWorld.death ? 1500 : 2500;
            if (BossRushEvent.BossRushActive)
            {
                NPC.lifeMax = 15000;
            }
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToWater = true;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override bool PreAI()
        {
            // Setting this in SetDefaults will disable expert mode scaling, so put it here instead
            NPC.damage = 0;

            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            if (CalamityGlobalNPC.calamitas < 0 || !Main.npc[CalamityGlobalNPC.calamitas].active)
            {
                NPC.active = false;
                NPC.netUpdate = true;
                return false;
            }

            NPC parent = Main.npc[CalamityGlobalNPC.calamitas];
            if (start)
            {
                for (int d = 0; d < 15; d++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                }
                NPC.ai[1] = NPC.ai[0];
                start = false;
            }

            NPC.TargetClosest();

            Vector2 velocity = Main.player[NPC.target].Center - NPC.Center;
            velocity.Normalize();
            velocity *= 9f;
            NPC.rotation = velocity.ToRotation() + MathHelper.Pi;

            timer++;
            if (timer >= 180)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient && parent.ai[1] < 2f && parent.Calamity().newAI[2] <= 0f)
                {
                    for (int d = 0; d < 3; d++)
                        Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);

                    int type = ModContent.ProjectileType<BrimstoneBarrage>();
                    int damage = NPC.GetProjectileDamage(type);
                    Projectile.NewProjectile(NPC.Center, velocity, type, damage, 1f, NPC.target, 1f, 0f);
                }
                timer = 0;
            }

            double deg = NPC.ai[1];
            double rad = deg * (Math.PI / 180);
            double dist = death ? 180 : 150;
            NPC.position.X = parent.Center.X - (int)(Math.Cos(rad) * dist) - NPC.width / 2;
            NPC.position.Y = parent.Center.Y - (int)(Math.Sin(rad) * dist) - NPC.height / 2;
            NPC.ai[1] += death ? 0.5f : 2f;
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Brimstone, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/CalamitasGores/SoulSeeker"), 1f);
                Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/CalamitasGores/SoulSeeker2"), 1f);
                Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/CalamitasGores/SoulSeeker3"), 1f);
                NPC.position = NPC.Center;
                NPC.width = NPC.height = 50;
                NPC.Center = NPC.position;
                for (int d = 0; d < 20; d++)
                {
                    int red = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                    Main.dust[red].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[red].scale = 0.5f;
                        Main.dust[red].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int d = 0; d < 40; d++)
                {
                    int red = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 3f);
                    Main.dust[red].noGravity = true;
                    Main.dust[red].velocity *= 5f;
                    red = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                    Main.dust[red].velocity *= 2f;
                }
            }
        }

        public override void NPCLoot()
        {
            if (!CalamityWorld.revenge)
            {
                int closestPlayer = Player.FindClosest(NPC.Center, 1, 1);
                if (Main.rand.Next(4) == 0 && Main.player[closestPlayer].statLife < Main.player[closestPlayer].statLifeMax2)
                    Item.NewItem((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ItemID.Heart);
            }
        }

        public override bool CheckActive() => false;

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture = Main.npcTexture[NPC.type];
            Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / Main.npcFrameCount[NPC.type] / 2f);
            Color white = Color.White;
            float colorLerpAmt = 0.5f;
            int afterImageAmt = 5;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int a = 1; a < afterImageAmt; a += 2)
                {
                    Color afterImageColor = lightColor;
                    afterImageColor = Color.Lerp(afterImageColor, white, colorLerpAmt);
                    afterImageColor = NPC.GetAlpha(afterImageColor);
                    afterImageColor *= (afterImageAmt - a) / 15f;
                    Vector2 afterimagePos = NPC.oldPos[a] + new Vector2(NPC.width, NPC.height) / 2f - Main.screenPosition;
                    afterimagePos -= new Vector2(texture.Width, texture.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
                    afterimagePos += origin * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture, afterimagePos, NPC.frame, afterImageColor, NPC.rotation, origin, NPC.scale, spriteEffects, 0f);
                }
            }

            Vector2 drawPos = NPC.Center - Main.screenPosition;
            drawPos -= new Vector2(texture.Width, texture.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
            drawPos += origin * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture, drawPos, NPC.frame, NPC.GetAlpha(lightColor), NPC.rotation, origin, NPC.scale, spriteEffects, 0f);

            texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/Calamitas/SoulSeekerGlow");
            Color glow = Color.Lerp(Color.White, Color.Red, colorLerpAmt);

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int a = 1; a < afterImageAmt; a++)
                {
                    Color glowColor = glow;
                    glowColor = Color.Lerp(glowColor, white, colorLerpAmt);
                    glowColor *= (afterImageAmt - a) / 15f;
                    Vector2 afterimagePos = NPC.oldPos[a] + new Vector2(NPC.width, NPC.height) / 2f - Main.screenPosition;
                    afterimagePos -= new Vector2(texture.Width, texture.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
                    afterimagePos += origin * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture, afterimagePos, NPC.frame, glowColor, NPC.rotation, origin, NPC.scale, spriteEffects, 0f);
                }
            }

            spriteBatch.Draw(texture, drawPos, NPC.frame, white, NPC.rotation, origin, NPC.scale, spriteEffects, 0f);

            return false;
        }
    }
}
