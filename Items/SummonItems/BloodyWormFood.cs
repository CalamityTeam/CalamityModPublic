using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.Perforator;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.SummonItems
{
    public class BloodyWormFood : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Bloody Worm Food");
            Tooltip.SetDefault("Summons the Perforator Hive when used in the crimson\n" +
                "Not consumable");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.rare = ItemRarityID.Orange;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ZoneCrimson && !NPC.AnyNPCs(ModContent.NPCType<PerforatorHive>()) && !BossRushEvent.BossRushActive;
        }

        public override bool? UseItem(Player player)
        {
            SoundEngine.PlaySound(SoundID.Roar, player.position);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<PerforatorHive>());
            else
                NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, ModContent.NPCType<PerforatorHive>());

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Vertebrae, 13).
                AddIngredient<BloodSample>(7).
                AddIngredient(ItemID.CrimtaneBar, 3).
                AddTile(TileID.DemonAltar).
                Register();
        }
    }
}
