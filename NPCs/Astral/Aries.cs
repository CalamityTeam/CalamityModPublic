using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Astral
{
    public class Aries : ModNPC
    {
        private static Texture2D glowmask;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aries");
            Main.npcFrameCount[npc.type] = 8;
            if (!Main.dedServ)
                glowmask = ModContent.GetTexture("CalamityMod/NPCs/Astral/AriesGlow");
        }

        public override void SetDefaults()
        {
            npc.damage = 50;
            npc.width = 66;
            npc.height = 64;
            npc.aiStyle = 41;
            npc.defense = 14;
            npc.DR_NERD(0.15f);
            npc.lifeMax = 300;
            npc.knockBackResist = 0.6f;
            npc.value = Item.buyPrice(0, 0, 10, 0);
            npc.DeathSound = mod.GetLegacySoundSlot(SoundType.NPCKilled, "Sounds/NPCKilled/AstralEnemyDeath");
            banner = npc.type;
            bannerItem = ModContent.ItemType<AriesBanner>();
            if (CalamityWorld.downedAstrageldon)
            {
                npc.damage = 85;
                npc.defense = 24;
                npc.knockBackResist = 0.5f;
                npc.lifeMax = 450;
            }
            npc.Calamity().VulnerableToHeat = true;
            npc.Calamity().VulnerableToSickness = false;
        }

        public override void FindFrame(int frameHeight)
        {
            CalamityGlobalNPC.SpawnDustOnNPC(npc, 66, frameHeight, ModContent.DustType<AstralOrange>(), new Rectangle(44, 18, 12, 12));
            if (npc.velocity.Y == 0f)
            {
                npc.frame.Y = 0;
            }
            else if ((double)npc.velocity.Y < -1.5)
            {
                npc.frame.Y = frameHeight * 7;
            }
            else if ((double)npc.velocity.Y < 0)
            {
                npc.frame.Y = frameHeight * 4;
            }
            else if ((double)npc.velocity.Y > 1.5)
            {
                npc.frame.Y = frameHeight * 6;
            }
            else
            {
                npc.frame.Y = frameHeight * 5;
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

            CalamityGlobalNPC.DoHitDust(npc, hitDirection, ModContent.DustType<AstralOrange>(), 1f, 4, 24);
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            //draw glowmask
            spriteBatch.Draw(glowmask, npc.Center - Main.screenPosition, npc.frame, Color.White * 0.6f, npc.rotation, new Vector2(33, 31), 1f, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (CalamityGlobalNPC.AnyEvents(spawnInfo.player))
            {
                return 0f;
            }
            else if (spawnInfo.player.InAstral(1))
            {
                return 0.15f;
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
            DropHelper.DropItemCondition(npc, ModContent.ItemType<StellarKnife>(), CalamityWorld.downedAstrageldon, 7, 1, 1);
        }
    }
}
