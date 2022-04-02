using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
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
    public class FusionFeeder : ModNPC
    {
        private static Texture2D glowmask;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fusion Feeder");
            if (!Main.dedServ)
                glowmask = ModContent.GetTexture("CalamityMod/NPCs/Astral/FusionFeederGlow");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.noGravity = true;
            npc.width = 120;
            npc.height = 24;
            npc.damage = 45;
            npc.aiStyle = 103;
            npc.lifeMax = 400;
            npc.defense = 12;
            npc.DR_NERD(0.15f);
            npc.value = Item.buyPrice(0, 0, 20, 0);
            npc.knockBackResist = 0.8f;
            npc.behindTiles = true;
            npc.DeathSound = mod.GetLegacySoundSlot(SoundType.NPCKilled, "Sounds/NPCKilled/AstralEnemyDeath");
            animationType = NPCID.SandShark;
            banner = npc.type;
            bannerItem = ModContent.ItemType<FusionFeederBanner>();
            if (CalamityWorld.downedAstrageldon)
            {
                npc.damage = 65;
                npc.defense = 22;
                npc.knockBackResist = 0.7f;
                npc.lifeMax = 600;
            }
            npc.Calamity().VulnerableToHeat = true;
            npc.Calamity().VulnerableToSickness = false;
        }

        public override void FindFrame(int frameHeight)
        {
            //DO DUST
            Dust d = CalamityGlobalNPC.SpawnDustOnNPC(npc, 134, frameHeight, ModContent.DustType<AstralOrange>(), new Rectangle(46, 4, 60, 6), Vector2.Zero, 0.55f, true);
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

            CalamityGlobalNPC.DoHitDust(npc, hitDirection, (Main.rand.Next(0, Math.Max(0, npc.life)) == 0) ? 5 : ModContent.DustType<AstralEnemy>(), 1f, 4, 25);

            //if dead do gores
            if (npc.life <= 0)
            {
                for (int i = 0; i < 6; i++)
                {
                    float rand = Main.rand.NextFloat(-0.18f, 0.18f);
                    Gore.NewGore(npc.position + new Vector2(Main.rand.NextFloat(0f, npc.width), Main.rand.NextFloat(0f, npc.height)), npc.velocity * rand, mod.GetGoreSlot("Gores/FusionFeeder/FusionFeederGore" + i));
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Vector2 offset = new Vector2(0f, 10f);
            Vector2 origin = new Vector2(67f, 23f);

            //draw shark
            spriteBatch.Draw(Main.npcTexture[npc.type], npc.Center - Main.screenPosition + offset, npc.frame, drawColor, npc.rotation, origin, 1f, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);

            //draw glowmask
            spriteBatch.Draw(glowmask, npc.Center - Main.screenPosition + offset, npc.frame, Color.White * 0.6f, npc.rotation, origin, 1f, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);

            return false;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (CalamityGlobalNPC.AnyEvents(spawnInfo.player))
            {
                return 0f;
            }
            else if (spawnInfo.player.InAstral(3))
            {
                return 0.14f;
            }
            return 0f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120, true);
        }

        public override void NPCLoot()
        {
            DropHelper.DropItem(npc, ModContent.ItemType<Stardust>(), 2, 3);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<Stardust>(), Main.expertMode);
        }
    }
}
