using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Projectiles.Melee.Spears;
using CalamityMod.Systems.Collections;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TenebreusTides : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetStaticDefaults()
        {
            ItemID.Sets.Spears[Type] = true;
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<CrushDepth>()];
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 68;
            Item.damage = 120;
            Item.knockBack = 4.5f;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 14;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<TenebreusTidesProjectile>();
            Item.shootSpeed = 12f;

            Item.value = CalamityGlobalItem.RarityRedBuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.Calamity().donorItem = true;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item1;
            Item.noMelee = true;
            Item.useTurn = true;
            Item.noUseGraphic = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AmidiasTrident>().
                AddIngredient<Atlantis>().
                AddIngredient(ItemID.InfluxWaver).
                AddIngredient<Lumenyl>(50).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
