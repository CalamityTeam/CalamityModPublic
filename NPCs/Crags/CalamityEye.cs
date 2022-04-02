using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.Crags
{
    public class CalamityEye : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Calamity Eye");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.lavaImmune = true;
            npc.aiStyle = 2;
            npc.damage = 40;
            npc.width = 30;
            npc.height = 30;
            npc.defense = 12;
            npc.lifeMax = 140;
            npc.knockBackResist = 0f;
            animationType = NPCID.DemonEye;
            npc.value = Item.buyPrice(0, 0, 5, 0);
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            if (CalamityWorld.downedProvidence)
            {
                npc.damage = 80;
                npc.defense = 20;
                npc.lifeMax = 3000;
            }
            banner = npc.type;
            bannerItem = ModContent.ItemType<CalamityEyeBanner>();
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToCold = true;
            npc.Calamity().VulnerableToWater = true;
        }

        public override void AI()
        {
            if (npc.life < npc.lifeMax * 0.5)
            {
                if (npc.direction == -1 && npc.velocity.X > -6f)
                {
                    npc.velocity.X -= 0.1f;
                    if (npc.velocity.X > 6f)
                        npc.velocity.X -= 0.1f;
                    else if (npc.velocity.X > 0f)
                        npc.velocity.X += 0.05f;
                    if (npc.velocity.X < -6f)
                        npc.velocity.X = -6f;
                }
                else if (npc.direction == 1 && npc.velocity.X < 6f)
                {
                    npc.velocity.X += 0.1f;
                    if (npc.velocity.X < -6f)
                        npc.velocity.X += 0.1f;
                    else if (npc.velocity.X < 0f)
                        npc.velocity.X -= 0.05f;
                    if (npc.velocity.X > 6f)
                        npc.velocity.X = 6f;
                }
                if (npc.directionY == -1 && npc.velocity.Y > -4f)
                {
                    npc.velocity.Y -= 0.1f;
                    if (npc.velocity.Y > 4f)
                        npc.velocity.Y -= 0.1f;
                    else if (npc.velocity.Y > 0f)
                        npc.velocity.Y += 0.05f;
                    if (npc.velocity.Y < -4f)
                        npc.velocity.Y = -4f;
                }
                else if (npc.directionY == 1 && npc.velocity.Y < 4f)
                {
                    npc.velocity.Y += 0.1f;
                    if (npc.velocity.Y < -4f)
                        npc.velocity.Y += 0.1f;
                    else if (npc.velocity.Y < 0f)
                        npc.velocity.Y -= 0.05f;
                    if (npc.velocity.Y > 4f)
                        npc.velocity.Y = 4f;
                }
            }
            if (Main.rand.NextBool(40))
            {
                int index = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y + npc.height * 0.25f), npc.width, (int)(npc.height * 0.5), DustID.Blood, npc.velocity.X, 2f, 0, new Color(), 1f);
                Main.dust[index].velocity.X *= 0.5f;
                Main.dust[index].velocity.Y *= 0.1f;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.player.Calamity().ZoneCalamity ? 0.25f : 0f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120, true);
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemCondition(npc, ModContent.ItemType<Bloodstone>(), CalamityWorld.downedProvidence, 2, 1, 1);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<EssenceofChaos>(), Main.hardMode, 3, 1, 1);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<BlightedLens>(), Main.hardMode, 2, 1, 1);
            DropHelper.DropItemChance(npc, ItemID.Lens, 2);
        }
    }
}
