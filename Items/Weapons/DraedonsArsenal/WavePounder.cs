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
    public class WavePounder : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wave Pounder");
            Tooltip.SetDefault("It utilizes its power to send heavy shockwaves throughout the area, causing agonizing internal damage\n" +
                               "Throws a bomb which explodes into a forceful shockwave\n" +
                               "Stealth strikes emit absurdly powerful shockwaves");
        }

        public override void SafeSetDefaults()
        {
            CalamityGlobalItem modItem = item.Calamity();

            item.damage = 75;
            modItem.rogue = true;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.width = 26;
            item.height = 44;
            item.useTime = 56;
            item.useAnimation = 56;
            item.autoReuse = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 0f;

            item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            item.rare = ItemRarityID.Red;

            modItem.customRarity = CalamityRarity.DraedonRust;
            item.UseSound = SoundID.Item1;

            item.shootSpeed = 16f;
            item.shoot = ModContent.ProjectileType<WavePounderProjectile>();

            modItem.UsesCharge = true;
            modItem.MaxCharge = 190f;
            modItem.ChargePerUse = 0.5f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int proj = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
            if (proj.WithinBounds(Main.maxProjectiles))
                Main.projectile[proj].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }

        public override void SafeModifyTooltips(List<TooltipLine> tooltips) => CalamityGlobalItem.InsertKnowledgeTooltip(tooltips, 4);

        public override void AddRecipes()
        {
            ArsenalTierGatedRecipe recipe = new ArsenalTierGatedRecipe(mod, 4);
            recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 12);
            recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 18);
            recipe.AddIngredient(ModContent.ItemType<UeliaceBar>(), 8);
            recipe.AddIngredient(ItemID.LunarBar, 4);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
