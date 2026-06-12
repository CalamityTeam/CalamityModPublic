using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Empyrean
{
    [AutoloadEquip(EquipType.Head)]
    [LegacyName("XerocMask")]
    public class EmpyreanMask : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.PostMoonLord";

        public static float RogueDamageBoost = 0.12f;
        public static int RogueCritBoost = 7;
        public static float RogueVelocityBoost = 0.1f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(RogueDamageBoost.ToPercent(), RogueCritBoost, RogueVelocityBoost.ToPercent());

        // Set Bonus
        public static float SetBonusRogueStealth = 1.15f;
        public static int WrathDuration = CalamityUtils.SecondsToFrames(3);
        public static float PermanentWrathHealthRatio = 0.5f;
        public static float WrathRogueDamageBoost = 0.1f;
        public static int WrathRogueCritBoost = 5;
        // There's 10 or so different magic numbers polluting CalamityPlayerOnHit for each projectile. I'm not adding it. - Iris

        public override void Load()
        {
            if (!Main.dedServ)
            {
                EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Armor/Empyrean/MeldTransformation_Head", EquipType.Head, name: "MeldTransformation");
                EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Armor/Empyrean/MeldTransformation_Body", EquipType.Body, name: "MeldTransformation");
                EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Armor/Empyrean/MeldTransformation_Neck", EquipType.Neck, name: "MeldTransformation");
                EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Armor/Empyrean/MeldTransformation_Legs", EquipType.Legs, name: "MeldTransformation");
            }
        }

        public override void SetStaticDefaults()
        {
            if (Main.dedServ)
                return;

            var equipSlotHead = EquipLoader.GetEquipSlot(Mod, "MeldTransformation", EquipType.Head);
            var equipSlotBody = EquipLoader.GetEquipSlot(Mod, "MeldTransformation", EquipType.Body);
            var equipSlotLegs = EquipLoader.GetEquipSlot(Mod, "MeldTransformation", EquipType.Legs);

            ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;
            ArmorIDs.Body.Sets.HidesTopSkin[equipSlotBody] = true;
            ArmorIDs.Body.Sets.HidesArms[equipSlotBody] = true;
            ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlotLegs] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityRedBuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.defense = 16; // 60
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) => body.type == ModContent.ItemType<EmpyreanCloak>() && legs.type == ModContent.ItemType<EmpyreanCuisses>();

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
            player.armorEffectDrawOutlines = true;
            player.Calamity().meldTransformation = true;
            player.Calamity().meldTransformationForce = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            var modPlayer = player.Calamity();
            modPlayer.xerocSet = true;
            modPlayer.rogueStealthMax += SetBonusRogueStealth;
            player.setBonus = this.GetLocalization("SetBonus").Format(SetBonusRogueStealth.ToStealth(), PermanentWrathHealthRatio.ToPercent());
            if (player.statLife <= (int)(player.statLifeMax2 * PermanentWrathHealthRatio))
                player.AddBuff(ModContent.BuffType<EmpyreanWrath>(), 2);
            modPlayer.wearingRogueArmor = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ThrowingDamageClass>() += RogueDamageBoost;
            player.GetCritChance<ThrowingDamageClass>() += RogueCritBoost;
            player.Calamity().rogueVelocity += RogueVelocityBoost;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MeldBlob>(10).
                AddIngredient(ItemID.LunarBar, 8).
                AddTile(TileID.LunarCraftingStation).
                SortBeforeFirstRecipesOf(ModContent.ItemType<EmpyreanCloak>()).
                Register();
        }
    }
}
