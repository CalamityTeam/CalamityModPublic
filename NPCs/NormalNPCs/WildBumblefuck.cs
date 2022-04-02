using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class WildBumblefuck : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Draconic Swarmer");
            Main.npcFrameCount[npc.type] = 5;
            NPCID.Sets.TrailingMode[npc.type] = 1;
        }

        public override string Texture => "CalamityMod/NPCs/Bumblebirb/BumbleFolly";

        public override void SetDefaults()
        {
            npc.npcSlots = 1f;
            npc.aiStyle = -1;
            aiType = -1;
            npc.damage = 90;
            npc.width = 120;
            npc.height = 80;
            npc.defense = 20;
            npc.LifeMaxNERB(9375, 11250, 5000); // Old HP - 12000, 15000
            npc.knockBackResist = 0f;
            npc.lavaImmune = true;
            npc.noTileCollide = true;
            npc.noGravity = true;
            npc.canGhostHeal = false;
            npc.HitSound = SoundID.NPCHit51;
            npc.DeathSound = SoundID.NPCDeath46;
            npc.Calamity().VulnerableToHeat = true;
            npc.Calamity().VulnerableToCold = true;
            npc.Calamity().VulnerableToSickness = true;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || !NPC.downedMoonlord || spawnInfo.player.Calamity().ZoneSunkenSea || NPC.AnyNPCs(npc.type))
            {
                return 0f;
            }
            return SpawnCondition.SurfaceJungle.Chance * 0.14f;
        }

        public override void AI()
        {
            CalamityAI.Bumblebirb2AI(npc, mod, false);
        }

        public override bool PreNPCLoot()
        {
            DropHelper.DropItemSpray(npc, ModContent.ItemType<EffulgentFeather>(), 2, 4);
            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += npc.ai[0] == 2.1f ? 1.5 : 1D;
            if (npc.frameCounter > 4D) //iban said the time between frames was 5 so using that as a base
            {
                npc.frameCounter = 0D;
                npc.frame.Y += frameHeight;
            }
            if (npc.frame.Y >= frameHeight * 4)
            {
                npc.frame.Y = 0;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (npc.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = Main.npcTexture[npc.type];
            Vector2 vector11 = new Vector2(Main.npcTexture[npc.type].Width / 2, Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2);
            Color color36 = Color.Gold;
            float amount9 = 0.5f;
            int num153 = npc.ai[0] == 2.1f ? 7 : 0;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num155 = 1; num155 < num153; num155 += 2)
                {
                    Color color38 = lightColor;
                    color38 = Color.Lerp(color38, color36, amount9);
                    color38 = npc.GetAlpha(color38);
                    color38 *= (num153 - num155) / 15f;
                    Vector2 vector41 = npc.oldPos[num155] + new Vector2(npc.width, npc.height) / 2f - Main.screenPosition;
                    vector41 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
                    vector41 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
                    spriteBatch.Draw(texture2D15, vector41, npc.frame, color38, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
                }
            }

            Vector2 vector43 = npc.Center - Main.screenPosition;
            vector43 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
            vector43 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
            spriteBatch.Draw(texture2D15, vector43, npc.frame, npc.GetAlpha(lightColor), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 244, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 50; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 244, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
