using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.SunkenSea
{
    public class EutrophicRay : ModNPC
    {
        public bool hasBeenHit = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eutrophic Ray");
            Main.npcFrameCount[npc.type] = 5;
        }

        public override void SetDefaults()
        {
            npc.damage = 20;
            npc.width = 116;
            npc.height = 36;
            npc.defense = Main.hardMode ? 15 : 5;
            npc.DR_NERD(0.05f);
            npc.lifeMax = Main.hardMode ? 500 : 150;
            npc.aiStyle = -1;
            aiType = -1;
            npc.value = Main.hardMode ? Item.buyPrice(0, 0, 50, 0) : Item.buyPrice(0, 0, 5, 0);
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath55;
            npc.knockBackResist = 0f;
            banner = npc.type;
            bannerItem = ModContent.ItemType<EutrophicRayBanner>();
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToSickness = true;
            npc.Calamity().VulnerableToElectricity = true;
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
            npc.TargetClosest(true);
            if (npc.velocity.X > 0.25f)
            {
                npc.spriteDirection = 1;
            }
            else if (npc.velocity.X < 0.25f)
            {
                npc.spriteDirection = -1;
            }
            if (npc.justHit && !hasBeenHit)
            {
                hasBeenHit = true;
                npc.damage = Main.expertMode ? 40 : 20;
                if (Main.hardMode)
                {
                    npc.damage = Main.expertMode ? 100 : 50;
                }
                npc.noTileCollide = true;
                npc.noGravity = true;
                if (npc.Center.X < Main.player[npc.target].Center.X)
                {
                    npc.ai[0] = 1f;
                }
                else
                {
                    npc.ai[0] = 2f;
                }
            }
            npc.chaseable = hasBeenHit;
            if (hasBeenHit)
            {
                float AccelerationY = Main.hardMode ? 0.4f : 0.2f;
                float MaxSpeedY = Main.hardMode ? 4f : 2.5f;
                float Rotation = 0;
                if ((npc.Center.Y + 0.4f) > Main.player[npc.target].Center.Y)
                {
                    npc.velocity.Y -= AccelerationY;
                    if (npc.velocity.Y < -MaxSpeedY)
                    {
                        npc.velocity.Y = -MaxSpeedY;
                    }
                }
                else if ((npc.Center.Y - 0.4f) < Main.player[npc.target].Center.Y)
                {
                    npc.velocity.Y += AccelerationY;
                    if (npc.velocity.Y > MaxSpeedY)
                    {
                        npc.velocity.Y = MaxSpeedY;
                    }
                }
                float AccelerationX = Main.hardMode ? 0.4f : 0.25f;
                float MaxSpeedX = Main.hardMode ? 6f : 4f;

                if (npc.ai[0] == 1f)
                {
                    Rotation = -0.05f;
                    npc.velocity.X -= AccelerationX;
                    if (npc.velocity.X < -MaxSpeedX)
                    {
                        npc.velocity.X = -MaxSpeedX;
                    }

                    if ((npc.Center.X + 300f) < Main.player[npc.target].Center.X)
                    {
                        npc.ai[0] = 2f;
                    }
                }
                else if (npc.ai[0] == 2f)
                {
                    Rotation = 0.05f;
                    npc.velocity.X += AccelerationX;
                    if (npc.velocity.X > MaxSpeedX)
                    {
                        npc.velocity.X = MaxSpeedX;
                    }

                    if ((npc.Center.X - 300f) > Main.player[npc.target].Center.X)
                    {
                        npc.ai[0] = 1f;
                    }
                }

                npc.rotation = npc.velocity.Y * Rotation;
                if (npc.rotation < -0.1f)
                {
                    npc.rotation = -0.1f;
                }
                if (npc.rotation > 0.1f)
                {
                    npc.rotation = 0.1f;
                    return;
                }
            }
            else
            {
                npc.damage = 0;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += hasBeenHit ? 0.15f : 0f;
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
            vector -= new Vector2((float)ModContent.GetTexture("CalamityMod/NPCs/SunkenSea/EutrophicRayGlow").Width, (float)(ModContent.GetTexture("CalamityMod/NPCs/SunkenSea/EutrophicRayGlow").Height / Main.npcFrameCount[npc.type])) * 1f / 2f;
            vector += vector11 * 1f + new Vector2(0f, 4f + npc.gfxOffY);
            Color color = new Color(127 - npc.alpha, 127 - npc.alpha, 127 - npc.alpha, 0).MultiplyRGBA(Microsoft.Xna.Framework.Color.LightBlue);
            Main.spriteBatch.Draw(ModContent.GetTexture("CalamityMod/NPCs/SunkenSea/EutrophicRayGlow"), vector,
                new Microsoft.Xna.Framework.Rectangle?(npc.frame), color, npc.rotation, vector11, 1f, spriteEffects, 0f);
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (projectile.minion && !projectile.Calamity().overridesMinionDamagePrevention)
            {
                return hasBeenHit;
            }
            return null;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.player.Calamity().ZoneSunkenSea && spawnInfo.water && !spawnInfo.player.Calamity().clamity)
            {
                return SpawnCondition.CaveJellyfish.Chance * 0.6f;
            }
            return 0f;
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemCondition(npc, ModContent.ItemType<EutrophicShank>(), CalamityWorld.downedDesertScourge, 3, 1, 1);
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
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/EutrophicRay/RayGore1"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/EutrophicRay/RayGore2"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/EutrophicRay/RayGore3"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/EutrophicRay/RayGore4"), 1f);
            }
        }
    }
}
