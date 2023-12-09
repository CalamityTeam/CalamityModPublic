using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Potions.Alcohol;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Silva
{
    [AutoloadEquip(EquipType.Head)]
    [LegacyName("SilvaHelmet")]
    public class SilvaHeadSummon : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.PostMoonLord";
        public static readonly SoundStyle ActivationSound = new("CalamityMod/Sounds/Custom/AbilitySounds/SilvaActivation");
        public static readonly SoundStyle DispelSound = new("CalamityMod/Sounds/Custom/AbilitySounds/SilvaDispel");

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 24;
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.defense = 13; //110
            Item.rare = ModContent.RarityType<DarkBlue>();
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<SilvaArmor>() && legs.type == ModContent.ItemType<SilvaLeggings>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            var modPlayer = player.Calamity();
            modPlayer.silvaSet = true;
            modPlayer.silvaSummon = true;
            modPlayer.WearingPostMLSummonerSet = true;
            player.setBonus = this.GetLocalizedValue("SetBonus") + "\n" + CalamityUtils.GetTextValueFromModItem<SilvaArmor>("CommonSetBonus");
            if (player.whoAmI == Main.myPlayer)
            {
                var source = player.GetSource_ItemUse(Item);
                if (player.FindBuffIndex(ModContent.BuffType<SilvaCrystalBuff>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<SilvaCrystalBuff>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<SilvaCrystal>()] < 1)
                {
                    // 08DEC2023: Ozzatron: Silva Crystals spawned with Old Fashioned active will retain their bonus damage indefinitely. Oops. Don't care.
                    int baseDamage = player.ApplyArmorAccDamageBonusesTo(600);
                    var damage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(baseDamage);

                    var p = Projectile.NewProjectile(source, player.Center.X, player.Center.Y, 0f, -1f, ModContent.ProjectileType<SilvaCrystal>(), damage, 0f, Main.myPlayer, -20f, 0f);
                    if (Main.projectile.IndexInRange(p))
                        Main.projectile[p].originalDamage = baseDamage;
                }
            }
            player.GetDamage<SummonDamageClass>() += 0.65f;
            player.maxMinions += 5;
        }

        public override void UpdateEquip(Player player) => player.GetDamage<SummonDamageClass>() += 0.1f;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PlantyMush>(6).
                AddIngredient<EffulgentFeather>(5).
                AddIngredient<AscendantSpiritEssence>(2).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
