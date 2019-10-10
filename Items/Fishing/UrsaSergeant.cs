using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.Fishing
{
    public class UrsaSergeant : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ursa Sergeant");
            Tooltip.SetDefault("+20 defense but 20% reduced movement speed\n" +
                "Immune to Astral Infection and Feral Bite\n" +
                "Increased regeneration at lower health");
        }

        public override void SetDefaults()
        {
            item.width = 36;
            item.height = 26;
            item.value = Item.buyPrice(0, 24, 0, 0);
            item.rare = 8;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.ursaSergeant = true;
            player.statDefense += 20;
            player.buffImmune[mod.BuffType("AstralInfectionDebuff")] = true;
            player.buffImmune[148] = true; //Feral Bite
        }
    }
}
