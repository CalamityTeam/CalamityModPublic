using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Animus : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Animus");
            Tooltip.SetDefault("Randomizes its damage on enemy hits");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 82;
            Item.height = 84;
            Item.scale = 1.5f;
            Item.damage = 800;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 11;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 11;
            Item.useTurn = true;
            Item.knockBack = 20f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = Item.buyPrice(5, 0, 0, 0);
            Item.rare = ItemRarityID.Red;
            Item.Calamity().customRarity = CalamityRarity.HotPink;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            damage *= player.Calamity().animusBoost;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 300);
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
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 300);
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

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BladeofEnmity>().
                AddIngredient<ShadowspecBar>(5).
                AddTile<DraedonsForge>().
                Register();
        }
    }
}
