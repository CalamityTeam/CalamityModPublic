using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Astral
{
    public class SmallSightseer : ModNPC
    {
        private static Texture2D glowmask;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Small Sightseer");
            Main.npcFrameCount[npc.type] = 4;

            if (!Main.dedServ)
                glowmask = ModContent.GetTexture("CalamityMod/NPCs/Astral/SmallSightseerGlow");
        }

        public override void SetDefaults()
        {
            npc.width = 48;
            npc.height = 40;
            npc.damage = 38;
            npc.defense = 16;
            npc.DR_NERD(0.15f);
            npc.lifeMax = 310;
            npc.DeathSound = mod.GetLegacySoundSlot(SoundType.NPCKilled, "Sounds/NPCKilled/AstralEnemyDeath");
            npc.noGravity = true;
            npc.knockBackResist = 0.58f;
            npc.value = Item.buyPrice(0, 0, 10, 0);
            npc.aiStyle = -1;
            banner = npc.type;
            bannerItem = ModContent.ItemType<SmallSightseerBanner>();
            if (CalamityWorld.downedAstrageldon)
            {
                npc.damage = 58;
                npc.defense = 26;
                npc.knockBackResist = 0.48f;
                npc.lifeMax = 460;
            }
            npc.Calamity().VulnerableToHeat = true;
            npc.Calamity().VulnerableToSickness = false;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.05f + npc.velocity.Length() * 0.667f;
            if (npc.frameCounter >= 8)
            {
                npc.frameCounter = 0;
                npc.frame.Y += frameHeight;
                if (npc.frame.Y > npc.height * 2)
                {
                    npc.frame.Y = 0;
                }
            }

            //DO DUST
            Dust d = CalamityGlobalNPC.SpawnDustOnNPC(npc, 80, frameHeight, ModContent.DustType<AstralOrange>(), new Rectangle(16, 8, 6, 6), Vector2.Zero, 0.45f, true);
            if (d != null)
            {
                d.customData = 0.04f;
            }
        }

        public override void AI()
        {
            CalamityGlobalNPC.DoFlyingAI(npc, (CalamityWorld.death ? 8.7f : 5.8f), (CalamityWorld.death ? 0.045f : 0.03f), 350f);
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
                for (int i = 0; i < 5; i++)
                {
                    float rand = Main.rand.NextFloat(-0.18f, 0.18f);
                    Gore.NewGore(npc.position + new Vector2(Main.rand.NextFloat(0f, npc.width), Main.rand.NextFloat(0f, npc.height)), npc.velocity * rand, mod.GetGoreSlot("Gores/SmallSightseer/SmallSightseerGore" + i));
                }
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            spriteBatch.Draw(glowmask, npc.Center - Main.screenPosition + new Vector2(0, 4f), new Rectangle(0, npc.frame.Y, 80, npc.frame.Height), Color.White * 0.75f, npc.rotation, new Vector2(40f, 20f), npc.scale, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (CalamityGlobalNPC.AnyEvents(spawnInfo.player))
            {
                return 0f;
            }
            else if (spawnInfo.player.InAstral(1))
            {
                return spawnInfo.player.ZoneDesert ? 0.16f : 0.2f;
            }
            return 0f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 60, true);
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemChance(npc, ModContent.ItemType<Stardust>(), 0.5f, 1, 2);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<Stardust>(), Main.expertMode);
        }
    }
}
