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
using Terraria.Audio;

namespace CalamityMod.NPCs.Astral
{
    public class SmallSightseer : ModNPC
    {
        private static Texture2D glowmask;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Small Sightseer");
            Main.npcFrameCount[NPC.type] = 4;

            if (!Main.dedServ)
                glowmask = ModContent.Request<Texture2D>("CalamityMod/NPCs/Astral/SmallSightseerGlow");
        }

        public override void SetDefaults()
        {
            NPC.width = 48;
            NPC.height = 40;
            NPC.damage = 38;
            NPC.defense = 16;
            NPC.DR_NERD(0.15f);
            NPC.lifeMax = 310;
            NPC.DeathSound = Mod.GetLegacySoundSlot(SoundType.NPCKilled, "Sounds/NPCKilled/AstralEnemyDeath");
            NPC.noGravity = true;
            NPC.knockBackResist = 0.58f;
            NPC.value = Item.buyPrice(0, 0, 10, 0);
            NPC.aiStyle = -1;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<SmallSightseerBanner>();
            if (CalamityWorld.downedAstrageldon)
            {
                NPC.damage = 58;
                NPC.defense = 26;
                NPC.knockBackResist = 0.48f;
                NPC.lifeMax = 460;
            }
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.05f + NPC.velocity.Length() * 0.667f;
            if (NPC.frameCounter >= 8)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > NPC.height * 2)
                {
                    NPC.frame.Y = 0;
                }
            }

            //DO DUST
            Dust d = CalamityGlobalNPC.SpawnDustOnNPC(NPC, 80, frameHeight, ModContent.DustType<AstralOrange>(), new Rectangle(16, 8, 6, 6), Vector2.Zero, 0.45f, true);
            if (d != null)
            {
                d.customData = 0.04f;
            }
        }

        public override void AI()
        {
            CalamityGlobalNPC.DoFlyingAI(NPC, (CalamityWorld.death ? 8.7f : 5.8f), (CalamityWorld.death ? 0.045f : 0.03f), 350f);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.soundDelay == 0)
            {
                NPC.soundDelay = 15;
                switch (Main.rand.Next(3))
                {
                    case 0:
                        SoundEngine.PlaySound(Mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/AstralEnemyHit"), NPC.Center);
                        break;
                    case 1:
                        SoundEngine.PlaySound(Mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/AstralEnemyHit2"), NPC.Center);
                        break;
                    case 2:
                        SoundEngine.PlaySound(Mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/AstralEnemyHit3"), NPC.Center);
                        break;
                }
            }

            CalamityGlobalNPC.DoHitDust(NPC, hitDirection, (Main.rand.Next(0, Math.Max(0, NPC.life)) == 0) ? 5 : ModContent.DustType<AstralEnemy>(), 1f, 4, 22);

            //if dead do gores
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 5; i++)
                {
                    float rand = Main.rand.NextFloat(-0.18f, 0.18f);
                    Gore.NewGore(NPC.position + new Vector2(Main.rand.NextFloat(0f, NPC.width), Main.rand.NextFloat(0f, NPC.height)), NPC.velocity * rand, Mod.GetGoreSlot("Gores/SmallSightseer/SmallSightseerGore" + i));
                }
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            spriteBatch.Draw(glowmask, NPC.Center - Main.screenPosition + new Vector2(0, 4f), new Rectangle(0, NPC.frame.Y, 80, NPC.frame.Height), Color.White * 0.75f, NPC.rotation, new Vector2(40f, 20f), NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (CalamityGlobalNPC.AnyEvents(spawnInfo.Player))
            {
                return 0f;
            }
            else if (spawnInfo.Player.InAstral(1))
            {
                return spawnInfo.Player.ZoneDesert ? 0.16f : 0.2f;
            }
            return 0f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 60, true);
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemChance(NPC, ModContent.ItemType<Stardust>(), 0.5f, 1, 2);
            DropHelper.DropItemCondition(NPC, ModContent.ItemType<Stardust>(), Main.expertMode);
        }
    }
}
