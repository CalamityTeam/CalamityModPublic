using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Items.Placeables.Plates;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [LegacyName("EldritchSoulArtifact")]
    public class FragmentsOfAnotherWorld : ModItem, ILocalizedModType
    {
        static float MeleeSpeedIncrease => 0.2f;
        static float RangedSpecialistDmgMult => 1.15f;
        static float ManaCostReduction => 0.25f;
        static int MaxSentries => 2;
        static float RogueSpeedIncrease => 0.1f;

        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.width = 64;
            Item.height = 58;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.RarityPurpleBuyPrice;
            Item.rare = ItemRarityID.Purple;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MeleeSpeedIncrease.ToPercent(), RangedSpecialistDmgMult, ManaCostReduction.ToPercent(), MaxSentries, RogueSpeedIncrease.ToPercent());

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.fragmentsOfAnotherWorld = true;

            player.buffImmune[ModContent.BuffType<WhisperingDeath>()] = true;
            player.GetAttackSpeed<MeleeDamageClass>() += MeleeSpeedIncrease;
            player.specialistDamage *= RangedSpecialistDmgMult;
            player.manaCost -= ManaCostReduction;
            player.maxTurrets += MaxSentries;
            player.GetAttackSpeed<RogueDamageClass>() += RogueSpeedIncrease;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Necroplasm>(5).
                AddIngredient<Navyplate>(25).
                AddIngredient<ExodiumCluster>(25).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
