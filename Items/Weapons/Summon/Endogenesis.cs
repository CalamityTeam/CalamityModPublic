using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class Endogenesis : ModItem
    {
        //Cooper be like cool

        public static int AttackMode = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Endogenesis");
            Tooltip.SetDefault("Summons an ascended ice construct to protect you \n" +
                "Changes attack modes by resummoning or reusing the staff \n" +
                "The first mode makes it shoot sweeping lasers aimed at the enemy \n" +
                "The second mode sacrifices its limbs to shoot out homing projectiles \n" +
                "The third mode allows it to agressively tackle its enemies \n" +
                "The fourth mode makes the limbs function as endothermic flamethrowers \n" +
                "Requires 10 minion slots to be summoned \n" +
                "There can only be one \n" +
                "[c/B0FBFF:Ice puns not included]"); //Icy no problems with that
        }

        public override void SetDefaults()
        {
            item.width = 80;
            item.height = 80;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.UseSound = SoundID.Item78;

            item.summon = true;
            item.mana = 80;
            item.damage = 1300;
            item.knockBack = 4f;
            item.autoReuse = true;
            item.useTime = item.useAnimation = 10;
            item.shoot = ModContent.ProjectileType<EndoCooperBody>();
            item.shootSpeed = 10f;

            item.value = CalamityGlobalItem.Rarity16BuyPrice;
            item.Calamity().customRarity = CalamityRarity.HotPink;
            item.Calamity().devItem = true;
        }

        public override bool CanUseItem(Player player) => player.maxMinions >= 10f;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse != 2)
            {
                player.itemTime = item.useTime;
				CalamityUtils.KillShootProjectileMany(player, new int[]
				{
					type,
					ModContent.ProjectileType<EndoCooperLimbs>(),
					ModContent.ProjectileType<EndoBeam>()
				});

                SummonEndoCooper(AttackMode, Main.MouseWorld, damage, knockBack, player, out _, out _);

                AttackMode++;
                if (AttackMode > 3)
                    AttackMode = 0;
            }
            return false;
        }

        public static void SummonEndoCooper(int attackMode, Vector2 spawnPosition, int damage, float knockBack, Player owner, out int bodyIndex, out int limbsIndex)
        {
            bodyIndex = limbsIndex = -1;
            if (Main.myPlayer != owner.whoAmI)
                return;

            float dmgMult = 1f;
            if (attackMode == 0) //lasers
                dmgMult = 0.65f;
            if (attackMode == 1) //icicles
                dmgMult = 1f;
            if (attackMode == 2) //melee
                dmgMult = 0.95f;
            if (attackMode == 3) //flamethrower
                dmgMult = 0.9f;
            bodyIndex = Projectile.NewProjectile(spawnPosition, Vector2.Zero, ModContent.ProjectileType<EndoCooperBody>(), (int)(damage * dmgMult), knockBack, owner.whoAmI, attackMode, 0f);
            limbsIndex = Projectile.NewProjectile(spawnPosition, Vector2.Zero, ModContent.ProjectileType<EndoCooperLimbs>(), (int)(damage * dmgMult), knockBack, owner.whoAmI, attackMode, bodyIndex);
            Main.projectile[bodyIndex].ai[1] = limbsIndex;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CryogenicStaff>());
            recipe.AddIngredient(ItemID.BlizzardStaff);
            recipe.AddIngredient(ModContent.ItemType<EndothermicEnergy>(), 99);
            recipe.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5);
            recipe.AddIngredient(ModContent.ItemType<CoreofEleum>(), 5);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
