using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.DraedonsArsenal;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
	public class SystemBane : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("System Bane");
            Tooltip.SetDefault("Hurls an unstable device which sticks to the ground and shocks nearby enemies with lightning\n" +
                               "Stealth Strikes make the device emit a large, damaging EMP field\n" +
                               "Stacks up to 5");
        }

        public override void SafeSetDefaults()
        {
            item.damage = 45;
            item.Calamity().rogue = true;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.width = 42;
            item.height = 36;
            item.useTime = 15;
            item.useAnimation = 15;
            item.autoReuse = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 4f;
            item.value = CalamityGlobalItem.Rarity8BuyPrice;
            item.rare = ItemRarityID.Red;
            item.Calamity().customRarity = CalamityRarity.DraedonRust;
            item.UseSound = SoundID.Item1;
            item.maxStack = 5;

            item.shootSpeed = 16f;
            item.shoot = ModContent.ProjectileType<SystemBaneProjectile>();
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] < item.stack;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectileDirect(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, 0f, 0f).Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 4);
            recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 4); // Less materials are used than unusual because this projectile is stackable.
            recipe.AddIngredient(ModContent.ItemType<InfectedArmorPlating>(), 2);
            recipe.AddIngredient(ModContent.ItemType<BarofLife>(), 1);
            recipe.AddIngredient(ItemID.SpikyBall, 20);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
