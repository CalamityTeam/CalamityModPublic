using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Astral
{
    public class AstralachneaWall : ModNPC
    {
        private static Texture2D glowmask;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astralachnea");

            Main.npcFrameCount[npc.type] = 4;

            if (!Main.dedServ)
                glowmask = ModContent.GetTexture("CalamityMod/NPCs/Astral/AstralachneaWallGlow");

            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            npc.width = 60;
            npc.height = 60;
            npc.aiStyle = -1;
            npc.damage = 55;
            npc.defense = 20;
            npc.DR_NERD(0.15f);
            npc.lifeMax = 500;
            npc.DeathSound = mod.GetLegacySoundSlot(SoundType.NPCKilled, "Sounds/NPCKilled/AstralEnemyDeath");
            npc.knockBackResist = 0f;
            npc.noGravity = true;
            npc.value = Item.buyPrice(0, 0, 20, 0);
            npc.timeLeft = NPC.activeTime * 2;
            animationType = NPCID.BlackRecluseWall;
            banner = ModContent.NPCType<AstralachneaGround>();
            bannerItem = ModContent.ItemType<AstralachneaBanner>();
            if (CalamityWorld.downedAstrageldon)
            {
                npc.damage = 90;
                npc.defense = 30;
                npc.lifeMax = 750;
            }
            npc.Calamity().VulnerableToHeat = true;
            npc.Calamity().VulnerableToSickness = false;
        }

        public override void AI()
        {
            CalamityGlobalNPC.DoSpiderWallAI(npc, ModContent.NPCType<AstralachneaGround>(), (CalamityWorld.death ? 3.6f : 2.4f), (CalamityWorld.death ? 0.15f : 0.1f));
        }

        public override void FindFrame(int frameHeight)
        {
            //DO DUST
            int frame = npc.frame.Y / frameHeight;
            Rectangle rect = new Rectangle(12, 24, 18, 10);
            Rectangle rect2 = new Rectangle(12, 44, 18, 10);
            switch (frame)
            {
                case 1:
                    rect = new Rectangle(6, 26, 28, 8);
                    rect2 = new Rectangle(6, 44, 28, 8);
                    break;
                case 2:
                    rect = new Rectangle(12, 26, 18, 8);
                    rect2 = new Rectangle(12, 44, 18, 8);
                    break;
                case 3:
                    rect = new Rectangle(16, 24, 16, 10);
                    rect2 = new Rectangle(16, 44, 16, 10);
                    break;
            }
            Dust d = CalamityGlobalNPC.SpawnDustOnNPC(npc, 80, frameHeight, ModContent.DustType<AstralOrange>(), rect, Vector2.Zero, 0.225f, true);
            Dust d2 = CalamityGlobalNPC.SpawnDustOnNPC(npc, 80, frameHeight, ModContent.DustType<AstralOrange>(), rect2, Vector2.Zero, 0.225f, true);
            if (d != null)
            {
                d.customData = 0.04f;
            }
            if (d2 != null)
            {
                d2.customData = 0.04f;
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
                    Gore.NewGore(npc.Center, npc.velocity * 0.3f, mod.GetGoreSlot("Gores/Astralachnea/AstralachneaGore" + i));
                }
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Vector2 origin = new Vector2(40f, 40f);
            spriteBatch.Draw(glowmask, npc.Center - Main.screenPosition - new Vector2(0, 8f), npc.frame, Color.White * 0.6f, npc.rotation, origin, 1f, SpriteEffects.None, 0);
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120, true);
        }

        public override void NPCLoot()
        {
            DropHelper.DropItem(npc, ModContent.ItemType<Stardust>(), 2, 3);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<Stardust>(), Main.expertMode);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<AstralachneaStaff>(), CalamityWorld.downedAstrageldon, 7, 1, 1);
        }
    }
}
