using CalamityMod.CustomRecipes;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.DraedonsArsenal;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    public class FrequencyManipulator : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frequency Manipulator");
            Tooltip.SetDefault("A long device, used in the tuning of some rather... original machines\n" +
                               "Swings a spear around and then throws it\n" +
                               "On collision, the spear releases a burst of homing energy\n" +
                               "Stealth strikes release more energy and explode on collision");
        }

        public override void SafeSetDefaults()
        {
            CalamityGlobalItem modItem = item.Calamity();

            item.damage = 80;
            modItem.rogue = true;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.width = 26;
            item.height = 44;
            item.useTime = 56;
            item.useAnimation = 56;
            item.autoReuse = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 5f;

            item.value = CalamityGlobalItem.Rarity5BuyPrice;
            item.rare = ItemRarityID.Red;
            modItem.customRarity = CalamityRarity.DraedonRust;
            item.UseSound = SoundID.Item1;

            item.shootSpeed = 16f;
            item.shoot = ModContent.ProjectileType<FrequencyManipulatorProjectile>();

            modItem.UsesCharge = true;
            modItem.MaxCharge = 85f;
            modItem.ChargePerUse = 0.04f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;

        public override void SafeModifyTooltips(List<TooltipLine> tooltips) => CalamityGlobalItem.InsertKnowledgeTooltip(tooltips, 2);

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
                damage = (int)(damage * 0.9f);

            int proj = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
            if (proj.WithinBounds(Main.maxProjectiles))
                Main.projectile[proj].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }

        public override void AddRecipes()
        {
            ArsenalTierGatedRecipe recipe = new ArsenalTierGatedRecipe(mod, 2);
            recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 8);
            recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 12);
            recipe.AddRecipeGroup("AnyMythrilBar", 10);
            recipe.AddIngredient(ItemID.SoulofSight, 20);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
