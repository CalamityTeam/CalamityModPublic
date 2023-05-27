using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing.AstralCatches
{
    public class UrsaSergeant : ModItem
    {
        public override void SetDefaults()
        {
            Item.defense = 20;
            Item.width = 36;
            Item.height = 26;
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.accessory = true;
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
