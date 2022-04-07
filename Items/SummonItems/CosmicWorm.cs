using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using System.Linq;

namespace CalamityMod.Items.SummonItems
{
    public class CosmicWorm : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmic Worm");
            Tooltip.SetDefault("Summons the Devourer of Gods\n" +
                "SENTINEL WARNING TOOLTIP LINE HERE\n" +
                "Not consumable");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            TooltipLine line = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip1");
            bool sentinelsNotDefeated = !DownedBossSystem.downedCeaselessVoid || !DownedBossSystem.downedStormWeaver || !DownedBossSystem.downedSignus;

            if (line != null)
                line.Text = sentinelsNotDefeated ? "WARNING! Some sentinels have not been truly defeated yet and will spawn at full power during this fight!" : "";
        }

        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(ModContent.NPCType<DevourerofGodsHead>()) && CalamityWorld.DoGSecondStageCountdown <= 0 && !BossRushEvent.BossRushActive;
        }

        public override bool? UseItem(Player player)
        {
            string key = "Mods.CalamityMod.EdgyBossText12";
            Color messageColor = Color.Cyan;
            CalamityUtils.DisplayLocalizedText(key, messageColor);

            SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/DevourerSpawn"), (int)player.position.X, (int)player.position.Y);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<DevourerofGodsHead>());
            else
                NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, ModContent.NPCType<DevourerofGodsHead>());

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<ArmoredShell>(), 3).AddIngredient(ModContent.ItemType<TwistingNether>()).AddIngredient(ModContent.ItemType<DarkPlasma>()).AddTile(TileID.LunarCraftingStation).Register();
            CreateRecipe(1).AddIngredient(ItemID.IronBar, 30).AddIngredient(ItemID.LunarBar, 10).AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 20).AddIngredient(ItemID.SoulofLight, 20).AddIngredient(ItemID.SoulofNight, 20).AddIngredient(ModContent.ItemType<Phantoplasm>(), 30).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}
