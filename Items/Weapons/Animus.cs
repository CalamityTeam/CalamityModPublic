using Terraria;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
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
            item.width = 84;
            item.damage = 4000;
            item.melee = true;
            item.useAnimation = 11;
            item.useStyle = 1;
            item.useTime = 11;
            item.useTurn = true;
            item.knockBack = 20f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 90;
            item.value = Item.buyPrice(5, 0, 0, 0);
            item.rare = 10;
            item.Calamity().postMoonLordRarity = 16;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "BladeofEnmity");
            recipe.AddIngredient(null, "ShadowspecBar", 5);
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 6000);
            int damageRan = Main.rand.Next(195); //0 to 195
            if (damageRan >= 50 && damageRan <= 99) //25%
            {
                item.damage = 6000;
            }
            else if (damageRan >= 100 && damageRan <= 139) //20%
            {
                item.damage = 9000;
            }
            else if (damageRan >= 140 && damageRan <= 169) //15%
            {
                item.damage = 15000;
            }
            else if (damageRan >= 170 && damageRan <= 189) //10%
            {
                item.damage = 30000;
            }
            else if (damageRan >= 190 && damageRan <= 194) //5%
            {
                item.damage = 50000;
            }
            else
            {
                item.damage = 4000;
            }
        }
    }
}
