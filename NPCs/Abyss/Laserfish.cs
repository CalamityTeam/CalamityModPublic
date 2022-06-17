using CalamityMod.BiomeManagers;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace CalamityMod.NPCs.Abyss
{
    public class Laserfish : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Laserfish");
            Main.npcFrameCount[NPC.type] = 6;
        }

        public override void SetDefaults()
        {
            NPC.noGravity = true;
            NPC.damage = 40;
            NPC.width = 60;
            NPC.height = 26;
            NPC.defense = 15;
            NPC.lifeMax = 480;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.value = Item.buyPrice(0, 0, 10, 0);
            NPC.HitSound = SoundID.NPCHit51;
            NPC.DeathSound = SoundID.NPCDeath26;
            NPC.knockBackResist = 0.65f;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<LaserfishBanner>();
            NPC.chaseable = false;
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = true;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
            SpawnModBiomes = new int[2] { ModContent.GetInstance<AbyssLayer2Biome>().Type, ModContent.GetInstance<AbyssLayer3Biome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {

				// Will move to localization whenever that is cleaned up.
				new FlavorTextBestiaryInfoElement("Its eyes are blinded by bacteria that gather in large sacs on its head. As it swims through the waters, relying on primitive echolocation, the bacteria are able to release concussive blasts to clear its path.")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.chaseable);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.chaseable = reader.ReadBoolean();
        }

        public override void AI()
        {
            // Setting this in SetDefaults will disable expert mode scaling, so put it here instead
            NPC.damage = 0;

            CalamityAI.PassiveSwimmingAI(NPC, Mod, 0, Main.player[NPC.target].Calamity().GetAbyssAggro(160f), 0.15f, 0.15f, 4f, 4f, 0.1f);
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (projectile.minion && !projectile.Calamity().overridesMinionDamagePrevention)
            {
                return NPC.chaseable;
            }
            return null;
        }

        public override void FindFrame(int frameHeight)
        {
            if (!NPC.wet && !NPC.IsABestiaryIconDummy)
            {
                NPC.frameCounter = 0.0;
                return;
            }
            NPC.frameCounter += NPC.chaseable ? 0.15f : 0.075f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Vector2 center = new Vector2(NPC.Center.X, NPC.Center.Y);
            Vector2 vector11 = new Vector2((float)(TextureAssets.Npc[NPC.type].Value.Width / 2), (float)(TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2));
            Vector2 vector = center - screenPos;
            vector -= new Vector2((float)ModContent.Request<Texture2D>("CalamityMod/NPCs/Abyss/LaserfishGlow").Value.Width, (float)(ModContent.Request<Texture2D>("CalamityMod/NPCs/Abyss/LaserfishGlow").Value.Height / Main.npcFrameCount[NPC.type])) * 1f / 2f;
            vector += vector11 * 1f + new Vector2(0f, 4f + NPC.gfxOffY);
            Color color = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.Yellow);
            Main.spriteBatch.Draw(ModContent.Request<Texture2D>("CalamityMod/NPCs/Abyss/LaserfishGlow").Value, vector,
                new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, vector11, 1f, spriteEffects, 0f);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.Calamity().ZoneAbyssLayer2 && spawnInfo.Water)
            {
                return SpawnCondition.CaveJellyfish.Chance * 0.6f;
            }
            if (spawnInfo.Player.Calamity().ZoneAbyssLayer3 && spawnInfo.Water)
            {
                return SpawnCondition.CaveJellyfish.Chance * 1.2f;
            }
            return 0f;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.AddIf(() => DownedBossSystem.downedCalamitas, ModContent.ItemType<Lumenyl>(), 2);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 25; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Laserfish").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Laserfish2").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Laserfish3").Type, 1f);
                }
            }
        }
    }
}
