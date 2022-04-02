using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Accessories;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.SunkenSea
{
    public class GhostBell : ModNPC
    {
        public bool hasBeenHit = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ghost Bell");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {
            npc.noGravity = true;
            npc.aiStyle = -1;
            aiType = -1;
            npc.damage = Main.hardMode ? 75 : 25;
            npc.width = 54;
            npc.height = 76;
            npc.defense = Main.hardMode ? 10 : 0;
            npc.lifeMax = Main.hardMode ? 400 : 120;
            npc.knockBackResist = 0f;
            npc.alpha = 100;
            npc.value = Main.hardMode ? Item.buyPrice(0, 0, 20, 0) : Item.buyPrice(0, 0, 2, 0);
            npc.HitSound = SoundID.NPCHit25;
            npc.DeathSound = SoundID.NPCDeath28;
            banner = npc.type;
            bannerItem = ModContent.ItemType<GhostBellBanner>();
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToSickness = true;
            npc.Calamity().VulnerableToElectricity = false;
            npc.Calamity().VulnerableToWater = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.chaseable);
            writer.Write(hasBeenHit);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.chaseable = reader.ReadBoolean();
            hasBeenHit = reader.ReadBoolean();
        }

        public override void AI()
        {
            Lighting.AddLight(npc.Center, 0f, (255 - npc.alpha) * 1.5f / 255f, (255 - npc.alpha) * 1.5f / 255f);
            if (npc.justHit)
            {
                hasBeenHit = true;
            }
            npc.chaseable = hasBeenHit;
            if (npc.localAI[0] == 0f)
            {
                npc.localAI[0] = 1f;
                npc.velocity.Y = -6f;
                npc.netUpdate = true;
            }
            if (npc.wet)
            {
                npc.noGravity = true;
                if (npc.localAI[2] > 0f)
                {
                    npc.localAI[2] -= 1f;
                }
                if (npc.localAI[2] <= 0f)
                {
                    if (npc.velocity.Y == 0f)
                    {
                        npc.localAI[1] += 1f;
                    }
                    else
                    {
                        npc.localAI[1] = 0f;
                    }
                    npc.velocity.Y += 0.1f;
                    if (npc.velocity.Y > 3f || npc.localAI[1] >= 6f)
                    {
                        npc.velocity.Y = -3f;
                    }
                }
            }
            else
            {
                npc.noGravity = false;
                npc.velocity.Y = 2f;
                npc.localAI[2] = 75f;
                npc.netUpdate = true;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.1f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.player.Calamity().ZoneSunkenSea && spawnInfo.water && !spawnInfo.player.Calamity().clamity)
            {
                return SpawnCondition.CaveJellyfish.Chance * 0.9f;
            }
            return 0f;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (npc.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Vector2 center = new Vector2(npc.Center.X, npc.Center.Y);
            Vector2 vector11 = new Vector2((float)(Main.npcTexture[npc.type].Width / 2), (float)(Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2));
            Vector2 vector = center - Main.screenPosition;
            vector -= new Vector2((float)ModContent.GetTexture("CalamityMod/NPCs/SunkenSea/GhostBellGlow").Width, (float)(ModContent.GetTexture("CalamityMod/NPCs/SunkenSea/GhostBellGlow").Height / Main.npcFrameCount[npc.type])) * 1f / 2f;
            vector += vector11 * 1f + new Vector2(0f, 4f + npc.gfxOffY);
            Color color = new Color(127 - npc.alpha, 127 - npc.alpha, 127 - npc.alpha, 0).MultiplyRGBA(Microsoft.Xna.Framework.Color.LightBlue);
            Main.spriteBatch.Draw(ModContent.GetTexture("CalamityMod/NPCs/SunkenSea/GhostBellGlow"), vector,
                new Microsoft.Xna.Framework.Rectangle?(npc.frame), color, npc.rotation, vector11, 1f, spriteEffects, 0f);
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Electrified, 120, true);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 68, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 25; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 68, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemCondition(npc, ModContent.ItemType<VoltaicJelly>(), CalamityWorld.downedDesertScourge, 0.2f);
            DropHelper.DropItemChance(npc, ItemID.JellyfishNecklace, 0.01f);
        }
    }
}
