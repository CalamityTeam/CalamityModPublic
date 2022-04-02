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
            Main.npcFrameCount[npc.type] = 5;
            NPCID.Sets.TrailingMode[npc.type] = 1;
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            aiType = -1;
            npc.width = 40;
            npc.height = 40;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.canGhostHeal = false;
            npc.damage = 40;
            npc.defense = 10;
            npc.DR_NERD(0.1f);
            npc.lifeMax = CalamityWorld.death ? 1500 : 2500;
            if (BossRushEvent.BossRushActive)
            {
                npc.lifeMax = 15000;
            }
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToCold = true;
            npc.Calamity().VulnerableToWater = true;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override bool PreAI()
        {
            // Setting this in SetDefaults will disable expert mode scaling, so put it here instead
            npc.damage = 0;

            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            if (CalamityGlobalNPC.calamitas < 0 || !Main.npc[CalamityGlobalNPC.calamitas].active)
            {
                npc.active = false;
                npc.netUpdate = true;
                return false;
            }

            NPC parent = Main.npc[CalamityGlobalNPC.calamitas];
            if (start)
            {
                for (int d = 0; d < 15; d++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                }
                npc.ai[1] = npc.ai[0];
                start = false;
            }

            npc.TargetClosest();

            Vector2 velocity = Main.player[npc.target].Center - npc.Center;
            velocity.Normalize();
            velocity *= 9f;
            npc.rotation = velocity.ToRotation() + MathHelper.Pi;

            timer++;
            if (timer >= 180)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient && parent.ai[1] < 2f && parent.Calamity().newAI[2] <= 0f)
                {
                    for (int d = 0; d < 3; d++)
                        Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);

                    int type = ModContent.ProjectileType<BrimstoneBarrage>();
                    int damage = npc.GetProjectileDamage(type);
                    Projectile.NewProjectile(npc.Center, velocity, type, damage, 1f, npc.target, 1f, 0f);
                }
                timer = 0;
            }

            double deg = npc.ai[1];
            double rad = deg * (Math.PI / 180);
            double dist = death ? 180 : 150;
            npc.position.X = parent.Center.X - (int)(Math.Cos(rad) * dist) - npc.width / 2;
            npc.position.Y = parent.Center.Y - (int)(Math.Sin(rad) * dist) - npc.height / 2;
            npc.ai[1] += death ? 0.5f : 2f;
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.Brimstone, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/CalamitasGores/SoulSeeker"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/CalamitasGores/SoulSeeker2"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/CalamitasGores/SoulSeeker3"), 1f);
                npc.position = npc.Center;
                npc.width = npc.height = 50;
                npc.Center = npc.position;
                for (int d = 0; d < 20; d++)
                {
                    int red = Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                    Main.dust[red].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[red].scale = 0.5f;
                        Main.dust[red].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int d = 0; d < 40; d++)
                {
                    int red = Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 3f);
                    Main.dust[red].noGravity = true;
                    Main.dust[red].velocity *= 5f;
                    red = Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                    Main.dust[red].velocity *= 2f;
                }
            }
        }

        public override void NPCLoot()
        {
            if (!CalamityWorld.revenge)
            {
                int closestPlayer = Player.FindClosest(npc.Center, 1, 1);
                if (Main.rand.Next(4) == 0 && Main.player[closestPlayer].statLife < Main.player[closestPlayer].statLifeMax2)
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Heart);
            }
        }

        public override bool CheckActive() => false;

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (npc.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture = Main.npcTexture[npc.type];
            Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / Main.npcFrameCount[npc.type] / 2f);
            Color white = Color.White;
            float colorLerpAmt = 0.5f;
            int afterImageAmt = 5;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int a = 1; a < afterImageAmt; a += 2)
                {
                    Color afterImageColor = lightColor;
                    afterImageColor = Color.Lerp(afterImageColor, white, colorLerpAmt);
                    afterImageColor = npc.GetAlpha(afterImageColor);
                    afterImageColor *= (afterImageAmt - a) / 15f;
                    Vector2 afterimagePos = npc.oldPos[a] + new Vector2(npc.width, npc.height) / 2f - Main.screenPosition;
                    afterimagePos -= new Vector2(texture.Width, texture.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
                    afterimagePos += origin * npc.scale + new Vector2(0f, npc.gfxOffY);
                    spriteBatch.Draw(texture, afterimagePos, npc.frame, afterImageColor, npc.rotation, origin, npc.scale, spriteEffects, 0f);
                }
            }

            Vector2 drawPos = npc.Center - Main.screenPosition;
            drawPos -= new Vector2(texture.Width, texture.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
            drawPos += origin * npc.scale + new Vector2(0f, npc.gfxOffY);
            spriteBatch.Draw(texture, drawPos, npc.frame, npc.GetAlpha(lightColor), npc.rotation, origin, npc.scale, spriteEffects, 0f);

            texture = ModContent.GetTexture("CalamityMod/NPCs/Calamitas/SoulSeekerGlow");
            Color glow = Color.Lerp(Color.White, Color.Red, colorLerpAmt);

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int a = 1; a < afterImageAmt; a++)
                {
                    Color glowColor = glow;
                    glowColor = Color.Lerp(glowColor, white, colorLerpAmt);
                    glowColor *= (afterImageAmt - a) / 15f;
                    Vector2 afterimagePos = npc.oldPos[a] + new Vector2(npc.width, npc.height) / 2f - Main.screenPosition;
                    afterimagePos -= new Vector2(texture.Width, texture.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
                    afterimagePos += origin * npc.scale + new Vector2(0f, npc.gfxOffY);
                    spriteBatch.Draw(texture, afterimagePos, npc.frame, glowColor, npc.rotation, origin, npc.scale, spriteEffects, 0f);
                }
            }

            spriteBatch.Draw(texture, drawPos, npc.frame, white, npc.rotation, origin, npc.scale, spriteEffects, 0f);

            return false;
        }
    }
}
