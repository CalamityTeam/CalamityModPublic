using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using CalamityMod.Systems.Collections;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class SpearofPaleolith : RogueWeapon
    {
        public static int ShardBaseDamage => 40;

        public static int NormalShardCount => 3;
        public static int ImpaledShardCount => 5;
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<ArmorCrunch>()];
        }
        public override void SetDefaults()
        {
            Item.width = 54;
            Item.height = 54;
            Item.damage = 90;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 27;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 27;
            Item.knockBack = 6f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ModContent.ProjectileType<SpearofPaleolithProj>();
            Item.shootSpeed = 6f;
            Item.DamageType = RogueDamageClass.Instance;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int stabDevice = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                if (stabDevice.WithinBounds(Main.maxProjectiles))
                    Main.projectile[stabDevice].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.AncientBattleArmorMaterial, 2).
                AddRecipeGroup("AnyAdamantiteBar", 4).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
