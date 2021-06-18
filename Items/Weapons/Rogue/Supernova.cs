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
            item.width = 34;
            item.damage = 675;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 24;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 24;
            item.knockBack = 8f;
            item.UseSound = SoundID.Item15;
            item.autoReuse = true;
            item.height = 36;
            item.value = Item.buyPrice(platinum: 2, gold: 50);
            item.rare = ItemRarityID.Red;
            item.shoot = ModContent.ProjectileType<SupernovaBomb>();
            item.shootSpeed = 16f;
            item.Calamity().rogue = true;
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                damage = (int)(damage * 0.9f);
                int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
				if (stealth.WithinBounds(Main.maxProjectiles))
					Main.projectile[stealth].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(ModContent.ItemType<TotalityBreakers>());
            recipe.AddIngredient(ModContent.ItemType<BallisticPoisonBomb>());
            recipe.AddIngredient(ModContent.ItemType<ShockGrenade>(), 200);
            recipe.AddIngredient(ModContent.ItemType<Penumbra>());
            recipe.AddIngredient(ModContent.ItemType<StarofDestruction>());
			recipe.AddIngredient(ModContent.ItemType<SealedSingularity>());

			recipe.AddIngredient(ModContent.ItemType<AuricBar>(), 4);
			recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
