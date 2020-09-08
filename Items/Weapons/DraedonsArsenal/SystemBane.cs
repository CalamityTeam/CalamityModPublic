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
            Tooltip.SetDefault("Can be used to quickly send out an electromagnetic blast, strong enough to target organic nervous systems.\n" +
                               "Hurls an unstable device which sticks to the ground and shocks nearby enemies with lightning\n" +
                               "Stealth strikes make the device emit a large, damaging EMP field\n" +
                               "Stacks up to 5");
        }

        public override void SafeSetDefaults()
        {
            CalamityGlobalItem modItem = item.Calamity();

            item.damage = 45;
            modItem.rogue = true;
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
            modItem.customRarity = CalamityRarity.DraedonRust;
            item.UseSound = SoundID.Item1;
            item.maxStack = 5;

            item.shootSpeed = 16f;
            item.shoot = ModContent.ProjectileType<SystemBaneProjectile>();

            modItem.UsesCharge = true;
            modItem.MaxCharge = 135f;
            modItem.ChargePerUse = 0.085f;
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
            recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 3);
            recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 3); // Less materials are used than unusual because this projectile is stackable.
            recipe.AddIngredient(ModContent.ItemType<BarofLife>(), 1);
            recipe.AddIngredient(ModContent.ItemType<InfectedArmorPlating>(), 1);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
