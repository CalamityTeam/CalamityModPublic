using CalamityMod.Buffs.StatBuffs;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Empyrean
{
    [AutoloadEquip(EquipType.Head)]
    [LegacyName("XerocMask")]
    public class EmpyreanMask : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.PostMoonLord";
        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Armor/Empyrean/MeldTransformation_Head", EquipType.Head, name: "MeldTransformation");
                EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Armor/Empyrean/MeldTransformation_Body", EquipType.Body, name: "MeldTransformation");
                EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Armor/Empyrean/MeldTransformation_Neck", EquipType.Neck, name: "MeldTransformation");
                EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Armor/Empyrean/MeldTransformation_Legs", EquipType.Legs, name: "MeldTransformation");
            }
        }

        public override void SetStaticDefaults()
        {

            if (Main.netMode == NetmodeID.Server)
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
            Item.value = CalamityGlobalItem.Rarity10BuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.defense = 20; //71
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<EmpyreanCloak>() && legs.type == ModContent.ItemType<EmpyreanCuisses>();
        }

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
            modPlayer.rogueStealthMax += 1.15f;
            player.setBonus = this.GetLocalizedValue("SetBonus");
            if (player.statLife <= (int)(player.statLifeMax2 * 0.5))
            {
                player.AddBuff(ModContent.BuffType<EmpyreanWrath>(), 2);
                player.AddBuff(ModContent.BuffType<EmpyreanRage>(), 2);
            }
            player.GetDamage<ThrowingDamageClass>() += 0.09f;
            modPlayer.rogueVelocity += 0.09f;
            modPlayer.wearingRogueArmor = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ThrowingDamageClass>() += 0.11f;
            player.GetCritChance<ThrowingDamageClass>() += 11;
            player.moveSpeed += 0.05f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MeldConstruct>(10).
                AddIngredient(ItemID.LunarBar, 8).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
