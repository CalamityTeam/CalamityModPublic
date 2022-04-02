using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class TacticiansTrumpCard : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tactician's Trump Card");
            Tooltip.SetDefault("Faint memories of a Princess from the future cross your mind...\n" +
                "Fires a sword beam that electrifies enemies on hit");
        }

        public override void SetDefaults()
        {
            item.width = 74;
            item.height = 70;
            item.damage = 248;
            item.knockBack = 7f;
            item.magic = true;
            item.useAnimation = item.useTime = 12;
            item.mana = 20;
            item.useTurn = true;
            item.autoReuse = true;
            item.shootSpeed = 13.5f;
            item.shoot = ModContent.ProjectileType<TacticiansTrumpCardProj>();
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;
            item.value = CalamityGlobalItem.Rarity12BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
            item.Calamity().donorItem = true;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
            {
                int dustType = Main.rand.NextBool() ? 132 : 264;
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, dustType);
                Main.dust[dust].noGravity = true;
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit) => target.AddBuff(BuffID.Electrified, 300);

        public override void OnHitPvp(Player player, Player target, int damage, bool crit) => target.AddBuff(BuffID.Electrified, 300);

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.CopperShortsword);
            recipe.AddIngredient(ModContent.ItemType<TomeofFates>());
            recipe.AddIngredient(ModContent.ItemType<FlareBolt>());
            recipe.AddIngredient(ModContent.ItemType<Tradewinds>());
            recipe.AddIngredient(ModContent.ItemType<NuclearFury>());
            recipe.AddIngredient(ModContent.ItemType<UeliaceBar>(), 5);
            recipe.AddIngredient(ModContent.ItemType<DarkPlasma>());
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.TinShortsword);
            recipe.AddIngredient(ModContent.ItemType<TomeofFates>());
            recipe.AddIngredient(ModContent.ItemType<FlareBolt>());
            recipe.AddIngredient(ModContent.ItemType<Tradewinds>());
            recipe.AddIngredient(ModContent.ItemType<NuclearFury>());
            recipe.AddIngredient(ModContent.ItemType<UeliaceBar>(), 5);
            recipe.AddIngredient(ModContent.ItemType<DarkPlasma>());
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
