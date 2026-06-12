using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using CalamityMod.Systems.Collections;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class TauCannon : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<AstralInfectionDebuff>()];
        }

        public override void SetDefaults()
        {
            Item.damage = 620;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 180;
            Item.shoot = ModContent.ProjectileType<TauCannonHoldout>();
            Item.shootSpeed = 15f;
            Item.knockBack = 4f;

            Item.width = 146;
            Item.height = 52;
            Item.noMelee = true;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.Calamity().donorItem = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] == 0;

        public override void HoldItem(Player player) => player.Calamity().mouseRotationListener = true;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ArcNovaDiffuser>().
                AddIngredient<MysteriousCircuitry>(10).
                AddIngredient<DubiousPlating>(15).
                AddIngredient<AstralBar>(10).
                AddIngredient<RuinousSoul>(2).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
