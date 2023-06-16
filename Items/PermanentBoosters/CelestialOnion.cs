using CalamityMod.CalPlayer;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.PermanentBoosters
{
    [LegacyName("MLGRune2")]
    public class CelestialOnion : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Misc";
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.rare = ItemRarityID.Red;
            Item.maxStack = 9999;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item4;
            Item.consumable = true;
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
        public override bool IsEnabled()
        {
            // GetModPlayer will throw an index error in this step of the loading process for whatever reason
            // We prematurely stop it from getting to that point
            if (!Player.active || !Main.masterMode)
                return false;
            
            return Player.Calamity().extraAccessoryML;
        }
        public override bool IsHidden() => IsEmpty && !IsEnabled();
    }
}
