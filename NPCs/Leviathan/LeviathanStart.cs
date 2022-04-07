using CalamityMod.CalPlayer;
using CalamityMod.Items.Accessories;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
namespace CalamityMod.NPCs.Leviathan
{
    public class LeviathanStart : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("???");
            Main.npcFrameCount[NPC.type] = 6;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.damage = 0;
            NPC.width = 100;
            NPC.height = 100;
            NPC.defense = 0;
            NPC.lifeMax = 1000;
            NPC.knockBackResist = 0f;
            NPC.Opacity = 0f;
            NPC.noGravity = true;
            NPC.dontTakeDamage = true;
            NPC.chaseable = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = null;
            NPC.rarity = 2;
            Music = CalamityMod.Instance.GetMusicFromMusicMod("AnahitaPreboss") ?? -1;
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = true;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.dontTakeDamage);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.dontTakeDamage = reader.ReadBoolean();
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.1f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            NPC.TargetClosest(true);

            float playerLocation = NPC.Center.X - Main.player[NPC.target].Center.X;
            NPC.direction = playerLocation < 0f ? 1 : -1;
            NPC.spriteDirection = NPC.direction;

            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) < 560f)
            {
                if (NPC.ai[0] < 90f)
                    NPC.ai[0] += 1f;
            }
            else if (NPC.ai[0] > 0f)
                NPC.ai[0] -= 1f;

            NPC.dontTakeDamage = NPC.ai[0] != 90f;

            NPC.Opacity = MathHelper.Clamp(NPC.ai[0] / 90f, 0f, 1f);

            Lighting.AddLight((int)NPC.Center.X / 16, (int)NPC.Center.Y / 16, 0f, 0f, 0.8f * NPC.Opacity);

            if (CalamityPlayer.areThereAnyDamnBosses)
                NPC.active = false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D drawTex = TextureAssets.Npc[NPC.type].Value;
            Vector2 origin = new Vector2(drawTex.Width / 2, drawTex.Height / 2);

            Vector2 drawPos = NPC.Center - screenPos;
            drawPos -= new Vector2(drawTex.Width, drawTex.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
            drawPos += origin * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(drawTex, drawPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, spriteEffects, 0f);

            drawTex = ModContent.Request<Texture2D>("CalamityMod/NPCs/Leviathan/LeviathanStartGlow").Value;

            spriteBatch.Draw(drawTex, drawPos, NPC.frame, Color.White, NPC.rotation, origin, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.Calamity().disableAnahitaSpawns)
                return 0f;

            if (spawnInfo.PlayerSafe ||
                NPC.AnyNPCs(NPCID.DukeFishron) ||
                NPC.AnyNPCs(NPC.type) ||
                NPC.AnyNPCs(ModContent.NPCType<Siren>()) ||
                NPC.AnyNPCs(ModContent.NPCType<Leviathan>()) ||
                spawnInfo.Player.Calamity().ZoneSulphur ||
                NPC.LunarApocalypseIsUp)
            {
                return 0f;
            }

            if (!Main.hardMode)
                return SpawnCondition.OceanMonster.Chance * 0.025f;

            if (!NPC.downedPlantBoss && !DownedBossSystem.downedCalamitas)
                return SpawnCondition.OceanMonster.Chance * 0.1f;

            return SpawnCondition.OceanMonster.Chance * 0.4f;
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemChance(NPC, ModContent.ItemType<SirensHeart>(), 0.25f);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life > 0)
            {
                for (int k = 0; k < 5; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                }
            }
            else if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int siren = NPC.NewNPC((int)NPC.Center.X, (int)NPC.position.Y + NPC.height, ModContent.NPCType<Siren>(), NPC.whoAmI);
                CalamityUtils.BossAwakenMessage(siren);
            }
        }
    }
}
