using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Items.Accessories
{
    public class StarTaintedGenerator : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Star-Tainted Generator");
            Tooltip.SetDefault("+2 max minions, does not stack with downgrades\n" +
                    "7% increased minion damage\n" +
                    "Minion attacks spawn astral explosions and inflict several debuffs\n" +
                    "Grants immunity to Irradiated");
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 60;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().voltaicJelly = true;
            player.Calamity().starbusterCore = true;
            player.Calamity().starTaintedGenerator = true;
            player.GetDamage<SummonDamageClass>() += 0.07f;
            player.buffImmune[ModContent.BuffType<Irradiated>()] = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<JellyChargedBattery>().
                AddIngredient<NuclearRod>().
                AddIngredient<StarbusterCore>().
                AddIngredient<LifeAlloy>(3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            CalamityUtils.DrawInventoryCustomScale(
                spriteBatch,
                texture: TextureAssets.Item[Type].Value,
                position,
                frame,
                drawColor,
                itemColor,
                origin,
                scale,
                wantedScale: 0.8f,
                drawOffset: new(-1f, 0f)
            );
            return false;
        }
    }
}
