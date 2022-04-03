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
            Item.width = 74;
            Item.height = 70;
            Item.damage = 248;
            Item.knockBack = 7f;
            Item.DamageType = DamageClass.Magic;
            Item.useAnimation = Item.useTime = 12;
            Item.mana = 20;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.shootSpeed = 13.5f;
            Item.shoot = ModContent.ProjectileType<TacticiansTrumpCardProj>();
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
            Item.Calamity().donorItem = true;
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
            CreateRecipe(1).AddIngredient(ItemID.CopperShortsword).AddIngredient(ModContent.ItemType<TomeofFates>()).AddIngredient(ModContent.ItemType<FlareBolt>()).AddIngredient(ModContent.ItemType<Tradewinds>()).AddIngredient(ModContent.ItemType<NuclearFury>()).AddIngredient(ModContent.ItemType<UeliaceBar>(), 5).AddIngredient(ModContent.ItemType<DarkPlasma>()).AddTile(TileID.LunarCraftingStation).Register();
            CreateRecipe(1).AddIngredient(ItemID.TinShortsword).AddIngredient(ModContent.ItemType<TomeofFates>()).AddIngredient(ModContent.ItemType<FlareBolt>()).AddIngredient(ModContent.ItemType<Tradewinds>()).AddIngredient(ModContent.ItemType<NuclearFury>()).AddIngredient(ModContent.ItemType<UeliaceBar>(), 5).AddIngredient(ModContent.ItemType<DarkPlasma>()).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}
