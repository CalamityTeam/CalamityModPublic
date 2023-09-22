using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class Sirius : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";
        public override void SetDefaults()
        {
            Item.width = Item.height = 62;

            Item.damage = 600;
            Item.useTime = Item.useAnimation = 10;
            Item.mana = 10;
            Item.knockBack = 10f;
            
            Item.shoot = ModContent.ProjectileType<SiriusMinion>();

            Item.DamageType = DamageClass.Summon;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item44;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.value = CalamityGlobalItem.Rarity13BuyPrice;
            Item.noMelee = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0 && player.maxMinions >= 6;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            CalamityUtils.KillShootProjectiles(true, type, player);
            int p = Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI);
            if (Main.projectile.IndexInRange(p))
                Main.projectile[p].originalDamage = Item.damage;
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<VengefulSunStaff>().
                AddIngredient<Lumenyl>(5).
                AddIngredient<RuinousSoul>(2).
                AddIngredient<ExodiumCluster>(12).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
