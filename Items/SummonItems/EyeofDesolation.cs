using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.Calamitas;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.SummonItems
{
    [LegacyName("BlightedEyeball")]
    public class EyeofDesolation : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Eye of Desolation");
            Tooltip.SetDefault("Tonight is going to be a horrific night...\n" +
                "Summons Calamitas when used during nighttime\n" +
                "Enrages during the day\n" +
                "Not consumable");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.rare = ItemRarityID.LightPurple;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return !Main.dayTime && !NPC.AnyNPCs(ModContent.NPCType<CalamitasClone>()) && !BossRushEvent.BossRushActive;
        }

        public override bool? UseItem(Player player)
        {
            SoundEngine.PlaySound(SoundID.Roar, player.position);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<CalamitasClone>());
            else
                NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, ModContent.NPCType<CalamitasClone>());

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.HellstoneBar, 10).
                AddIngredient<EssenceofChaos>(7).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
