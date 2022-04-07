using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

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
            Main.npcFrameCount[NPC.type] = 7;
        }

        public override void SetDefaults()
        {
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.damage = 100;
            NPC.width = 70;
            NPC.height = 162;
            NPC.defense = 10;
            NPC.lifeMax = 6000;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 25, 0);
            NPC.HitSound = SoundID.NPCHit25;
            NPC.DeathSound = SoundID.NPCDeath28;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<MirageJellyBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = false;
            NPC.Calamity().VulnerableToWater = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(hasBeenHit);
            writer.Write(teleporting);
            writer.Write(rephasing);
            writer.Write(NPC.chaseable);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            hasBeenHit = reader.ReadBoolean();
            teleporting = reader.ReadBoolean();
            rephasing = reader.ReadBoolean();
            NPC.chaseable = reader.ReadBoolean();
        }

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];
            NPC.velocity *= 0.985f;
            if (NPC.velocity.Y > -0.3f)
            {
                NPC.velocity.Y = -3f;
            }
            if (NPC.justHit)
            {
                if (Main.rand.NextBool(10))
                {
                    teleporting = true;
                }
                hasBeenHit = true;
            }
            if (NPC.ai[0] == 0f)
            {
                NPC.chaseable = true;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (teleporting)
                    {
                        teleporting = false;
                        NPC.TargetClosest(true);
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
                                Main.tile[num1250, num1251].LiquidAmount > 204)
                            {
                                break;
                            }
                            if (num1249 > 100)
                            {
                                goto Block;
                            }
                        }
                        NPC.ai[0] = 1f;
                        NPC.ai[1] = (float)num1250;
                        NPC.ai[2] = (float)num1251;
                        NPC.netUpdate = true;
                        Block:
                        ;
                    }
                }
            }
            else if (NPC.ai[0] == 1f)
            {
                NPC.damage = 0;
                NPC.chaseable = false;
                NPC.alpha += 5;
                if (NPC.alpha >= 255)
                {
                    NPC.alpha = 255;
                    NPC.position.X = NPC.ai[1] * 16f - (float)(NPC.width / 2);
                    NPC.position.Y = NPC.ai[2] * 16f - (float)(NPC.height / 2);
                    NPC.ai[0] = 2f;
                    NPC.netUpdate = true;
                }
            }
            else if (NPC.ai[0] == 2f)
            {
                NPC.alpha -= 5;
                if (NPC.alpha <= 0)
                {
                    NPC.damage = Main.expertMode ? 200 : 100;
                    NPC.chaseable = true;
                    NPC.alpha = 0;
                    NPC.ai[0] = 0f;
                    NPC.netUpdate = true;
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
            vector -= new Vector2((float)ModContent.Request<Texture2D>("CalamityMod/NPCs/Abyss/MirageJellyGlow").Value.Width, (float)(ModContent.Request<Texture2D>("CalamityMod/NPCs/Abyss/MirageJellyGlow").Value.Height / Main.npcFrameCount[NPC.type])) * 1f / 2f;
            vector += vector11 * 1f + new Vector2(0f, 4f + NPC.gfxOffY);
            Color color = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Microsoft.Xna.Framework.Color.Purple);
            Main.spriteBatch.Draw(ModContent.Request<Texture2D>("CalamityMod/NPCs/Abyss/MirageJellyGlow").Value, vector,
                new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, vector11, 1f, spriteEffects, 0f);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += hasBeenHit ? 0.15f : 0.1f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.Calamity().ZoneAbyssLayer3 && spawnInfo.Water)
            {
                return SpawnCondition.CaveJellyfish.Chance * 0.6f;
            }
            return 0f;
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemCondition(NPC, ModContent.ItemType<AbyssShocker>(), NPC.downedBoss3, 10, 1, 1);
            int minCells = Main.expertMode ? 10 : 5;
            int maxCells = Main.expertMode ? 14 : 7;
            DropHelper.DropItemCondition(NPC, ModContent.ItemType<DepthCells>(), DownedBossSystem.downedCalamitas, 0.5f, minCells, maxCells);
            DropHelper.DropItemChance(NPC, ModContent.ItemType<LifeJelly>(), Main.expertMode ? 5 : 7);
            DropHelper.DropItemChance(NPC, ModContent.ItemType<ManaJelly>(), Main.expertMode ? 5 : 7);
            DropHelper.DropItemChance(NPC, ModContent.ItemType<VitalJelly>(), Main.expertMode ? 5 : 7);
            DropHelper.DropItemChance(NPC, ItemID.JellyfishNecklace, 0.01f);
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
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 15; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
