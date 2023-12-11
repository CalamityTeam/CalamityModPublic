using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.CalPlayer.Dashes;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Shield)]
    public class AsgardianAegis : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public const int ShieldSlamDamage = 1000;
        public const float ShieldSlamKnockback = 15f;
        public const int ShieldSlamIFrames = 12;

        public const int RamExplosionDamage = 1000;
        public const float RamExplosionKnockback = 20f;

        public override void SetDefaults()
        {
            Item.width = 60;
            Item.height = 54;
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.defense = 20;
            Item.accessory = true;
            Item.rare = ModContent.RarityType<DarkBlue>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();

            // Asgardian Aegis ram dash
            modPlayer.DashID = AsgardianAegisDash.ID;
            player.dashType = 0;

            // Inherited Ankh Shield effects
            player.noKnockback = true;
            player.fireWalk = true;
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

            // Additional debuff immunities (Counterparts, similar debuffs to vanilla Ankh Shield debuffs, or upgrades thereof)
            player.buffImmune[ModContent.BuffType<ArmorCrunch>()] = true; // "Stronger" Broken Armor
            player.buffImmune[ModContent.BuffType<BrainRot>()] = true; // Counterpart to Burning Blood
            player.buffImmune[ModContent.BuffType<BurningBlood>()] = true; // "Stronger" Bleeding
            player.buffImmune[BuffID.Venom] = true; // "Stronger" Poisoned
            player.buffImmune[ModContent.BuffType<SulphuricPoisoning>()] = true; // "Stronger" Poisoned
            player.buffImmune[BuffID.Webbed] = true; // "Stronger" Slow
            player.buffImmune[BuffID.Blackout] = true; // "Stronger" Darkness

            // Additional debuff immunities (Everything from Ornate Shield + Asgard's Valor)
            player.buffImmune[BuffID.OnFire] = true;
            player.buffImmune[BuffID.OnFire3] = true;
            player.buffImmune[ModContent.BuffType<BrimstoneFlames>()] = true;
            player.buffImmune[BuffID.Chilled] = true;
            player.buffImmune[BuffID.Frozen] = true;
            player.buffImmune[BuffID.Frostburn] = true;
            player.buffImmune[BuffID.Frostburn2] = true;

            // Additional debuff immunities (Everything from Elysian Aegis + thematic counterparts)
            player.buffImmune[BuffID.CursedInferno] = true;
            player.buffImmune[BuffID.ShadowFlame] = true;
            player.buffImmune[BuffID.Daybreak] = true;
            player.buffImmune[ModContent.BuffType<Nightwither>()] = true;
            player.buffImmune[ModContent.BuffType<HolyFlames>()] = true;

            // Immune to God Slayer Inferno itself
            player.buffImmune[ModContent.BuffType<GodSlayerInferno>()] = true;
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
