using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Systems.Collections;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Respiteblock : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<HeavyBleeding>()];
        }
        public override void SetDefaults()
        {
            Item.width = 108;
            Item.height = 40;
            Item.damage = 70;
            Item.knockBack = 9f;
            Item.useTime = 10;
            Item.useAnimation = 20;
            // In-game, the displayed axe power is 5x the value set here.
            // This corrects for trees having 500% hardness internally.
            // So that the axe power in the code looks like the axe power you see on screen, divide by 5.
            Item.axe = 610 / 5; 
            // The axe power is entirely for show. It instantly one shots trees.
            // ***Axe Power is an integer which increments in 5s. So any number not divisible by 5 is impossible without tooltip modifications.
            // See CalamityGlobalItemTooltip for axe power tooltip edit

            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<RespiteblockHoldout>();
            Item.shootSpeed = 1f;

            Item.rare = ItemRarityID.Red;
            Item.value = CalamityGlobalItem.RarityRedBuyPrice;
            Item.Calamity().donorItem = true;
        }
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.ButchersChainsaw).
                AddIngredient(ItemID.FragmentSolar, 4). // 413 is also a homestuck referene I think...
                AddIngredient<EssenceofSunlight>(13).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
