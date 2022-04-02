using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing.AstralCatches
{
    public class UrsaSergeant : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ursa Sergeant");
            Tooltip.SetDefault("15% decreased movement speed\n" +
                "Immune to Astral Infection and Feral Bite\n" +
                "Increased regeneration at lower health");
        }

        public override void SetDefaults()
        {
            item.defense = 20;
            item.width = 36;
            item.height = 26;
            item.value = CalamityGlobalItem.Rarity4BuyPrice;
            item.rare = ItemRarityID.LightRed;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.ursaSergeant = true;
            player.buffImmune[ModContent.BuffType<AstralInfectionDebuff>()] = true;
            player.buffImmune[BuffID.Rabies] = true; //Feral Bite
        }
    }
}
