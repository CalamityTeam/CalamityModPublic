using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Crags
{
    public class ScryllarRage : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scryllar");
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            aiType = -1;
            NPC.damage = 65;
            NPC.width = 80;
            NPC.height = 80;
            NPC.defense = 10;
            NPC.lifeMax = 50;
            NPC.alpha = 100;
            NPC.knockBackResist = 0.9f;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.HitSound = SoundID.NPCHit49;
            NPC.DeathSound = SoundID.NPCDeath51;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.lavaImmune = true;
            if (CalamityWorld.downedProvidence)
            {
                NPC.damage = 110;
                NPC.defense = 20;
                NPC.lifeMax = 1500;
            }
            banner = ModContent.NPCType<Scryllar>();
            bannerItem = ModContent.ItemType<ScryllarBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToWater = true;
        }

        public override void AI()
        {
            NPC.rotation = NPC.velocity.X * 0.04f;
            NPC.spriteDirection = (NPC.direction > 0) ? 1 : -1;
            bool flag19 = false;
            if (NPC.justHit)
            {
                NPC.ai[2] = 0f;
            }
            if (NPC.ai[2] >= 0f)
            {
                int num282 = 16;
                bool flag21 = false;
                bool flag22 = false;
                if (NPC.position.X > NPC.ai[0] - (float)num282 && NPC.position.X < NPC.ai[0] + (float)num282)
                {
                    flag21 = true;
                }
                else if ((NPC.velocity.X < 0f && NPC.direction > 0) || (NPC.velocity.X > 0f && NPC.direction < 0))
                {
                    flag21 = true;
                }
                num282 += 24;
                if (NPC.position.Y > NPC.ai[1] - (float)num282 && NPC.position.Y < NPC.ai[1] + (float)num282)
                {
                    flag22 = true;
                }
                if (flag21 && flag22)
                {
                    NPC.ai[2] += 1f;
                    if (NPC.ai[2] >= 30f && num282 == 16)
                    {
                        flag19 = true;
                    }
                    if (NPC.ai[2] >= 60f)
                    {
                        NPC.ai[2] = -200f;
                        NPC.direction *= -1;
                        NPC.velocity.X = NPC.velocity.X * -1f;
                        NPC.collideX = false;
                    }
                }
                else
                {
                    NPC.ai[0] = NPC.position.X;
                    NPC.ai[1] = NPC.position.Y;
                    NPC.ai[2] = 0f;
                }
                NPC.TargetClosest(true);
            }
            else
            {
                NPC.TargetClosest(true);
                NPC.ai[2] += 2f;
            }
            int num283 = (int)((NPC.position.X + (float)(NPC.width / 2)) / 16f) + NPC.direction * 2;
            int num284 = (int)((NPC.position.Y + (float)NPC.height) / 16f);
            bool flag23 = true;
            int num285 = 3;
            for (int num308 = num284; num308 < num284 + num285; num308++)
            {
                if (Main.tile[num283, num308] == null)
                {
                    Main.tile[num283, num308] = new Tile();
                }
                if ((Main.tile[num283, num308].nactive() && Main.tileSolid[(int)Main.tile[num283, num308].TileType]) || Main.tile[num283, num308].LiquidAmount > 0)
                {
                    flag23 = false;
                    break;
                }
            }
            if (Main.player[NPC.target].npcTypeNoAggro[NPC.type])
            {
                bool flag25 = false;
                for (int num309 = num284; num309 < num284 + num285 - 2; num309++)
                {
                    if (Main.tile[num283, num309] == null)
                    {
                        Main.tile[num283, num309] = new Tile();
                    }
                    if ((Main.tile[num283, num309].nactive() && Main.tileSolid[(int)Main.tile[num283, num309].TileType]) || Main.tile[num283, num309].LiquidAmount > 0)
                    {
                        flag25 = true;
                        break;
                    }
                }
                NPC.directionY = (!flag25).ToDirectionInt();
            }
            if (flag19)
            {
                flag23 = true;
            }
            if (flag23)
            {
                NPC.velocity.Y = NPC.velocity.Y + 0.1f;
                if (NPC.velocity.Y > 3f)
                {
                    NPC.velocity.Y = 3f;
                }
            }
            else
            {
                if (NPC.directionY < 0 && NPC.velocity.Y > 0f)
                {
                    NPC.velocity.Y = NPC.velocity.Y - 0.1f;
                }
                if (NPC.velocity.Y < -4f)
                {
                    NPC.velocity.Y = -4f;
                }
            }
            if (NPC.collideX)
            {
                NPC.velocity.X = NPC.oldVelocity.X * -0.4f;
                if (NPC.direction == -1 && NPC.velocity.X > 0f && NPC.velocity.X < 1f)
                {
                    NPC.velocity.X = 1f;
                }
                if (NPC.direction == 1 && NPC.velocity.X < 0f && NPC.velocity.X > -1f)
                {
                    NPC.velocity.X = -1f;
                }
            }
            if (NPC.collideY)
            {
                NPC.velocity.Y = NPC.oldVelocity.Y * -0.25f;
                if (NPC.velocity.Y > 0f && NPC.velocity.Y < 1f)
                {
                    NPC.velocity.Y = 1f;
                }
                if (NPC.velocity.Y < 0f && NPC.velocity.Y > -1f)
                {
                    NPC.velocity.Y = -1f;
                }
            }
            float num311 = 7f;
            if (NPC.direction == -1 && NPC.velocity.X > -num311)
            {
                NPC.velocity.X = NPC.velocity.X - 0.1f;
                if (NPC.velocity.X > num311)
                {
                    NPC.velocity.X = NPC.velocity.X - 0.1f;
                }
                else if (NPC.velocity.X > 0f)
                {
                    NPC.velocity.X = NPC.velocity.X + 0.05f;
                }
                if (NPC.velocity.X < -num311)
                {
                    NPC.velocity.X = -num311;
                }
            }
            else if (NPC.direction == 1 && NPC.velocity.X < num311)
            {
                NPC.velocity.X = NPC.velocity.X + 0.1f;
                if (NPC.velocity.X < -num311)
                {
                    NPC.velocity.X = NPC.velocity.X + 0.1f;
                }
                else if (NPC.velocity.X < 0f)
                {
                    NPC.velocity.X = NPC.velocity.X - 0.05f;
                }
                if (NPC.velocity.X > num311)
                {
                    NPC.velocity.X = num311;
                }
            }
            num311 = 1.5f;
            if (NPC.directionY == -1 && NPC.velocity.Y > -num311)
            {
                NPC.velocity.Y = NPC.velocity.Y - 0.04f;
                if (NPC.velocity.Y > num311)
                {
                    NPC.velocity.Y = NPC.velocity.Y - 0.05f;
                }
                else if (NPC.velocity.Y > 0f)
                {
                    NPC.velocity.Y = NPC.velocity.Y + 0.03f;
                }
                if (NPC.velocity.Y < -num311)
                {
                    NPC.velocity.Y = -num311;
                }
            }
            else if (NPC.directionY == 1 && NPC.velocity.Y < num311)
            {
                NPC.velocity.Y = NPC.velocity.Y + 0.04f;
                if (NPC.velocity.Y < -num311)
                {
                    NPC.velocity.Y = NPC.velocity.Y + 0.05f;
                }
                else if (NPC.velocity.Y < 0f)
                {
                    NPC.velocity.Y = NPC.velocity.Y - 0.03f;
                }
                if (NPC.velocity.Y > num311)
                {
                    NPC.velocity.Y = num311;
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.Calamity().ZoneCalamity ? 0.125f : 0f;
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemCondition(NPC, ModContent.ItemType<LanternoftheSoul>(), CalamityWorld.downedProvidence, 20, 1, 1);
            DropHelper.DropItemCondition(NPC, ModContent.ItemType<Bloodstone>(), CalamityWorld.downedProvidence, 2, 1, 1);
            DropHelper.DropItemCondition(NPC, ModContent.ItemType<EssenceofChaos>(), Main.hardMode, 3, 1, 1);
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120, true);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Brimstone, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 40; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Brimstone, hitDirection, -1f, 0, default, 1f);
                }
                Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/ScryllarGores/ScryllarRage"), NPC.scale);
                Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/ScryllarGores/ScryllarRage2"), NPC.scale);
            }
        }
    }
}
