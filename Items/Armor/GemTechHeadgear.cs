using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class GemTechHeadgear : ModItem
    {
        public const int GemBreakDamageLowerBound = 100;
        public const int GemDamage = 40000;
        public const int GemDamageSoftcapThreshold = 100000;
        public const int GemRegenTime = 1800;

        public const int MeleeShardBaseDamage = 825;
        public const int MeleeShardDelay = 330;
        public const float MeleeDamageBoost = 0.45f;
        public const float MeleeCritBoost = 0.12f;
        public const float MeleeSpeedBoost = 0.26f;

        public const int MaxFlechettes = 8;
        public const float RangedDamageBoost = 0.5f;
        public const float RangedCritBoost = 0.16f;

        public const int MagicManaBoost = 100;
        public const int NonMagicItemManaRegenBoost = 8;
        public const float MagicDamageBoost = 0.5f;
        public const float MagicCritBoost = 0.16f;

        public const int SummonMinionCountBoost = 4;
        public const float SummonDamageBoost = 0.72f;

        public const int RogueStealthBoost = 130;
        public const float RogueDamageBoost = 0.5f;
        public const float RogueCritBoost = 0.16f;

        public const int BaseGemDefenseBoost = 75;
        public const int BaseGemLifeRegenBoost = 2;
        public const float BaseGemDRBoost = 0.06f;
        public const float BaseGemMovementSpeedBoost = 0.4f;
        public const float BaseGemJumpSpeedBoost = 0.4f;

        public const int AllGemsWeaponUseLifeRegenBoost = 2;
        public const int AllGemsMultiWeaponUseLifeRegenBoost = 3;
        public const int AllGemsLifeRegenBoostTime = 480;
        public const int AllGemsMultiWeaponLifeRegenBoostTime = 150;

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Gem Tech Headgear");
            Tooltip.SetDefault("The Devil said: Revel in your victory; You've earned your damning. Pack your things and leave.");
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 32;
            Item.defense = 14;
            Item.rare = ItemRarityID.Purple;

            // Exact worth of the armor piece's constituents.
            Item.value = Item.sellPrice(platinum: 6, gold: 14, silver: 88);
            Item.Calamity().customRarity = CalamityRarity.Violet;
            Item.Calamity().donorItem = true;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<GemTechBodyArmor>() && legs.type == ModContent.ItemType<GemTechSchynbaulds>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.Calamity().GemTechSet = true;
            player.Calamity().wearingRogueArmor = true;
            if (player.Calamity().GemTechState.IsRedGemActive)
                player.Calamity().rogueStealthMax += RogueStealthBoost * 0.01f;

            string redGemText = $"The red gem grants {RogueStealthBoost} maximum stealth, increased rogue stats, and makes stealth only consumable by rogue weapons\n";
            string yellowGemText = $"The yellow gem provides increased melee stats and makes melee attacks release shards on hit with a cooldown. This cooldown is shorter for true melee attacks\n";
            string greenGemText = $"The green gem provides increased ranged stats and causes flechettes to fly swiftly towards targets when they are damaged by a ranged projectile\n";
            string blueGemText = $"The blue gem grants {SummonMinionCountBoost} extra maximum minions, increased minion damage, and reduces the penalty for summoner items while holding a non-summoner weapon\n";
            string purpleGemText = $"The violet gem grants {MagicManaBoost} extra maximum mana, increased magic stats, and makes mana rapidly regenerate when holding a non-magic weapon\n";
            string pinkGemText = $"The pink base gem grants {BaseGemDefenseBoost} extra defense, extra damage reduction, increased movement speed, jump speed, and +{BaseGemLifeRegenBoost} life regen\n";

            player.setBonus = "Six gem fragments idly orbit you; one for each class, and a base gem\n" +
                $"A gem is lost when you take more than {GemBreakDamageLowerBound} damage in a single hit. The type of gem lost is the same as the class of the previous when you used\n" +
                "If said gem has already been lost, the base gem is lost instead\n" +
                $"When a gem is lost, it breaks off and homes towards the nearest enemy or boss, if one is present, dealing a base of {GemDamage} damage\n" +
                $"Gems have a {GemRegenTime / 60} second delay before they appear again\n" +
                redGemText +
                yellowGemText +
                greenGemText +
                blueGemText +
                purpleGemText +
                pinkGemText +
                $"When all gems exist simultaneously, hitting a target with any weapon grants you +{AllGemsWeaponUseLifeRegenBoost} life regen for {AllGemsLifeRegenBoostTime / 60} seconds\n" +
                $"This is increased to +{AllGemsMultiWeaponUseLifeRegenBoost} life regen if a weapon of another class is used during that {AllGemsLifeRegenBoostTime / 60} second period for {AllGemsMultiWeaponLifeRegenBoostTime / 60f} seconds";
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ExoPrism>(10).
                AddIngredient<GalacticaSingularity>(3).
                AddIngredient<CoreofCalamity>(3).
                AddTile<DraedonsForge>().
                Register();
        }
    }
}
