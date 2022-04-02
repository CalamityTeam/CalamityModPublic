using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Abyss
{
	public class MirageJelly : ModNPC
    {
        private bool teleporting = false;
        private bool rephasing = false;
        private bool hasBeenHit = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mirage Jelly");
            Main.npcFrameCount[npc.type] = 7;
        }

        public override void SetDefaults()
        {
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.damage = 100;
            npc.width = 70;
            npc.height = 162;
            npc.defense = 10;
            npc.lifeMax = 6000;
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(0, 0, 25, 0);
            npc.HitSound = SoundID.NPCHit25;
            npc.DeathSound = SoundID.NPCDeath28;
            banner = npc.type;
            bannerItem = ModContent.ItemType<MirageJellyBanner>();
			npc.Calamity().VulnerableToHeat = false;
			npc.Calamity().VulnerableToSickness = false;
			npc.Calamity().VulnerableToElectricity = false;
			npc.Calamity().VulnerableToWater = false;
		}

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(hasBeenHit);
            writer.Write(teleporting);
            writer.Write(rephasing);
            writer.Write(npc.chaseable);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            hasBeenHit = reader.ReadBoolean();
            teleporting = reader.ReadBoolean();
            rephasing = reader.ReadBoolean();
            npc.chaseable = reader.ReadBoolean();
        }

        public override void AI()
        {
            npc.TargetClosest(true);
            Player player = Main.player[npc.target];
            npc.velocity *= 0.985f;
            if (npc.velocity.Y > -0.3f)
            {
                npc.velocity.Y = -3f;
            }
            if (npc.justHit)
            {
                if (Main.rand.NextBool(10))
                {
                    teleporting = true;
                }
                hasBeenHit = true;
            }
            if (npc.ai[0] == 0f)
            {
                npc.chaseable = true;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (teleporting)
                    {
                        teleporting = false;
                        npc.TargetClosest(true);
                        int num1249 = 0;
                        int num1250;
                        int num1251;
                        while (true)
                        {
                            num1249++;
                            num1250 = (int)player.Center.X / 16;
                            num1251 = (int)player.Center.Y / 16;

                            int min = 6;
                            int max = 9;

                            if (Main.rand.NextBool(2))
                                num1250 += Main.rand.Next(min, max);
                            else
                                num1250 -= Main.rand.Next(min, max);

							min = 11;
							max = 26;

							num1251 += Main.rand.Next(min, max);

                            if (!WorldGen.SolidTile(num1250, num1251) && Collision.CanHit(new Vector2((float)(num1250 * 16), (float)(num1251 * 16)), 1, 1, player.position, player.width, player.height) &&
                                Main.tile[num1250, num1251].liquid > 204)
                            {
                                break;
                            }
                            if (num1249 > 100)
                            {
                                goto Block;
                            }
                        }
                        npc.ai[0] = 1f;
                        npc.ai[1] = (float)num1250;
                        npc.ai[2] = (float)num1251;
                        npc.netUpdate = true;
                        Block:
                        ;
                    }
                }
            }
            else if (npc.ai[0] == 1f)
            {
                npc.damage = 0;
                npc.chaseable = false;
                npc.alpha += 5;
                if (npc.alpha >= 255)
                {
                    npc.alpha = 255;
                    npc.position.X = npc.ai[1] * 16f - (float)(npc.width / 2);
                    npc.position.Y = npc.ai[2] * 16f - (float)(npc.height / 2);
                    npc.ai[0] = 2f;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 2f)
            {
                npc.alpha -= 5;
                if (npc.alpha <= 0)
                {
                    npc.damage = Main.expertMode ? 200 : 100;
                    npc.chaseable = true;
                    npc.alpha = 0;
                    npc.ai[0] = 0f;
                    npc.netUpdate = true;
                }
            }
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (projectile.minion && !projectile.Calamity().overridesMinionDamagePrevention)
            {
                return hasBeenHit;
            }
            return null;
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
            vector -= new Vector2((float)ModContent.GetTexture("CalamityMod/NPCs/Abyss/MirageJellyGlow").Width, (float)(ModContent.GetTexture("CalamityMod/NPCs/Abyss/MirageJellyGlow").Height / Main.npcFrameCount[npc.type])) * 1f / 2f;
            vector += vector11 * 1f + new Vector2(0f, 4f + npc.gfxOffY);
            Color color = new Color(127 - npc.alpha, 127 - npc.alpha, 127 - npc.alpha, 0).MultiplyRGBA(Microsoft.Xna.Framework.Color.Purple);
            Main.spriteBatch.Draw(ModContent.GetTexture("CalamityMod/NPCs/Abyss/MirageJellyGlow"), vector,
                new Microsoft.Xna.Framework.Rectangle?(npc.frame), color, npc.rotation, vector11, 1f, spriteEffects, 0f);
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += hasBeenHit ? 0.15f : 0.1f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.player.Calamity().ZoneAbyssLayer3 && spawnInfo.water)
            {
                return SpawnCondition.CaveJellyfish.Chance * 0.6f;
            }
            return 0f;
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemCondition(npc, ModContent.ItemType<AbyssShocker>(), NPC.downedBoss3, 10, 1, 1);
            int minCells = Main.expertMode ? 10 : 5;
            int maxCells = Main.expertMode ? 14 : 7;
            DropHelper.DropItemCondition(npc, ModContent.ItemType<DepthCells>(), CalamityWorld.downedCalamitas, 0.5f, minCells, maxCells);
            DropHelper.DropItemChance(npc, ModContent.ItemType<LifeJelly>(), Main.expertMode ? 5 : 7);
            DropHelper.DropItemChance(npc, ModContent.ItemType<ManaJelly>(), Main.expertMode ? 5 : 7);
            DropHelper.DropItemChance(npc, ModContent.ItemType<VitalJelly>(), Main.expertMode ? 5 : 7);
			DropHelper.DropItemChance(npc, ItemID.JellyfishNecklace, 0.01f);
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Venom, 240, true);
            player.AddBuff(BuffID.Electrified, 60, true);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 15; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
