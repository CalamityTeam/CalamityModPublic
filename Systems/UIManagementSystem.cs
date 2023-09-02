using System.Collections.Generic;
using CalamityMod.UI;
using CalamityMod.UI.CalamitasEnchants;
using CalamityMod.UI.DraedonsArsenal;
using CalamityMod.UI.DraedonSummoning;
using CalamityMod.UI.ModeIndicator;
using CalamityMod.UI.Rippers;
using CalamityMod.UI.SulphurousWaterMeter;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace CalamityMod.Systems
{
    public class UIManagementSystem : ModSystem
    {
        public static Vector2 PreviousMouseWorld;

        public static Vector2 PreviousZoom;

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int buffDisplayIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Resource Bars");
            if (buffDisplayIndex != -1)
            {
                layers.Insert(buffDisplayIndex, new LegacyGameInterfaceLayer("Cooldown Rack UI", delegate ()
                {
                    CooldownRackUI.Draw(Main.spriteBatch);
                    return true;
                }, InterfaceScaleType.UI));
            }

            int mouseIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Mouse Text");
            if (mouseIndex != -1)
            {
                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("Draedon Hologram", () =>
                {
                    LabHologramProjectorUI.Draw(Main.spriteBatch);
                    return true;
                }, InterfaceScaleType.None));

                // For these layers, InterfaceScaleType.Game tells the game that this UI should take zoom into account.
                // These must be separate layers or they will malfunction when hovering one at non-100% zoom.

                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("Charging Station UI", () =>
                {
                    ChargingStationUI.Draw(Main.spriteBatch);
                    return true;
                }, InterfaceScaleType.Game));

                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("Power Cell Factory UI", () =>
                {
                    PowerCellFactoryUI.Draw(Main.spriteBatch);
                    return true;
                }, InterfaceScaleType.Game));

                // Mode Indicator UI.
                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("Mode Indicator UI", delegate ()
                {
                    ModeIndicatorUI.Draw(Main.spriteBatch);
                    return true;
                }, InterfaceScaleType.UI));

                // Speedrun Timer
                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("Speedrun Timer", delegate ()
                {
                    SpeedrunTimerUI.Draw(Main.LocalPlayer);
                    return true;
                }, InterfaceScaleType.None));

                // Rage and Adrenaline bars
                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("Rage and Adrenaline UI", delegate ()
                {
                    RipperUI.Draw(Main.spriteBatch, Main.LocalPlayer);
                    return true;
                }, InterfaceScaleType.None));

                // Stealth bar
                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("Stealth UI", () =>
                {
                    StealthUI.Draw(Main.spriteBatch, Main.LocalPlayer);
                    return true;
                }, InterfaceScaleType.None));

                // Sulphuric water poison bar
                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("Sulphuric Water Poisoning UI", () =>
                {
                    SulphurousWaterMeterUI.Draw(Main.spriteBatch, Main.LocalPlayer);
                    return true;
                }, InterfaceScaleType.None));
                
                //Flight bar
                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("Flight UI", () =>
                {
                    FlightBar.Draw(Main.spriteBatch, Main.LocalPlayer);
                    return true;
                }, InterfaceScaleType.None));

                // Charge meter
                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("Charge UI", () =>
                {
                    ChargeMeterUI.Draw(Main.spriteBatch, Main.LocalPlayer);
                    return true;
                }, InterfaceScaleType.None));

                // Enchantment meters
                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("Enchantment Meters", () =>
                {
                    EnchantmentMetersUI.Draw(Main.spriteBatch, Main.LocalPlayer);
                    return true;
                }, InterfaceScaleType.None));

                // Calamitas Enchantment UI
                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("Calamitas Enchantment", () =>
                {
                    CalamitasEnchantUI.Draw(Main.spriteBatch);
                    return true;
                }, InterfaceScaleType.None));

                // Codebreaker UI.
                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("Codebreaker Decryption GUI", () =>
                {
                    CodebreakerUI.Draw(Main.spriteBatch);
                    return true;
                }, InterfaceScaleType.None));

                // Popup GUIs.
                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("Popup GUIs", () =>
                {
                    PopupGUIManager.UpdateAndDraw(Main.spriteBatch);
                    return true;
                }, InterfaceScaleType.None));

                // Exo Mech selection.
                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("Exo Mech Selection", () =>
                {
                    if (Main.LocalPlayer.Calamity().AbleToSelectExoMech)
                        ExoMechSelectionUI.Draw();
                    else
                        ExoMechSelectionUI.HoverSoundMechType = null;
                    return true;
                }, InterfaceScaleType.None));

                // Defense damage indicator.
                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("Defense Damage Indicator", () =>
                {
                    if (Main.EquipPage != 1 && Main.EquipPage != 2)
                        DefenseDamageDisplayUI.Draw(Main.spriteBatch);
                    return true;
                }, InterfaceScaleType.None));
            }

            // Invasion UIs.
            int invasionIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Diagnose Net");
            if (invasionIndex != -1)
            {
                layers.Insert(invasionIndex, new LegacyGameInterfaceLayer("Calamity Invasion UIs", () =>
                {
                    InvasionProgressUIManager.UpdateAndDraw(Main.spriteBatch);
                    return true;
                }, InterfaceScaleType.None));
            }
        }
    }
}
