using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Astral
{
    public class StellarCulex : ModNPC
    {
        private static Texture2D glowmask;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stellar Culex");
            if (!Main.dedServ)
                glowmask = ModContent.GetTexture("CalamityMod/NPCs/Astral/StellarCulexGlow");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.width = 60;
            npc.height = 50;
            npc.aiStyle = 14; //bats
            npc.npcSlots = 0.5f; //needed?
            npc.damage = 55;
            npc.defense = 18;
            npc.DR_NERD(0.15f);
            npc.knockBackResist = 0.65f;
            npc.lifeMax = 210;
            npc.value = Item.buyPrice(0, 0, 10, 0);
            npc.DeathSound = mod.GetLegacySoundSlot(SoundType.NPCKilled, "Sounds/NPCKilled/AstralEnemyDeath");
            animationType = NPCID.GiantFlyingFox;
            banner = npc.type;
            bannerItem = ModContent.ItemType<StellarCulexBanner>();
            if (CalamityWorld.downedAstrageldon)
            {
                npc.damage = 90;
                npc.defense = 28;
                npc.knockBackResist = 0.55f;
                npc.lifeMax = 320;
            }
            npc.Calamity().VulnerableToHeat = true;
            npc.Calamity().VulnerableToSickness = false;
        }

        public override void FindFrame(int frameHeight)
        {
            //DO DUST
            int frame = npc.frame.Y / frameHeight;
            Dust d = CalamityGlobalNPC.SpawnDustOnNPC(npc, 100, frameHeight, ModContent.DustType<AstralOrange>(), new Rectangle(66, 10, (frame == 0 || frame == 3) ? 32 : 24, 16), Vector2.Zero, 0.45f, true);
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

            CalamityGlobalNPC.DoHitDust(npc, hitDirection, (Main.rand.Next(0, Math.Max(0, npc.life)) == 0) ? 5 : ModContent.DustType<AstralEnemy>(), 1f, 4, 22);

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
            if (CalamityGlobalNPC.AnyEvents(spawnInfo.player))
            {
                return 0f;
            }
            else if (spawnInfo.player.InAstral(2))
            {
                return 0.16f;
            }
            return 0f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120, true);
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemChance(npc, ModContent.ItemType<Stardust>(), 0.5f, 1, 2);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<Stardust>(), Main.expertMode);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<StarbusterCore>(), CalamityWorld.downedAstrageldon, 7, 1, 1);
        }
    }
}
