using CalamityMod.Items.Placeables;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.SunkenSea
{
    public class PrismTurtle : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prism-Back");
            Main.npcFrameCount[npc.type] = 5;
        }

        public override void SetDefaults()
        {
            npc.noGravity = true;
            npc.damage = Main.hardMode ? 40 : 20; //normal damage
            npc.width = 72;
            npc.height = 58;
            npc.defense = Main.hardMode ? 25 : 10;
            npc.DR_NERD(0.25f);
            npc.lifeMax = Main.hardMode ? 1000 : 350;
            npc.aiStyle = -1;
            aiType = -1;
            npc.value = Main.hardMode ? Item.buyPrice(0, 0, 50, 0) : Item.buyPrice(0, 0, 5, 0);
            npc.HitSound = SoundID.NPCHit24;
            npc.DeathSound = SoundID.NPCDeath27;
            npc.knockBackResist = 0.15f;
            banner = npc.type;
            bannerItem = ModContent.ItemType<PrismTurtleBanner>();
            npc.chaseable = false;
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToSickness = true;
            npc.Calamity().VulnerableToElectricity = true;
            npc.Calamity().VulnerableToWater = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.chaseable);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.chaseable = reader.ReadBoolean();
        }

        public override void AI()
        {
            if ((npc.Center.Y + 10f) > Main.player[npc.target].Center.Y)
            {
                if (CalamityWorld.death) //gotta do damage scaling directly
                {
                    npc.damage = Main.hardMode ? 240 : 120;
                }
                else if (CalamityWorld.revenge)
                {
                    npc.damage = Main.hardMode ? 168 : 84;
                }
                else if (Main.expertMode)
                {
                    npc.damage = Main.hardMode ? 160 : 80;
                }
                else
                {
                    npc.damage = Main.hardMode ? 80 : 40;
                }
            }
            else
            {
                if (CalamityWorld.death) //gotta do damage scaling directly
                {
                    npc.damage = Main.hardMode ? 120 : 60;
                }
                else if (CalamityWorld.revenge)
                {
                    npc.damage = Main.hardMode ? 84 : 42;
                }
                else if (Main.expertMode)
                {
                    npc.damage = Main.hardMode ? 80 : 40;
                }
                else
                {
                    npc.damage = Main.hardMode ? 40 : 20;
                }
            }
            Lighting.AddLight(npc.Center, (255 - npc.alpha) * 0f / 255f, (255 - npc.alpha) * 0.75f / 255f, (255 - npc.alpha) * 0.75f / 255f);
            CalamityAI.PassiveSwimmingAI(npc, mod, 2, 0f, 0f, 0f, 0f, 0f, 0.1f);
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += npc.wet ? 0.1f : 0f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
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
            vector -= new Vector2((float)ModContent.GetTexture("CalamityMod/NPCs/SunkenSea/PrismTurtleGlow").Width, (float)(ModContent.GetTexture("CalamityMod/NPCs/SunkenSea/PrismTurtleGlow").Height / Main.npcFrameCount[npc.type])) * 1f / 2f;
            vector += vector11 * 1f + new Vector2(0f, 4f + npc.gfxOffY);
            Color color = new Color(127 - npc.alpha, 127 - npc.alpha, 127 - npc.alpha, 0).MultiplyRGBA(Microsoft.Xna.Framework.Color.Blue);
            Main.spriteBatch.Draw(ModContent.GetTexture("CalamityMod/NPCs/SunkenSea/PrismTurtleGlow"), vector,
                new Microsoft.Xna.Framework.Rectangle?(npc.frame), color, npc.rotation, vector11, 1f, spriteEffects, 0f);
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (projectile.minion && !projectile.Calamity().overridesMinionDamagePrevention)
            {
                return npc.chaseable;
            }
            return null;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.player.Calamity().ZoneSunkenSea && spawnInfo.water && !spawnInfo.player.Calamity().clamity)
            {
                return SpawnCondition.CaveJellyfish.Chance * 0.9f;
            }
            return 0f;
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemCondition(npc, ModContent.ItemType<PrismShard>(), CalamityWorld.downedDesertScourge, 1, 1, 3);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 68, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/PrismTurtle/PrismTurtleGore1"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/PrismTurtle/PrismTurtleGore2"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/PrismTurtle/PrismTurtleGore3"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/PrismTurtle/PrismTurtleGore4"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/PrismTurtle/PrismTurtleGore5"), 1f);
                for (int k = 0; k < 25; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 68, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
