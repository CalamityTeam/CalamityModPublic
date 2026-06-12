using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Fearmonger
{
    [AutoloadEquip(EquipType.Head)]
    public class FearmongerGreathelm : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.PostMoonLord";

        public static int MaxManaBoost = 60;
        public static float ManaCostReduction = 0.1f;
        public static float SummonDamageBoost = 0.2f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MaxManaBoost, ManaCostReduction.ToPercent(), SummonDamageBoost.ToPercent());

        // Set Bonus
        public static int SetBonusMinionSlotBoost = 2;
        public static int RegenBoostDurationPerHit = CalamityUtils.SecondsToFrames(0.17f);
        public static int RegenBoostDurationLimit = CalamityUtils.SecondsToFrames(1.5f);
        public static int MinionRegenBoost = 5;
        public static int MinionRegenTimeBoost = 4;
        public static int MinionRegenTimeFloor = 900;

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            Item.defense = 35; // 125
            Item.rare = ModContent.RarityType<CosmicPurple>();
        }

        public override void UpdateEquip(Player player)
        {
            player.statManaMax2 += MaxManaBoost;
            player.manaCost -= ManaCostReduction;
            player.GetDamage<SummonDamageClass>() += SummonDamageBoost;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) => body.type == ModContent.ItemType<FearmongerPlateMail>() && legs.type == ModContent.ItemType<FearmongerGreaves>();

        public override void ArmorSetShadows(Player player) => player.armorEffectDrawOutlines = true;

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = this.GetLocalization("SetBonus").Format(SetBonusMinionSlotBoost, MinionRegenBoost.ToRegenPerSecond());

            // This bool encompasses cross-class nerf immunity and colossal life regen on minion attack
            // TODO -- Fearmonger life regen from minion attacks needs some sort of cool visual effect
            player.Calamity().fearmongerSet = true;

            // All-class armors count as rogue sets, but don't grant stealth bonuses
            player.Calamity().wearingRogueArmor = true;
            player.Calamity().WearingPostMLSummonerSet = true;
            player.maxMinions += SetBonusMinionSlotBoost;

            int[] immuneDebuffs = {
                BuffID.OnFire,
                BuffID.Frostburn,
                BuffID.CursedInferno,
                BuffID.ShadowFlame, //doesn't do anything
                ModContent.BuffType<Daybroken>(),
                BuffID.Burning,
                ModContent.BuffType<Shadowflame>(),
                ModContent.BuffType<BrimstoneFlames>(),
                ModContent.BuffType<HolyFlames>(),
                ModContent.BuffType<Voidfrost>(),
                ModContent.BuffType<GodSlayerInferno>(),
                BuffID.Chilled,
                BuffID.Frozen,
                ModContent.BuffType<WindChilled>(),
            };
            for (var i = 0; i < immuneDebuffs.Length; ++i)
                player.buffImmune[immuneDebuffs[i]] = true;

            // Constantly emit dim orange light
            Lighting.AddLight(player.Center, 0.3f, 0.18f, 0f);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SpookyHelmet).
                AddIngredient<CosmiliteBar>(8).
                AddIngredient<AscendantSpiritEssence>(2).
                AddIngredient(ItemID.SoulofFright, 8).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
