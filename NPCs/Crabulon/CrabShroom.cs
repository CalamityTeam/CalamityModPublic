using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Crabulon
{
    public class CrabShroom : ModNPC
    {
        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.GetNPCDamage();
            NPC.width = 14;
            NPC.height = 14;
            if (CalamityWorld.LegendaryMode && CalamityWorld.revenge)
                NPC.scale = 2f;

            NPC.lifeMax = (CalamityWorld.LegendaryMode && CalamityWorld.revenge) ? 30 : 15;
            if (BossRushEvent.BossRushActive)
                NPC.lifeMax = 8000;
            if (Main.getGoodWorld)
                NPC.lifeMax *= 2;

            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            AIType = -1;
            NPC.knockBackResist = 0.75f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.canGhostHeal = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            Lighting.AddLight((int)((NPC.position.X + (NPC.width / 2)) / 16f), (int)((NPC.position.Y + (NPC.height / 2)) / 16f), 0f, 0.2f, 0.4f);
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            float xVelocityLimit = BossRushEvent.BossRushActive ? 7.5f : 5f;
            float yVelocityLimit = (CalamityWorld.LegendaryMode && CalamityWorld.revenge) ? 0.5f : revenge ? 1.25f : 1f;
            NPC.TargetClosest();
            Player player = Main.player[NPC.target];
            NPC.velocity.Y += 0.02f;
            if (NPC.velocity.Y > yVelocityLimit)
            {
                NPC.velocity.Y = yVelocityLimit;
            }
            if (NPC.position.X + NPC.width < player.position.X)
            {
                if (NPC.velocity.X < 0f)
                {
                    NPC.velocity.X *= 0.98f;
                }
                NPC.velocity.X += 0.1f;
            }
            else if (NPC.position.X > player.position.X + player.width)
            {
                if (NPC.velocity.X > 0f)
                {
                    NPC.velocity.X *= 0.98f;
                }
                NPC.velocity.X -= 0.1f;
            }
            if (NPC.velocity.X > xVelocityLimit || NPC.velocity.X < -xVelocityLimit)
            {
                NPC.velocity.X *= 0.97f;
            }
            NPC.rotation = NPC.velocity.X * 0.1f;

            if (CalamityWorld.LegendaryMode && CalamityWorld.revenge)
            {
                float pushVelocity = 0.5f;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active)
                    {
                        if (i != NPC.whoAmI && Main.npc[i].type == NPC.type)
                        {
                            if (Vector2.Distance(NPC.Center, Main.npc[i].Center) < 30f * NPC.scale)
                            {
                                if (NPC.position.X < Main.npc[i].position.X)
                                    NPC.velocity.X -= pushVelocity;
                                else
                                    NPC.velocity.X += pushVelocity;

                                if (NPC.position.Y < Main.npc[i].position.Y)
                                    NPC.velocity.Y -= pushVelocity;
                                else
                                    NPC.velocity.Y += pushVelocity;
                            }
                        }
                    }
                }
            }
        }

        public override Color? GetAlpha(Color drawColor) => Main.zenithWorld ? new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, drawColor.A) * NPC.Opacity : null;

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>("CalamityMod/NPCs/Crabulon/CrabShroomGlow").Value;
            Color colorToShift = Main.zenithWorld ? new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB) : Color.Cyan;
            Color glowColor = Color.Lerp(Color.White, colorToShift, 0.5f);
            
            int ClonesAroundShroom = Main.zenithWorld ? 4 : 0;
            for (int c = 0; c < 1 + ClonesAroundShroom; c++)
            {
                Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / Main.npcFrameCount[NPC.type] / 2);
                Vector2 drawPos = NPC.Center - screenPos + (Vector2.UnitX * texture.Width * c).RotatedByRandom(c);
                drawPos -= new Vector2(texture.Width, texture.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
                drawPos += drawOrigin * NPC.scale + new Vector2(0f, NPC.gfxOffY);

                spriteBatch.Draw(texture, drawPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0f);
                spriteBatch.Draw(glow, drawPos, NPC.frame, glowColor, NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0f);
            }

            return false;
        }

        public override void OnKill()
        {
            int closestPlayer = Player.FindClosest(NPC.Center, 1, 1);
            if (Main.rand.NextBool(8) && Main.player[closestPlayer].statLife < Main.player[closestPlayer].statLifeMax2)
                Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ItemID.Heart);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 56, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 56, hit.HitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
