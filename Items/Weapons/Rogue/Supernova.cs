using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class Supernova : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Supernova");
            Tooltip.SetDefault(@"Creates a massive explosion on impact
Explodes into spikes and homing energy
Stealth strikes release energy as they fly");
        }

        public override void SafeSetDefaults()
        {
            Item.width = 34;
            Item.damage = 675;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 24;
            Item.knockBack = 8f;
            Item.UseSound = SoundID.Item15;
            Item.autoReuse = true;
            Item.height = 36;
            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.shoot = ModContent.ProjectileType<SupernovaBomb>();
            Item.shootSpeed = 16f;
            Item.Calamity().rogue = true;
            Item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                damage = (int)(damage * 1.08f);
                int stealth = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                if (stealth.WithinBounds(Main.maxProjectiles))
                    Main.projectile[stealth].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<TotalityBreakers>()).AddIngredient(ModContent.ItemType<BallisticPoisonBomb>()).AddIngredient(ModContent.ItemType<ShockGrenade>(), 200).AddIngredient(ModContent.ItemType<Penumbra>()).AddIngredient(ModContent.ItemType<StarofDestruction>()).AddIngredient(ModContent.ItemType<SealedSingularity>()).AddIngredient(ModContent.ItemType<MiracleMatter>()).AddTile(ModContent.TileType<DraedonsForge>()).Register();
        }
    }
}
