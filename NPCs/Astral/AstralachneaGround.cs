using CalamityMod.BiomeManagers;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using CalamityMod.Sounds;

namespace CalamityMod.NPCs.Astral
{
    public class AstralachneaGround : ModNPC
    {
        private static Texture2D glowmask;

        public override void SetStaticDefaults()
        {

            Main.npcFrameCount[NPC.type] = 5;

            if (!Main.dedServ)
                glowmask = ModContent.Request<Texture2D>("CalamityMod/NPCs/Astral/AstralachneaGroundGlow", AssetRequestMode.ImmediateLoad).Value;

            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            NPC.width = 70;
            NPC.height = 34;
            NPC.aiStyle = NPCAIStyleID.Fighter;
            NPC.damage = 55;
            NPC.defense = 20;
            NPC.DR_NERD(0.15f);
            NPC.lifeMax = 500;
            NPC.DeathSound = CommonCalamitySounds.AstralNPCDeathSound;
            NPC.knockBackResist = 0.38f;
            NPC.value = Item.buyPrice(0, 0, 20, 0);
            NPC.timeLeft = NPC.activeTime * 2;
            AnimationType = NPCID.WallCreeper;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<AstralachneaBanner>();
            if (DownedBossSystem.downedAstrumAureus)
            {
                NPC.damage = 90;
                NPC.defense = 30;
                NPC.knockBackResist = 0.28f;
                NPC.lifeMax = 750;
            }
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToSickness = false;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<UndergroundAstralBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.Astralachnea")
            });
        }

        public override void AI()
        {
            NPC.TargetClosest();
            if (Main.netMode != NetmodeID.MultiplayerClient && NPC.velocity.Y == 0f)
            {
                int x = (int)NPC.Center.X / 16;
                int y = (int)NPC.Center.Y / 16;
                bool transform = false;
                for (int i = x - 1; i <= x + 1; i++)
                {
                    for (int j = y - 1; j <= y + 1; j++)
                    {
                        if (Main.tile[i, j] != null && Main.tile[i, j].WallType > 0)
                        {
                            transform = true;
                        }
                    }
                }
                if (transform)
                {
                    NPC.Transform(ModContent.NPCType<AstralachneaWall>());
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                NPC.frameCounter += 1;
                if (NPC.frameCounter > 6)
                {
                    NPC.frame.Y += frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y > frameHeight * 5)
                {
                    NPC.frame.Y = 0;
                }
            }
            //DO DUST
            int frame = NPC.frame.Y / frameHeight;
            Rectangle rect = new Rectangle(62, 4, 14, 6);
            switch (frame)
            {
                case 1:
                    rect = new Rectangle(64, 6, 12, 6);
                    break;
                case 2:
                    rect = new Rectangle(58, 8, 22, 6);
                    break;
                case 3:
                    rect = new Rectangle(54, 8, 26, 8);
                    break;
                case 4:
                    rect = new Rectangle(58, 6, 20, 8);
                    break;
            }
            Dust d = CalamityGlobalNPC.SpawnDustOnNPC(NPC, 80, frameHeight, ModContent.DustType<AstralOrange>(), rect, Vector2.Zero, 0.45f, true);
            if (d != null)
            {
                d.customData = 0.04f;
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.soundDelay == 0)
            {
                NPC.soundDelay = 15;
                SoundEngine.PlaySound(CommonCalamitySounds.AstralNPCHitSound, NPC.Center);
            }

            CalamityGlobalNPC.DoHitDust(NPC, hit.HitDirection, (Main.rand.Next(0, Math.Max(0, NPC.life)) == 0) ? 5 : ModContent.DustType<AstralEnemy>(), 1f, 4, 22);

            //if dead do gores
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity * 0.3f, Mod.Find<ModGore>("AstralachneaGore" + i).Type);
                    }
                }
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 origin = new Vector2(40f, 21f);
            spriteBatch.Draw(glowmask, NPC.Center - screenPos, NPC.frame, Color.White * 0.6f, NPC.rotation, origin, 1f, NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (CalamityGlobalNPC.AnyEvents(spawnInfo.Player))
            {
                return 0f;
            }
            else if (spawnInfo.Player.InAstral(2))
            {
                return 0.17f;
            }
            return 0f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120, true);
        }

        public static void ModifyAstralachneaLoot(NPCLoot npcLoot)
        {
            npcLoot.AddIf(() => !Main.expertMode, ModContent.ItemType<Stardust>(), 2, 2, 3);
            npcLoot.AddIf(() => Main.expertMode, ModContent.ItemType<Stardust>(), 1, 1, 4);
            npcLoot.AddIf(() => DownedBossSystem.downedAstrumAureus, ModContent.ItemType<AstralachneaStaff>(), 7);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) => ModifyAstralachneaLoot(npcLoot);
    }
}
