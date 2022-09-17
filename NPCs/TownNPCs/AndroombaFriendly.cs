using CalamityMod.BiomeManagers;
using CalamityMod.Items.Ammo;
using CalamityMod.Items.Critters;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.TownNPCs
{
    public class AndroombaFriendly : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Androomba Pal");
            Main.npcFrameCount[NPC.type] = 9;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.damage = 0;
            NPC.width = 36;
            NPC.height = 16;
            NPC.lifeMax = 80;
            NPC.friendly = true;
            NPC.townNPC = true;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.chaseable = false;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath44;
            NPC.catchItem = (short)ModContent.ItemType<AndroombaItem>();
            SpawnModBiomes = new int[1] { ModContent.GetInstance<ArsenalLabBiome>().Type };
            NPC.dontTakeDamage = true;
        }

        public override bool CanChat()
        {
            return false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {

				// Will move to localization whenever that is cleaned up.
				new FlavorTextBestiaryInfoElement("Easily reversed engineered, these robots can be made to spread whatever you put into them, giving them a new purpose.")
            });
        }

        public override void AI()
        {
            // Gravity
            NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y + 0.4f, -15f, 15f);
            NPC.spriteDirection = (int)NPC.ai[2];
            switch (NPC.ai[0])
            {
                case 0:
                    {
                        if (NPC.ai[1] == 0)
                        {
                            NPC.ai[2] = (Main.LocalPlayer.position.X <= NPC.position.X) ? 1 : -1;
                        }
                        GrabSolution(true);
                    }
                    break;
                // Main
                case 1:
                    {
                        NPC.ai[1]++;
                        NPC.velocity.X = NPC.ai[2] * 2;
                        if (!Collision.CanHit(NPC.Center - Vector2.UnitX * NPC.ai[2] * 8f, 2, 2, NPC.Center + Vector2.UnitX * NPC.ai[2] * 32f, 8, 8))
                        {
                            ChangeAI(2);
                        }
                        Convert((int)NPC.ai[3]);
                        GrabSolution(false);
                    }
                    break;
                // Turn
                case 2:
                    {
                        NPC.velocity.X = 0;
                        Convert((int)NPC.ai[3]);
                        GrabSolution(false);
                    }
                    break;
            }
        }

        public void GrabSolution(bool reset = false)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                bool holdingsol = ((Main.LocalPlayer.HeldItem.type >= ItemID.GreenSolution && Main.LocalPlayer.HeldItem.type <= ItemID.RedSolution) || Main.LocalPlayer.HeldItem.type == ModContent.ItemType<AstralSolution>());
                if (NPC.Hitbox.Contains(Main.MouseWorld.ToPoint()) && holdingsol && Main.LocalPlayer.Distance(NPC.Center) < 450)
                {
                    Main.LocalPlayer.cursorItemIconEnabled = true;
                    Main.LocalPlayer.cursorItemIconID = Main.LocalPlayer.HeldItem.type;
                    Main.LocalPlayer.cursorItemIconText = "";
                    NPC.ShowNameOnHover = false;

                    if (Main.mouseRight && Main.mouseRightRelease && Main.LocalPlayer.Distance(NPC.Center) < 300)
                    {
                        NPC.netUpdate = true;

                        int soltype = 0;
                        if (Main.LocalPlayer.HeldItem.type == ModContent.ItemType<AstralSolution>())
                        {
                            soltype = 5;
                        }
                        else
                        {
                            switch (Main.LocalPlayer.HeldItem.type)
                            {
                                case ItemID.GreenSolution:
                                    soltype = 0;
                                    break;
                                case ItemID.PurpleSolution:
                                    soltype = 1;
                                    break;
                                case ItemID.BlueSolution:
                                    soltype = 2;
                                    break;
                                case ItemID.DarkBlueSolution:
                                    soltype = 3;
                                    break;
                                case ItemID.RedSolution:
                                    soltype = 4;
                                    break;
                            }
                        }
                        if (NPC.ai[3] != soltype || NPC.ai[0] == 0)
                        {
                            Main.LocalPlayer.ConsumeItem(Main.LocalPlayer.HeldItem.type);
                            SoundEngine.PlaySound(SoundID.Item87);
                            NPC.ai[3] = soltype;
                            if (reset)
                            {
                                ChangeAI(1);
                            }
                        }
                    }
                }

                else
                    NPC.ShowNameOnHover = true;
            }
        }

        public void Convert(int conversionType)
        {
            int x = (int)(NPC.Center.X / 16f);
            int y = (int)(NPC.Center.Y / 16f);
            if (conversionType <= 4)
            {
                ConvertType type = ConvertType.Pure;
                switch (conversionType)
                {
                    case 0:
                        type = ConvertType.Pure;
                        break;
                    case 1:
                        type = ConvertType.Corrupt;
                        break;
                    case 2:
                        type = ConvertType.Hallow;
                        break;
                    case 4:
                        type = ConvertType.Crimson;
                        break;
                }
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        AstralBiome.ConvertFromAstral(x + i, y + j, type);
                        AstralBiome.ConvertFromAstral(x - i, y - j, type);
                        AstralBiome.ConvertFromAstral(x + i, y - j, type);
                        AstralBiome.ConvertFromAstral(x - i, y + j, type);
                    }
                }
                WorldGen.Convert(x, y, conversionType, 2);
            }
            else
            {
                AstralBiome.ConvertToAstral(x - 2, x + 2, y - 2, y + 2);
            }
        }

        public void ChangeAI(int phase)
        {
            NPC.ai[0] = phase;
            NPC.ai[1] = 0;
            NPC.netUpdate = true;
        }

        public override bool? CanBeHitByItem(Player player, Item item) => null;

        public override bool? CanBeHitByProjectile(Projectile projectile) => null;

        public override void FindFrame(int frameHeight)
        {
            /*
            -Frame 0: Asleep
		    -Frames 1-4: Moving
		    -Frames 5-9: Turning
            */
            NPC.frameCounter += 1.0;
            if (NPC.frameCounter > 6.0)
            {
                NPC.frameCounter = 0.0;
                NPC.frame.Y += frameHeight;
            }

            // Idle
            if (NPC.ai[0] == 1 || NPC.IsABestiaryIconDummy)
            {
                if (NPC.frame.Y > frameHeight * 4)
                {
                    NPC.frameCounter = 0.0;
                    NPC.frame.Y = frameHeight;
                }
            }
            // Turnaround
            else if (NPC.ai[0] == 2)
            {
                if (NPC.frame.Y < frameHeight * 5)
                {
                    NPC.frameCounter = 0.0;
                    NPC.frame.Y = frameHeight * 5;
                }
                if (NPC.frame.Y > frameHeight * 8)
                {
                    NPC.frame.Y = frameHeight;
                    NPC.ai[2] *= -1;
                    ChangeAI(1);
                }
            }
            // Sleep
            else
            {
                NPC.frame.Y = 0;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int i = 0; i < 6; i++)
                Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, 226);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D critterTexture = TextureAssets.Npc[NPC.type].Value;
            string pathextenstion = "Pure";
            switch (NPC.ai[3])
            {
                case 0:
                    pathextenstion = "Pure";
                    break;
                case 1:
                    pathextenstion = "Corruption";
                    break;
                case 2:
                    pathextenstion = "Hallow";
                    break;
                case 3:
                    pathextenstion = "Mushroom";
                    break;
                case 4:
                    pathextenstion = "Crimson";
                    break;
                case 5:
                    pathextenstion = "Astral";
                    break;
            }
            Texture2D glowmask = ModContent.Request<Texture2D>("CalamityMod/NPCs/TownNPCs/AndroombaFriendly_" + pathextenstion).Value;
            Vector2 drawPosition = NPC.Center - screenPos + Vector2.UnitY * NPC.gfxOffY;
            drawPosition.Y += DrawOffsetY;
            SpriteEffects direction = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(critterTexture, drawPosition, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, direction, 0f);
            spriteBatch.Draw(glowmask, drawPosition, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, direction, 0f);
            return false;
        }
    }
}
