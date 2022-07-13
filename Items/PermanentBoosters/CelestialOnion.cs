using CalamityMod.CalPlayer;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.PermanentBoosters
{
    [LegacyName("MLGRune2")]
    public class CelestialOnion : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Celestial Onion");
            Tooltip.SetDefault(@"Alien power pulses inside its layers
Consuming it does something that cannot be reversed
This item does nothing in Master Mode");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.rare = ItemRarityID.Red;
            Item.maxStack = 99;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item4;
            Item.consumable = true; // Not researchable, only drops one time.
        }

        public override bool CanUseItem(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            return !Main.masterMode && !modPlayer.extraAccessoryML;
        }

        public override bool? UseItem(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (player.itemAnimation > 0 && !modPlayer.extraAccessoryML && player.itemTime == 0)
            {
                player.itemTime = Item.useTime;
                modPlayer.extraAccessoryML = true;

                // TODO -- remove "onionMode", it does nothing. It is the old internal name for "Prepare to Cry".
                if (!CalamityWorld.onionMode)
                    CalamityWorld.onionMode = true;
            }
            return true;
        }
    }

    public class CelestialOnionAccessorySlot : ModAccessorySlot
    {
        // Celestial Onion does not work in Master Mode.
        public override bool IsEnabled() => !Main.masterMode && (Player?.Calamity().extraAccessoryML ?? false);
        public override bool IsHidden() => IsEmpty && !IsEnabled();
    }
}
