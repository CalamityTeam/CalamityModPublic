using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Perforator
{
    public class PerforatorCyst : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0);
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override void SetDefaults()
        {
            NPC.npcSlots = 0.1f;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.damage = 0;
            NPC.width = 30; //324
            NPC.height = 26; //216
            NPC.defense = 0;
            NPC.lifeMax = 1000;
            NPC.knockBackResist = 0f;
            NPC.chaseable = false;
            NPC.HitSound = SoundID.NPCHit13;
            NPC.rarity = 2;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;
            NPC.Calamity().ProvidesProximityRage = false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundCrimson,
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.PerforatorCyst")
            });
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = TextureAssets.Npc[NPC.type].Value;
            Vector2 vector11 = new Vector2((float)(TextureAssets.Npc[NPC.type].Value.Width / 2), (float)(TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2));

            Vector2 vector43 = NPC.Center - screenPos;
            vector43 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[NPC.type])) * NPC.scale / 2f;
            vector43 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, vector43, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            texture2D15 = ModContent.Request<Texture2D>("CalamityMod/NPCs/Perforator/PerforatorCystGlow").Value;
            Color color37 = Color.Lerp(Color.White, Color.Yellow, 0.5f);

            spriteBatch.Draw(texture2D15, vector43, NPC.frame, color37, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (CalamityGlobalNPC.AnyEvents(spawnInfo.Player) || !spawnInfo.Player.ZoneCrimson)
                return 0f;

            if (spawnInfo.Player.Calamity().disablePerfCystSpawns)
                return 0f;

            bool crimson = TileID.Sets.Crimson[spawnInfo.SpawnTileType] || spawnInfo.SpawnTileType == TileID.Crimtane;
            if (spawnInfo.PlayerSafe || !crimson)
                return 0f;

            // Keep this as a separate if check, because it's a loop and we don't want to be checking it constantly.
            if (NPC.AnyNPCs(NPC.type))
                return 0f;

            // Keep this as a separate if check, because it's a loop and we don't want to be checking it constantly.
            if (NPC.AnyNPCs(ModContent.NPCType<PerforatorHive>()))
                return 0f;

            if (NPC.downedBoss2 && !DownedBossSystem.downedPerforator)
                return 1.5f;

            return Main.hardMode ? 0.05f : 0.5f;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment) => NPC.lifeMax = 2000;

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
                }
                if (Main.netMode != NetmodeID.MultiplayerClient && NPC.CountNPCS(ModContent.NPCType<PerforatorHive>()) < 1)
                {
                    Vector2 spawnAt = NPC.Center + new Vector2(0f, (float)NPC.height / 2f);
                    NPC.NewNPC(NPC.GetSource_Death(), (int)spawnAt.X, (int)spawnAt.Y, ModContent.NPCType<PerforatorHive>());
                }
            }
        }
    }
}
