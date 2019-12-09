using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class NanoblackReaperRogue : RogueWeapon
    {
        public static int BaseDamage = 600;
        public static float Knockback = 9f;
        public static float Speed = 16f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nanoblack Reaper");
            Tooltip.SetDefault("Unleashes a storm of nanoblack energy blades\nBlades target bosses whenever possible\n'She smothered them in Her hatred'");
        }

        public override void SafeSetDefaults()
        {
            item.width = 78;
            item.height = 64;
            item.damage = BaseDamage;
            item.knockBack = Knockback;
            item.useTime = 6;
            item.useAnimation = 6;
            item.autoReuse = true;
            item.noMelee = true;
            item.noUseGraphic = true;

            item.useStyle = 1;
            item.UseSound = SoundID.Item18;

            item.rare = 10;
            item.Calamity().postMoonLordRarity = 16;
            item.value = Item.buyPrice(5, 0, 0, 0);

            item.Calamity().rogue = true;
            item.shoot = ModContent.ProjectileType<NanoblackMain>();
            item.shootSpeed = Speed;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int proj = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
            Main.projectile[proj].Calamity().forceRogue = true;
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.SetResult(this);
            r.AddTile(ModContent.TileType<DraedonsForge>());
            r.AddIngredient(ModContent.ItemType<GhoulishGouger>());
            r.AddIngredient(ModContent.ItemType<SoulHarvester>());
            r.AddIngredient(ModContent.ItemType<EssenceFlayer>());
            r.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5);
            r.AddIngredient(ModContent.ItemType<EndothermicEnergy>(), 40);
            r.AddIngredient(ModContent.ItemType<DarkPlasma>(), 10);
            r.AddIngredient(ItemID.Nanites, 400);
            r.AddRecipe();
        }
    }
}
