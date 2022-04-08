using Terraria.DataStructures;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.Weapons.Summon
{
    public class PrototypeAndromechaRing : ModItem
    {
        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Mod.AddEquipTexture(this, EquipType.Head, "CalamityMod/ExtraTextures/AndromedaWithout_Head");
            }
        }

        public const int CrippleTime = 360; // 6 seconds
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flamsteed Ring");
            Tooltip.SetDefault("Summons a colossal controllable mech\n" +
                "Right click to display the mech's control panel\n" +
                "The panel has 3 configurations, selected using the brackets on the edges of the UI\n" +
                "Each bracket powers 2 out of 3 possible functions, represented by the circular icons.\n" +
                "The bottom left icon miniaturizes the mech to the size of a player, but weakens its weapons.\n" +
                "The bottom right icon is a powerful jet booster which greatly enhances movement.\n" +
                "The top icon is the mech's weaponry. It must be powered in order to attack.\n" +
                "Click the top icon to switch between Regicide, an enormous energy blade, and a powerful Gauss rifle.\n" +
                "Exiting the mount while a boss is alive will temporarily hinder your movement\n" +
            CalamityUtils.ColorMessage("Now, make them pay.", new Color(135, 206, 235)));

            int equipSlotHead = Mod.GetEquipSlot(Name, EquipType.Head);
            ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;
        }

        public override void SetDefaults()
        {
            Item.mana = 200;
            Item.damage = 1999;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.width = Item.height = 28;
            Item.useTime = Item.useAnimation = 9;
            Item.noMelee = true;
            Item.knockBack = 1f;

            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.Calamity().customRarity = CalamityRarity.HotPink;
            Item.Calamity().devItem = true;

            Item.UseSound = SoundID.Item117;
            Item.shoot = ModContent.ProjectileType<GiantIbanRobotOfDoom>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
            Item.Calamity().CannotBeEnchanted = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<Excelsus>()).AddIngredient(ModContent.ItemType<CosmicViperEngine>()).AddIngredient(ItemID.WingsVortex).AddIngredient(ModContent.ItemType<CosmiliteBar>(), 40).AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5).AddTile(ModContent.TileType<DraedonsForge>()).Register();
        }

        public override bool CanUseItem(Player player) => !(player.Calamity().andromedaCripple > 0 && CalamityPlayer.areThereAnyDamnBosses);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // If the player has any robots, kill them all.
            if (player.ownedProjectileCounts[Item.shoot] > 0)
            {
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    if (Main.projectile[i].active &&
                        Main.projectile[i].type == Item.shoot &&
                        Main.projectile[i].owner == player.whoAmI)
                    {
                        Main.projectile[i].Kill();
                    }
                }
                if (CalamityPlayer.areThereAnyDamnBosses)
                {
                    player.Calamity().andromedaCripple = CrippleTime;
                    player.AddBuff(ModContent.BuffType<AndromedaCripple>(), player.Calamity().andromedaCripple);
                    SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/AndromedaCripple"), position);
                }
                return false;
            }
            // Otherwise create one.
            return true;
        }

        // Moved from CalamityGlobalItem since it's just a function called in one place.
        internal static bool TransformItemUsage(Item item, Player player)
        {
            if (player.whoAmI != Main.myPlayer)
                return false;

            int robotIndex = -1;

            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].active &&
                    Main.projectile[i].type == ModContent.ProjectileType<GiantIbanRobotOfDoom>() &&
                    Main.projectile[i].owner == player.whoAmI)
                {
                    robotIndex = i;
                    break;
                }
            }
            if (robotIndex != -1)
            {
                Projectile robot = Main.projectile[robotIndex];
                GiantIbanRobotOfDoom robotModProjectile = ((GiantIbanRobotOfDoom)robot.ModProjectile);
                if (player.ownedProjectileCounts[ModContent.ProjectileType<AndromedaRegislash>()] <= 0 &&
                    robotModProjectile.TopIconActive &&
                    (robotModProjectile.RightIconCooldown <= GiantIbanRobotOfDoom.RightIconAttackTime ||
                     !robotModProjectile.RightIconActive)) // "Melee" attack
                {
                    var source = player.GetProjectileSource_Item(item);
                    int damage = player.Calamity().andromedaState == AndromedaPlayerState.SmallRobot ? GiantIbanRobotOfDoom.RegicideBaseDamageSmall : GiantIbanRobotOfDoom.RegicideBaseDamageLarge;
                    Projectile.NewProjectileDirect(source, robot.Center + (robot.spriteDirection > 0).ToDirectionInt() * robot.width / 2 * Vector2.UnitX,
                               Vector2.Zero, ModContent.ProjectileType<AndromedaRegislash>(), damage, 15f, player.whoAmI, Projectile.GetByUUID(robot.owner, robot.whoAmI));
                }

                if (!robotModProjectile.TopIconActive &&
                    (robotModProjectile.LeftBracketActive || robotModProjectile.RightBracketActive) &&
                    !robotModProjectile.BottomBracketActive &&
                    robotModProjectile.LaserCooldown <= 0 &&
                    (robotModProjectile.RightIconCooldown <= GiantIbanRobotOfDoom.RightIconAttackTime ||
                     !robotModProjectile.RightIconActive)) // "Ranged" attack
                {
                    robotModProjectile.LaserCooldown = AndromedaDeathRay.TrueTimeLeft * 2;
                    if (player.Calamity().andromedaState == AndromedaPlayerState.SmallRobot)
                    {
                        robotModProjectile.LaserCooldown = AndromedaDeathRay.TrueTimeLeft + 1;
                    }
                }
            }
            return false;
        }
    }
}
