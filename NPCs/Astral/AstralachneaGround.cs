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
    public class AstralachneaGround : ModNPC
    {
        private static Texture2D glowmask;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astralachnea");

            Main.npcFrameCount[npc.type] = 5;

            if (!Main.dedServ)
                glowmask = ModContent.GetTexture("CalamityMod/NPCs/Astral/AstralachneaGroundGlow");

            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            npc.width = 70;
            npc.height = 34;
            npc.aiStyle = 3;
            npc.damage = 55;
            npc.defense = 20;
            npc.DR_NERD(0.15f);
            npc.lifeMax = 500;
            npc.DeathSound = mod.GetLegacySoundSlot(SoundType.NPCKilled, "Sounds/NPCKilled/AstralEnemyDeath");
            npc.knockBackResist = 0.38f;
            npc.value = Item.buyPrice(0, 0, 20, 0);
            npc.timeLeft = NPC.activeTime * 2;
            animationType = NPCID.WallCreeper;
            banner = npc.type;
            bannerItem = ModContent.ItemType<AstralachneaBanner>();
            if (CalamityWorld.downedAstrageldon)
            {
                npc.damage = 90;
                npc.defense = 30;
                npc.knockBackResist = 0.28f;
                npc.lifeMax = 750;
            }
            npc.Calamity().VulnerableToHeat = true;
            npc.Calamity().VulnerableToSickness = false;
        }

        public override void AI()
        {
            npc.TargetClosest();
            if (Main.netMode != NetmodeID.MultiplayerClient && npc.velocity.Y == 0f)
            {
                int x = (int)npc.Center.X / 16;
                int y = (int)npc.Center.Y / 16;
                bool transform = false;
                for (int i = x - 1; i <= x + 1; i++)
                {
                    for (int j = y - 1; j <= y + 1; j++)
                    {
                        if (Main.tile[i, j] != null && Main.tile[i, j].wall > 0)
                        {
                            transform = true;
                        }
                    }
                }
                if (transform)
                {
                    npc.Transform(ModContent.NPCType<AstralachneaWall>());
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            //DO DUST
            int frame = npc.frame.Y / frameHeight;
            Rectangle rect = new Rectangle(62, 4, 14, 6);
            switch (frame)
            {
                case 1:
                    rect = new Rectangle(64, 6, 12, 6);
                    break;
                case 2:
                    rect = new Rectangle(58, 8, 22, 6);
                    break;
                case 3:
                    rect = new Rectangle(54, 8, 26, 8);
                    break;
                case 4:
                    rect = new Rectangle(58, 6, 20, 8);
                    break;
            }
            Dust d = CalamityGlobalNPC.SpawnDustOnNPC(npc, 80, frameHeight, ModContent.DustType<AstralOrange>(), rect, Vector2.Zero, 0.45f, true);
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
                    Gore.NewGore(npc.Center, npc.velocity * 0.3f, mod.GetGoreSlot("Gores/Astralachnea/AstralachneaGore" + i));
                }
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Vector2 origin = new Vector2(40f, 21f);
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
                return 0.17f;
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
            DropHelper.DropItemCondition(npc, ModContent.ItemType<AstralachneaStaff>(), CalamityWorld.downedAstrageldon, 7, 1, 1);
        }
    }
}
