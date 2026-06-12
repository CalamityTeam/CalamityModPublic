using CalamityMod.Buffs.Summon;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Demonshade
{
    [AutoloadEquip(EquipType.Head)]
    public class DemonshadeHelm : ModItem, IExtendedHat, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.PostMoonLord";
        public static readonly SoundStyle ActivationSound = new("CalamityMod/Sounds/Custom/AbilitySounds/DemonshadeEnrage");
        internal static string ShadowScytheEntitySourceContext => "SetBonus_Calamity_Demonshade";

        public static int MinionSlotBoost = 2;
        public static float DamageBoost = 0.3f;
        public static int CritBoost = 15;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MinionSlotBoost, DamageBoost.ToPercent(), CritBoost);

        // Set Bonus
        public static int SetBonusMinionSlotBoost = 8;
        public static float SetBonusSummonDamageBoost = 1f;
        public static int BeamDamage => CalamityUtils.ScaleWithDifficulty(300);
        public static int ScytheDamage => CalamityUtils.ScaleWithDifficulty(500);
        public static int EnrageDuration = CalamityUtils.SecondsToFrames(10);
        public static float MultDamageBoost = 0.5f;
        public static double MultDamageTakenBoost = 0.25D;

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = 50;
            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.Calamity().devItem = true;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) => body.type == ModContent.ItemType<DemonshadeBreastplate>() && legs.type == ModContent.ItemType<DemonshadeGreaves>();

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = this.GetLocalization("SetBonus").Format(SetBonusMinionSlotBoost, SetBonusSummonDamageBoost.ToPercent(), CalamityUtils.GetArmorSetBonusKey(), EnrageDuration.FramesToSeconds(), (1f + MultDamageBoost).Round(), (1D + MultDamageTakenBoost).Round());
            var modPlayer = player.Calamity();
            modPlayer.dsSetBonus = true;
            modPlayer.wearingRogueArmor = true;
            modPlayer.WearingPostMLSummonerSet = true;
            player.maxMinions += SetBonusMinionSlotBoost;
            player.GetDamage<SummonDamageClass>() += SetBonusSummonDamageBoost;
        }

        public override void UpdateEquip(Player player)
        {
            player.maxMinions += MinionSlotBoost;
            player.GetDamage<GenericDamageClass>() += DamageBoost;
            player.GetCritChance<GenericDamageClass>() += CritBoost;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ShadowspecBar>(12).
                AddTile<DraedonsForge>().
                SortBeforeFirstRecipesOf(ModContent.ItemType<DemonshadeBreastplate>()).
                Register();
        }

        public string ExtensionTexture => "CalamityMod/Items/Armor/Demonshade/DemonshadeHelm_Extension";
        public Vector2 ExtensionSpriteOffset(PlayerDrawSet drawInfo) => new Vector2(0, -4f);
    }
}
