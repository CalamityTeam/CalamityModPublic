using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.CalPlayer.Dashes;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Shield)]
    public class AsgardianAegis : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public const int ShieldSlamIFrames = 12;

        public override void SetDefaults()
        {
            Item.width = 60;
            Item.height = 54;
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.defense = 28;
            Item.accessory = true;
            Item.rare = ModContent.RarityType<DarkBlue>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.DashID = AsgardianAegisDash.ID;
            player.dashType = 0;
            player.noKnockback = true;
            player.fireWalk = true;
            player.statLifeMax2 += 40;
            player.lifeRegen++;
            player.buffImmune[BuffID.OnFire] = true;
            player.buffImmune[BuffID.Chilled] = true;
            player.buffImmune[BuffID.Frostburn] = true;
            player.buffImmune[BuffID.Weak] = true;
            player.buffImmune[BuffID.BrokenArmor] = true;
            player.buffImmune[BuffID.Bleeding] = true;
            player.buffImmune[BuffID.Poisoned] = true;
            player.buffImmune[BuffID.Slow] = true;
            player.buffImmune[BuffID.Confused] = true;
            player.buffImmune[BuffID.Silenced] = true;
            player.buffImmune[BuffID.Cursed] = true;
            player.buffImmune[BuffID.Darkness] = true;
            player.buffImmune[BuffID.WindPushed] = true;
            player.buffImmune[BuffID.Stoned] = true;
            player.buffImmune[ModContent.BuffType<HolyFlames>()] = true;
            player.buffImmune[ModContent.BuffType<GodSlayerInferno>()] = true;
            if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
            { player.statDefense += 20; }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AsgardsValor>().
                AddIngredient<ElysianAegis>().
                AddIngredient<CosmiliteBar>(10).
                AddIngredient<AscendantSpiritEssence>(4).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
