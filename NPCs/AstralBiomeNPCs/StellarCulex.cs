using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.AstralBiomeNPCs
{
    public class StellarCulex : ModNPC
    {
        private static Texture2D glowmask;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stellar Culex");
            if (!Main.dedServ)
                glowmask = mod.GetTexture("NPCs/AstralBiomeNPCs/StellarCulexGlow");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.width = 60;
            npc.height = 50;
            npc.aiStyle = 14; //bats
            npc.npcSlots = 0.5f; //needed?
            npc.damage = 90;
            npc.defense = 28;
            npc.knockBackResist = 0.55f;
            npc.lifeMax = 320;
            npc.value = Item.buyPrice(0, 0, 10, 0);
            npc.DeathSound = mod.GetLegacySoundSlot(SoundType.NPCKilled, "Sounds/NPCKilled/AstralEnemyDeath");
            npc.buffImmune[31] = false;
            animationType = NPCID.GiantFlyingFox;
			banner = npc.type;
			bannerItem = mod.ItemType("StellarCulexBanner");
		}

        public override void FindFrame(int frameHeight)
        {
            //DO DUST
            int frame = npc.frame.Y / frameHeight;
            Dust d = CalamityGlobalNPC.SpawnDustOnNPC(npc, 100, frameHeight, mod.DustType("AstralOrange"), new Rectangle(66, 10, (frame == 0 || frame == 3) ? 32 : 24, 16), Vector2.Zero, 0.45f, true);
            if (d != null)
            {
                d.customData = 0.04f;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.soundDelay == 0)
            {
                npc.soundDelay = 15;
                switch (Main.rand.Next(3))
                {
                    case 0:
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/AstralEnemyHit"), npc.Center);
                        break;
                    case 1:
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/AstralEnemyHit2"), npc.Center);
                        break;
                    case 2:
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/AstralEnemyHit3"), npc.Center);
                        break;
                }
            }

            CalamityGlobalNPC.DoHitDust(npc, hitDirection, (Main.rand.Next(0, Math.Max(0, npc.life)) == 0) ? 5 : mod.DustType("AstralEnemy"), 1f, 4, 22);

            //if dead do gores
            if (npc.life <= 0)
            {
                for (int i = 0; i < 6; i++)
                {
                    Gore.NewGore(npc.Center, npc.velocity * 0.3f, mod.GetGoreSlot("Gores/StellarCulex/StellarCulexGore" + i));
                }
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Vector2 origin = new Vector2(50, 30f);
            spriteBatch.Draw(glowmask, npc.Center - Main.screenPosition, npc.frame, Color.White * 0.6f, npc.rotation, origin, 1f, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.player.GetModPlayer<CalamityPlayer>().ZoneAstral && spawnInfo.player.ZoneRockLayerHeight)
            {
                return 0.16f;
            }
            return 0f;
        }

        public override void NPCLoot()
        {
            if (Main.rand.Next(2) == 0)
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Stardust"), Main.rand.Next(1, 3));
            }
            if (Main.expertMode)
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Stardust"));
            }
        }
    }
}
