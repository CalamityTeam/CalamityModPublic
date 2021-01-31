using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Animus : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Animus");
            Tooltip.SetDefault("Randomizes its damage on enemy hits");
        }

        public override void SetDefaults()
        {
            item.width = 82;
            item.height = 84;
			item.scale = 1.5f;
            item.damage = 4000;
            item.melee = true;
            item.useAnimation = 11;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 11;
            item.useTurn = true;
            item.knockBack = 20f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = Item.buyPrice(5, 0, 0, 0);
            item.rare = 10;
            item.Calamity().customRarity = CalamityRarity.HotPink;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<BladeofEnmity>());
            recipe.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void ModifyWeaponDamage(Player player, ref float add, ref float mult, ref float flat)
		{
			float damageMult = player.Calamity().animusBoost;
			damageMult -= 1f;
			mult += damageMult;
		}

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 6000);
            int damageRan = Main.rand.Next(195); //0 to 194
            if (damageRan >= 50 && damageRan <= 99) //25%
            {
                player.Calamity().animusBoost = 1.5f;
            }
            else if (damageRan >= 100 && damageRan <= 139) //20%
            {
                player.Calamity().animusBoost = 2.25f;
            }
            else if (damageRan >= 140 && damageRan <= 169) //15%
            {
                player.Calamity().animusBoost = 3.75f;
            }
            else if (damageRan >= 170 && damageRan <= 189) //10%
            {
                player.Calamity().animusBoost = 7.5f;
            }
            else if (damageRan >= 190 && damageRan <= 194) //5%
            {
                player.Calamity().animusBoost = 12.5f;
            }
            else
            {
                player.Calamity().animusBoost = 1f;
            }
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 6000);
            int damageRan = Main.rand.Next(195); //0 to 194
            if (damageRan >= 50 && damageRan <= 99) //25%
            {
                player.Calamity().animusBoost = 1.5f;
            }
            else if (damageRan >= 100 && damageRan <= 139) //20%
            {
                player.Calamity().animusBoost = 2.25f;
            }
            else if (damageRan >= 140 && damageRan <= 169) //15%
            {
                player.Calamity().animusBoost = 3.75f;
            }
            else if (damageRan >= 170 && damageRan <= 189) //10%
            {
                player.Calamity().animusBoost = 7.5f;
            }
            else if (damageRan >= 190 && damageRan <= 194) //5%
            {
                player.Calamity().animusBoost = 12.5f;
            }
            else
            {
                player.Calamity().animusBoost = 1f;
            }
        }
    }
}
