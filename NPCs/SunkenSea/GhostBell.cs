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
            Main.npcFrameCount[NPC.type] = 6;
        }

        public override void SetDefaults()
        {
            NPC.noGravity = true;
            NPC.aiStyle = -1;
            aiType = -1;
            NPC.damage = Main.hardMode ? 75 : 25;
            NPC.width = 54;
            NPC.height = 76;
            NPC.defense = Main.hardMode ? 10 : 0;
            NPC.lifeMax = Main.hardMode ? 400 : 120;
            NPC.knockBackResist = 0f;
            NPC.alpha = 100;
            NPC.value = Main.hardMode ? Item.buyPrice(0, 0, 20, 0) : Item.buyPrice(0, 0, 2, 0);
            NPC.HitSound = SoundID.NPCHit25;
            NPC.DeathSound = SoundID.NPCDeath28;
            banner = NPC.type;
            bannerItem = ModContent.ItemType<GhostBellBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = true;
            NPC.Calamity().VulnerableToElectricity = false;
            NPC.Calamity().VulnerableToWater = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.chaseable);
            writer.Write(hasBeenHit);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.chaseable = reader.ReadBoolean();
            hasBeenHit = reader.ReadBoolean();
        }

        public override void AI()
        {
            Lighting.AddLight(NPC.Center, 0f, (255 - NPC.alpha) * 1.5f / 255f, (255 - NPC.alpha) * 1.5f / 255f);
            if (NPC.justHit)
            {
                hasBeenHit = true;
            }
            NPC.chaseable = hasBeenHit;
            if (NPC.localAI[0] == 0f)
            {
                NPC.localAI[0] = 1f;
                NPC.velocity.Y = -6f;
                NPC.netUpdate = true;
            }
            if (NPC.wet)
            {
                NPC.noGravity = true;
                if (NPC.localAI[2] > 0f)
                {
                    NPC.localAI[2] -= 1f;
                }
                if (NPC.localAI[2] <= 0f)
                {
                    if (NPC.velocity.Y == 0f)
                    {
                        NPC.localAI[1] += 1f;
                    }
                    else
                    {
                        NPC.localAI[1] = 0f;
                    }
                    NPC.velocity.Y += 0.1f;
                    if (NPC.velocity.Y > 3f || NPC.localAI[1] >= 6f)
                    {
                        NPC.velocity.Y = -3f;
                    }
                }
            }
            else
            {
                NPC.noGravity = false;
                NPC.velocity.Y = 2f;
                NPC.localAI[2] = 75f;
                NPC.netUpdate = true;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.1f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.Calamity().ZoneSunkenSea && spawnInfo.water && !spawnInfo.Player.Calamity().clamity)
            {
                return SpawnCondition.CaveJellyfish.Chance * 0.9f;
            }
            return 0f;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Vector2 center = new Vector2(NPC.Center.X, NPC.Center.Y);
            Vector2 vector11 = new Vector2((float)(Main.npcTexture[NPC.type].Width / 2), (float)(Main.npcTexture[NPC.type].Height / Main.npcFrameCount[NPC.type] / 2));
            Vector2 vector = center - Main.screenPosition;
            vector -= new Vector2((float)ModContent.Request<Texture2D>("CalamityMod/NPCs/SunkenSea/GhostBellGlow").Width, (float)(ModContent.Request<Texture2D>("CalamityMod/NPCs/SunkenSea/GhostBellGlow").Height / Main.npcFrameCount[NPC.type])) * 1f / 2f;
            vector += vector11 * 1f + new Vector2(0f, 4f + NPC.gfxOffY);
            Color color = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Microsoft.Xna.Framework.Color.LightBlue);
            Main.spriteBatch.Draw(ModContent.Request<Texture2D>("CalamityMod/NPCs/SunkenSea/GhostBellGlow"), vector,
                new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, vector11, 1f, spriteEffects, 0f);
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Electrified, 120, true);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 68, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 25; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 68, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemCondition(NPC, ModContent.ItemType<VoltaicJelly>(), CalamityWorld.downedDesertScourge, 0.2f);
            DropHelper.DropItemChance(NPC, ItemID.JellyfishNecklace, 0.01f);
        }
    }
}
