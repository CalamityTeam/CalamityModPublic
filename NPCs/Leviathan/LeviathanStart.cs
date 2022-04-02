using CalamityMod.CalPlayer;
using CalamityMod.Items.Accessories;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.Leviathan
{
    public class LeviathanStart : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("???");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            aiType = -1;
            npc.damage = 0;
            npc.width = 100;
            npc.height = 100;
            npc.defense = 0;
            npc.lifeMax = 1000;
            npc.knockBackResist = 0f;
            npc.Opacity = 0f;
            npc.noGravity = true;
            npc.dontTakeDamage = true;
            npc.chaseable = false;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = null;
            npc.rarity = 2;
            music = CalamityMod.Instance.GetMusicFromMusicMod("AnahitaPreboss") ?? -1;
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToSickness = true;
            npc.Calamity().VulnerableToElectricity = true;
            npc.Calamity().VulnerableToWater = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.dontTakeDamage);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.dontTakeDamage = reader.ReadBoolean();
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.1f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            npc.TargetClosest(true);

            float playerLocation = npc.Center.X - Main.player[npc.target].Center.X;
            npc.direction = playerLocation < 0f ? 1 : -1;
            npc.spriteDirection = npc.direction;

            if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) < 560f)
            {
                if (npc.ai[0] < 90f)
                    npc.ai[0] += 1f;
            }
            else if (npc.ai[0] > 0f)
                npc.ai[0] -= 1f;

            npc.dontTakeDamage = npc.ai[0] != 90f;

            npc.Opacity = MathHelper.Clamp(npc.ai[0] / 90f, 0f, 1f);

            Lighting.AddLight((int)npc.Center.X / 16, (int)npc.Center.Y / 16, 0f, 0f, 0.8f * npc.Opacity);

            if (CalamityPlayer.areThereAnyDamnBosses)
                npc.active = false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (npc.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D drawTex = Main.npcTexture[npc.type];
            Vector2 origin = new Vector2(drawTex.Width / 2, drawTex.Height / 2);

            Vector2 drawPos = npc.Center - Main.screenPosition;
            drawPos -= new Vector2(drawTex.Width, drawTex.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
            drawPos += origin * npc.scale + new Vector2(0f, npc.gfxOffY);
            spriteBatch.Draw(drawTex, drawPos, npc.frame, npc.GetAlpha(lightColor), npc.rotation, origin, npc.scale, spriteEffects, 0f);

            drawTex = ModContent.GetTexture("CalamityMod/NPCs/Leviathan/LeviathanStartGlow");

            spriteBatch.Draw(drawTex, drawPos, npc.frame, Color.White, npc.rotation, origin, npc.scale, spriteEffects, 0f);

            return false;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.player.Calamity().disableAnahitaSpawns)
                return 0f;

            if (spawnInfo.playerSafe ||
                NPC.AnyNPCs(NPCID.DukeFishron) ||
                NPC.AnyNPCs(npc.type) ||
                NPC.AnyNPCs(ModContent.NPCType<Siren>()) ||
                NPC.AnyNPCs(ModContent.NPCType<Leviathan>()) ||
                spawnInfo.player.Calamity().ZoneSulphur ||
                NPC.LunarApocalypseIsUp)
            {
                return 0f;
            }

            if (!Main.hardMode)
                return SpawnCondition.OceanMonster.Chance * 0.025f;

            if (!NPC.downedPlantBoss && !CalamityWorld.downedCalamitas)
                return SpawnCondition.OceanMonster.Chance * 0.1f;

            return SpawnCondition.OceanMonster.Chance * 0.4f;
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemChance(npc, ModContent.ItemType<SirensHeart>(), 0.25f);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life > 0)
            {
                for (int k = 0; k < 5; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                }
            }
            else if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int siren = NPC.NewNPC((int)npc.Center.X, (int)npc.position.Y + npc.height, ModContent.NPCType<Siren>(), npc.whoAmI);
                CalamityUtils.BossAwakenMessage(siren);
            }
        }
    }
}
