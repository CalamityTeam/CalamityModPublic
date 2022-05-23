using CalamityMod.CalPlayer;
using CalamityMod.Events;
using CalamityMod.Items.Placeables;
using CalamityMod.NPCs.AquaticScourge;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.SummonItems
{
    public class Seafood : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Seafood");
            Tooltip.SetDefault("Summons the Aquatic Scourge when used in the sulphur sea\n" +
                "Not consumable");
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 24;
            Item.rare = ItemRarityID.Pink;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            return modPlayer.ZoneSulphur && !NPC.AnyNPCs(ModContent.NPCType<AquaticScourgeHead>()) && !BossRushEvent.BossRushActive;
        }

        public override bool? UseItem(Player player)
        {
            SoundEngine.PlaySound(SoundID.Roar, player.position);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<AquaticScourgeHead>());
            else
                NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, ModContent.NPCType<AquaticScourgeHead>());

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SulphurousSand>(20).
                AddIngredient(ItemID.Starfish, 10).
                AddIngredient(ItemID.SharkFin, 5).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
