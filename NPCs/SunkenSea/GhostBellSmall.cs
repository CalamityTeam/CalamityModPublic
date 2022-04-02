using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Critters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Fishing.SunkenSeaCatches;
namespace CalamityMod.NPCs.SunkenSea
{
    public class GhostBellSmall : ModNPC
    {
        public bool hasBeenHit = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Baby Ghost Bell");
            Main.npcFrameCount[npc.type] = 4;
            Main.npcCatchable[npc.type] = true;
        }

        public override void SetDefaults()
        {
            npc.npcSlots = 0.1f;
            npc.noGravity = true;
            npc.chaseable = false;
            npc.aiStyle = -1;
            aiType = -1;
            npc.damage = 0;
            npc.width = 28;
            npc.height = 36;
            npc.defense = 0;
            npc.lifeMax = 5;
            npc.knockBackResist = 1f;
            npc.alpha = 100;
            npc.HitSound = SoundID.NPCHit25;
            npc.DeathSound = SoundID.NPCDeath28;
            banner = npc.type;
            bannerItem = ModContent.ItemType<GhostBellSmallBanner>();
            npc.catchItem = (short)ModContent.ItemType<BabyGhostBellItem>();
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
            if (npc.localAI[0] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (Main.rand.NextBool(20))
                    npc.catchItem = (short)ModContent.ItemType<RustedJingleBell>();
                npc.localAI[0] = 1f;
                npc.velocity.Y = -3f;
                npc.netUpdate = true;
            }
            if (Main.rand.Next(8) < 1 && npc.catchItem == (short)ModContent.ItemType<RustedJingleBell>())
            {
                int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, 68, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 200, default, 1f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 1.1f;
                Main.dust[dust].velocity.Y += 0.25f;
                Main.dust[dust].noLight = true;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[dust].noGravity = false;
                    Main.dust[dust].scale *= 0.5f;
                }
            }
            Lighting.AddLight(npc.Center, 0f, (255 - npc.alpha) * 1f / 255f, (255 - npc.alpha) * 1f / 255f);
            if (npc.wet)
            {
                npc.noGravity = true;
                if (npc.velocity.Y < 0f)
                {
                    npc.velocity.Y += 0.1f;
                }
                if (npc.velocity.Y > 0f)
                {
                    npc.velocity.Y = 0f;
                }
            }
            else
            {
                npc.noGravity = false;
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

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.player.Calamity().ZoneSunkenSea && spawnInfo.water && !spawnInfo.player.Calamity().clamity)
            {
                return SpawnCondition.CaveJellyfish.Chance * 1.5f;
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
            vector -= new Vector2((float)ModContent.GetTexture("CalamityMod/NPCs/SunkenSea/GhostBellSmallGlow").Width, (float)(ModContent.GetTexture("CalamityMod/NPCs/SunkenSea/GhostBellSmallGlow").Height / Main.npcFrameCount[npc.type])) * 1f / 2f;
            vector += vector11 * 1f + new Vector2(0f, 4f + npc.gfxOffY);
            Color color = new Color(127 - npc.alpha, 127 - npc.alpha, 127 - npc.alpha, 0).MultiplyRGBA(Microsoft.Xna.Framework.Color.LightBlue);
            Main.spriteBatch.Draw(ModContent.GetTexture("CalamityMod/NPCs/SunkenSea/GhostBellSmallGlow"), vector,
                new Microsoft.Xna.Framework.Rectangle?(npc.frame), color, npc.rotation, vector11, 1f, spriteEffects, 0f);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 2; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 68, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 68, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override void OnCatchNPC(Player player, Item item)
        {
            try
            {
            } catch
            {
                return;
            }
        }
    }
}
