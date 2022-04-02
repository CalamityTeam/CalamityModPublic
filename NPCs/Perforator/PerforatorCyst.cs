using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Perforator
{
    public class PerforatorCyst : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Perforator Cyst");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.npcSlots = 0.1f;
            npc.aiStyle = -1;
            aiType = -1;
            npc.damage = 0;
            npc.width = 30; //324
            npc.height = 26; //216
            npc.defense = 0;
            npc.lifeMax = 1000;
            npc.knockBackResist = 0f;
            npc.chaseable = false;
            npc.HitSound = SoundID.NPCHit13;
            npc.rarity = 2;
            npc.Calamity().VulnerableToHeat = true;
            npc.Calamity().VulnerableToCold = true;
            npc.Calamity().VulnerableToSickness = true;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (npc.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = Main.npcTexture[npc.type];
            Vector2 vector11 = new Vector2((float)(Main.npcTexture[npc.type].Width / 2), (float)(Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2));

            Vector2 vector43 = npc.Center - Main.screenPosition;
            vector43 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[npc.type])) * npc.scale / 2f;
            vector43 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
            spriteBatch.Draw(texture2D15, vector43, npc.frame, npc.GetAlpha(lightColor), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

            texture2D15 = ModContent.GetTexture("CalamityMod/NPCs/Perforator/PerforatorCystGlow");
            Color color37 = Color.Lerp(Color.White, Color.Yellow, 0.5f);

            spriteBatch.Draw(texture2D15, vector43, npc.frame, color37, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

            return false;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (CalamityGlobalNPC.AnyEvents(spawnInfo.player))
                return 0f;
            if (spawnInfo.player.Calamity().disablePerfCystSpawns)
                return 0f;

            bool anyBossElements = NPC.AnyNPCs(ModContent.NPCType<PerforatorCyst>()) || NPC.AnyNPCs(ModContent.NPCType<PerforatorHive>());
            bool crimson = TileID.Sets.Crimson[spawnInfo.spawnTileType] || spawnInfo.spawnTileType == TileID.Crimtane && spawnInfo.player.ZoneCrimson;
            if (spawnInfo.playerSafe || anyBossElements || !crimson)
                return 0f;

            if (NPC.downedBoss2 && !CalamityWorld.downedPerforator)
                return 1.5f;

            return Main.hardMode ? 0.05f : 0.5f;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = 2000;
            npc.damage = 0;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                }
                if (Main.netMode != NetmodeID.MultiplayerClient && NPC.CountNPCS(ModContent.NPCType<PerforatorHive>()) < 1)
                {
                    Vector2 spawnAt = npc.Center + new Vector2(0f, (float)npc.height / 2f);
                    NPC.NewNPC((int)spawnAt.X, (int)spawnAt.Y, ModContent.NPCType<PerforatorHive>());
                }
            }
        }
    }
}
