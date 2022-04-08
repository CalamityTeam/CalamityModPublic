using CalamityMod.Events;
using CalamityMod.NPCs.Crabulon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;

namespace CalamityMod.Items.SummonItems
{
    public class DecapoditaSprout : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Decapodita Sprout");
            Tooltip.SetDefault("Summons Crabulon when used in the mushroom biome\n" +
                "Not consumable");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.rare = ItemRarityID.Green;
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ZoneGlowshroom && !NPC.AnyNPCs(ModContent.NPCType<CrabulonIdle>()) && !BossRushEvent.BossRushActive;
        }

        public override bool? UseItem(Player player)
        {
            SoundEngine.PlaySound(SoundID.Roar, player.position, 0);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int npc = NPC.NewNPC(new EntitySource_BossSpawn(player), (int)(player.position.X + Main.rand.Next(-250, 251)), (int)(player.position.Y - 500f), ModContent.NPCType<CrabulonIdle>(), 1);
                Main.npc[npc].timeLeft *= 20;
                CalamityUtils.BossAwakenMessage(npc);
            }
            else
                NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, ModContent.NPCType<CrabulonIdle>());

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.GlowingMushroom, 50).AddTile(TileID.DemonAltar).Register();
        }
    }
}
