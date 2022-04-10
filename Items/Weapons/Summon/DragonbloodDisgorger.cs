using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class DragonbloodDisgorger : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dragonblood Disgorger");
            Tooltip.SetDefault("Summons a skeletal dragon and her two children\n" +
                               "Requires 6 minion slots to be summoned\n" +
                               "There can only be one family");
        }

        public override void SetDefaults()
        {
            Item.damage = 160;
            Item.mana = 10;
            Item.width = 64;
            Item.height = 62;
            Item.useTime = Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 4f;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
            Item.UseSound = SoundID.DD2_SkeletonDeath;
            Item.shoot = ModContent.ProjectileType<SkeletalDragonMother>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int p = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI);
            if (Main.projectile.IndexInRange(p))
                Main.projectile[p].originalDamage = Item.damage;
            return false;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0 && player.maxMinions >= 5;

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<BloodstoneCore>(), 12).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}
